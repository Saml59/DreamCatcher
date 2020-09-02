using System;
using System.Collections.Generic;
using DC.Models;
using DC.Views.Menu;
using Xamarin.Forms;
using Xamarin.Essentials;

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

            User user = new User(En_Username.Text, En_Password.Text);
            if (user.checkInformation())
            { 
                //var result =  await App.RestService.Login(user);
                if (true) //result.access_token != null)
                {
                    //App.UserDB.saveUser(user);
                    //App.TokenDB.saveToken(result);
                    await DisplayAlert("Login", "Login Successful", "OK");
                    Navigation.InsertPageBefore(new Dashboard(), this);
                    await Navigation.PopAsync();
                }
            }
            else await DisplayAlert("Login", "Login Not Successful", "OK");
        }
    }
}
