using System;
using System.Collections.Generic;
using DC.Models;
using DC.Views.Menu;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Text;
using System.Security.Cryptography;
using Amazon.DynamoDBv2.DataModel;
using System.Threading.Tasks;

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
            else await DisplayAlert("Login", "Login Not Successful", "OK");
        }
        
        private async Task<Boolean> checkInformation()
        {
            // Initialize the Amazon Cognito credentials provider
            var credentials = new Amazon.CognitoIdentity.CognitoAWSCredentials("us-east-1:68f3ffe2-5c4c-4f76-9d2b-a82b20f93716", Amazon.RegionEndpoint.USEast1);
            var client = new Amazon.DynamoDBv2.AmazonDynamoDBClient(credentials, Amazon.RegionEndpoint.USEast1);
            DynamoDBContext context = new DynamoDBContext(client);
            User serverUser = await context.LoadAsync<User>(En_Username.Text);
            if (serverUser == null)
            {
                await DisplayAlert("Error", "Username and/or Password are incorrect", "OK");
                En_Password.Text = "";
                return false;
            }
            string salt = serverUser.Salt;
            string passHash = getHashedPassword(salt);
            if (serverUser.Passhash != passHash)
            {
                await DisplayAlert("Error", "Username and/or Password are incorrect", "OK");
                En_Password.Text = "";
                return false;
            }
            else
            {
                App.currentUser = serverUser;
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
