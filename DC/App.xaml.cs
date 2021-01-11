using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DC.Services;
using DC.Views.Menu;
using DC.Views;
using DC.Models;
using System.Threading;
using System.Threading.Tasks;

namespace DC
{
    public partial class App : Application
    {
        //TODO: Replace with *.azurewebsites.net url after deploying backend to Azure
        //To debug on Android emulators run the web backend against .NET Core not IIS
        //If using other emulators besides stock Google images you may need to adjust the IP address
        public static string AzureBackendUrl =
            DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5000" : "http://localhost:5000";
        public static bool UseMockDataStore = true;
        public static Label LabelScreen;
        public static Timer timer;
        public static bool HasInternet;
        public static User currentUser;

        public App()
        {
            InitializeComponent();

            if (UseMockDataStore)
                DependencyService.Register<MockDataStore>();
            else
                DependencyService.Register<AzureDataStore>();
            //TODO check if already logged in
            NavigationPage nav = new NavigationPage();
            if (!Constant.LoggedIn)
            {
                LoginPage lgin = new LoginPage();
                nav.PushAsync(lgin);
            }
            else
            {
                Dashboard dash = new Dashboard();
                nav.PushAsync(dash);
            }
            
            MainPage = nav;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        

  
    }
    
}
