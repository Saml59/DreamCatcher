using System;
using System.Collections.Generic;
using DC.Models;
using Xamarin.Forms;

namespace DC.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            Init();
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
        void SignInProcess(object sender, EventArgs e)
        {
            User user = new User(En_Username.Text, En_Password.Text);
            if (user.checkInformation())
            {
                DisplayAlert("Login", "Login Successful", "OK");
                App.UserDB.saveUser(user);
            }
            else DisplayAlert("Login", "Login Not Successful", "OK");
        }
    }
}
