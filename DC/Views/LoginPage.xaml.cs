using System;
using System.Collections.Generic;
using DC.Models;
using DC.Views.Menu;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using RestSharp;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DC.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            Init();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        void Init()
        { 
            BackgroundColor = Constant.BackgroundColor;
            Lb_Username.TextColor = Constant.MainTextColor;
            Lb_Password.TextColor = Constant.MainTextColor;
            ActivitySpinner.IsVisible = false;
            Logo.HeightRequest = Constant.LogoHeight;
         

            En_Username.Completed += (s, e) => En_Password.Focus();
            En_Password.Completed += (s, e) => SignInProcess(s, e);
        }


        async void SignInProcess(object sender, EventArgs e)
        {

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("Error", "No Internet Access", "Retry");
                return;

            }
            //check with server to make sure that this is the correct login information
            if (await checkInformation())
            { 
                await DisplayAlert("Login", "Login Successful", "OK");
                Navigation.InsertPageBefore(new Dashboard(), this);
                await Navigation.PopAsync();
                
            }
        }
        
        private async Task<Boolean> checkInformation()
        {
            string username = En_Username.Text;
            var client = new RestClient(Constant.BaseURL);
            var salt_request = new RestRequest("users/salt/").AddJsonBody(new LoginInfo(username, null));
            var salt_response = await client.GetAsync<Generic_Response<Dictionary<string, string>>>(salt_request);
            if (!salt_response.success)
            {
                await DisplayAlert("Error", salt_response.error, "OK");
                En_Password.Text = "";
                return false;
            }
            string salt = salt_response.data["salt"];
            string passhash = getHashedPassword(salt);
            var request = new RestRequest("login/").AddJsonBody(new LoginInfo(username, passhash));
            var response = await client.PostAsync<Generic_Response<Tokens>>(request);
            if (!response.success)
            {
                await DisplayAlert("Error", response.error, "OK");
                En_Password.Text = "";
                return false;
            }
            else
            {
                var user_request = new RestRequest("users/current/");
                string authheader = "Bearer " + response.data.SessionToken;
                user_request.AddHeader("Authorization", authheader);
                var user_response = await client.GetAsync<Generic_Response<User>>(user_request);
                if (!user_response.success)
                {
                    await DisplayAlert("Error", user_response.error, "OK");
                    En_Password.Text = "";
                    return false;
                }
                App.currentUser = user_response.data;
                App.currentUser.User_tokens = response.data;
                App.currentUser.Passhash = passhash;
                App.currentUser.Salt = salt;
                return true;
            }

        }


        string getHashedPassword(string salt)
        {
            //prehashing string of the salt + the password
            string preHash = salt + En_Password.Text;
            SHA256 hasher = SHA256.Create();
            //create the hash in a bytearray form
            byte[] rawHash = hasher.ComputeHash(Encoding.UTF8.GetBytes(preHash));
            return Encoding.UTF8.GetString(rawHash, 0, rawHash.Length);
        }
        
        async void Register(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Register());
        }
    }
}
