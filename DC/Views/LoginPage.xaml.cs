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
using RestSharp.Serializers.SystemTextJson;

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
            var api_client = new APICall("users/salt/", "POST", new LoginInfo(username, null));
            var salt_response = await api_client.make_call<string>();
            if (!salt_response.success) //Unable to get salt
            {
                await DisplayAlert("Error", salt_response.error, "OK");
                En_Password.Text = "";
                return false;
            }
            string salt = salt_response.data;
            string passhash = getHashedPassword(salt);
            var api_login_client = new APICall("login/", "POST", new LoginInfo(username, passhash));
            var response = await api_login_client.make_call<Tokens>();
            if (!response.success) //Unable to log in
            {
                await DisplayAlert("Error", response.error, "OK");
                En_Password.Text = "";
                return false;
            }
            else
            {
                Tokens tokens = response.data;
                var api_user_call = new APICall("users/current/", "GET");
                string authheader = "Bearer " + tokens.SessionToken;
                api_user_call.addHeader("Authorization", authheader);
                var user_response = await api_user_call.make_call<User>();
                if (!user_response.success)
                {
                    await DisplayAlert("Error", user_response.error, "OK");
                    En_Password.Text = "";
                    return false;
                }
                App.currentUser = user_response.data;
                App.currentUser.User_tokens = tokens;
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
