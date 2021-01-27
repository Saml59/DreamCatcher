using System;
using Xamarin.Forms;
using Amazon;
using DC.Models;
using DC.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;
using System.Text.Json;
using System.Text.Json.Serialization;
using RestSharp.Serializers.SystemTextJson;

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
            if (Birthdate.Date.Year <= 1920)
            {
                await DisplayAlert("Error", "You are not over 100 years old", "Guilty as Charged");
                En_Password.Text = "";
                En_Password_Confirm.Text = "";
                return;
            }
            string email = En_Email.Text;
            string username = En_Username.Text;
            string password = En_Password.Text;
            string firstname = En_FirstName.Text;
            string middleinitial = En_MiddleInitial.Text;
            string lastname = En_LastName.Text;
            var dob = Birthdate.Date;
            int day = dob.Day;
            int month = dob.Month;
            int year = dob.Year;
            int role_index = Role_Picker.SelectedIndex;
            string role = "";

            //determine the role based on the index
            User user;
            if (role_index == 0)
            {
                role = "student";
                user = new Student(email, username, password, firstname, middleinitial, lastname, month, day, year, role);
            }
            else if (role_index == 1)
            {
                role = "tutor";
                user = new Tutor(email, username, password, firstname, middleinitial, lastname, month, day, year, role);
            }
            else if (role_index == 2)
            {
                role = "instructor";
                user = new Instructor(email, username, password, firstname, middleinitial, lastname, month, day, year, role);
            }
            else
            {
                user = new User(email, username, password, firstname, middleinitial, lastname, month, day, year, role);
            }
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
            var client = new RestClient(Constant.BaseURL);
            client.UseSystemTextJson();
            var serialized_user = JsonSerializer.Serialize(user);
            var request = new RestRequest("users/", Method.POST).AddJsonBody(serialized_user);
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            var iresponse = await client.ExecuteAsync(request);
            if (!iresponse.IsSuccessful)
            {
                await DisplayAlert("Error", iresponse.ErrorMessage, "OK");
                return;
            }
            Generic_Response<Tokens> response = JsonSerializer.Deserialize<Generic_Response<Tokens>>(iresponse.Content);

            if (response.success)
            {
                
                user.User_tokens = response.data;
                sentInfo = true;
                return;
            }
            else 
            {
                await DisplayAlert("Error", response.error, "OK");
                En_Username.Text = "";
                En_Password.Text = "";
                En_Password_Confirm.Text = "";
                sentInfo = false;
                return;
            }

        }
    }
}
