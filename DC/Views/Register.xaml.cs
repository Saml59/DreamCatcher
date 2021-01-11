using System;
using Xamarin.Forms;
using Amazon;
using Amazon.DynamoDBv2.DataModel;
using DC.Models;
using DC.ViewModels;
using Amazon.DynamoDBv2.DocumentModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DC.Views
{
    public partial class Register : ContentPage
    {
        private Boolean sentInfo = false;

        public Register()
        {
            InitializeComponent();
            BindingContext = new RegisterViewModel();
        }

        async void createAccount(object sender, EventArgs e)
        {
            //check that passwords match
            if (!En_Password.Text.Equals(En_Password_Confirm.Text))
            {
                await DisplayAlert("Error", "Passwords do not match", "OK");
                En_Password.Text = "";
                En_Password_Confirm.Text = "";
                return;
            }
            //check that password is at least 8 characters long
            if (En_Password.Text.Length < 8)
            {
                await DisplayAlert("Error", "Passwords is not 8 characters long", "OK");
                En_Password.Text = "";
                En_Password_Confirm.Text = "";
                return;
            }
            //check to make sure a person did not input a ridiculous age (>90)
            if (Birthdate.Date.Year <= 1930)
            {
                await DisplayAlert("Error", "You are not over 90 years old", "Guilty as Charged");
                En_Password.Text = "";
                En_Password_Confirm.Text = "";
                return;
            }



            string username = En_Username.Text;
            string password = En_Password.Text;
            string firstname = En_FirstName.Text;
            string lastname = En_LastName.Text;
            var dob = Birthdate.Date;
            int day = dob.Day;
            int month = dob.Month;
            int year = dob.Year;
            User user = new User(username, password, firstname, lastname, month, day, year);
            await sendData(user);
            //TODO: check for properly inputted information, internet connection, and then send data to the server
            if (!sentInfo)
            {
                return;
            }
            else await Navigation.PopAsync();
        }

        private async Task sendData(User user)
        {
            // Initialize the Amazon Cognito credentials provider
            var credentials = new Amazon.CognitoIdentity.CognitoAWSCredentials("us-east-1:68f3ffe2-5c4c-4f76-9d2b-a82b20f93716", Amazon.RegionEndpoint.USEast1);
            var client = new Amazon.DynamoDBv2.AmazonDynamoDBClient(credentials, Amazon.RegionEndpoint.USEast1);
            DynamoDBContext context = new DynamoDBContext(client);
            //check that the username is not in the accounts already
            User sameUsername = await context.LoadAsync<User>(user.Username);
            if (sameUsername != null)
            {
                await DisplayAlert("Error", "Username already taken", "OK");
                En_Username.Text = "";
                En_Password.Text = "";
                En_Password_Confirm.Text = "";
                sentInfo = false;
                return;
            }
            else
            {
                await context.SaveAsync(user);
                sentInfo = true;
            }

        }
    }
}
