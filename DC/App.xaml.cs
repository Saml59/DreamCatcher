using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DC.Services;
using DC.Views;
using DC.Data;

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
        public static TokenDatabaseController tokenDB;
        public static UserDatabaseController userDB;

        public App()
        {
            InitializeComponent();

            if (UseMockDataStore)
                DependencyService.Register<MockDataStore>();
            else
                DependencyService.Register<AzureDataStore>();
            MainPage = new LoginPage();
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

        public static TokenDatabaseController TokenDB
        {
            get
            {
                if (tokenDB == null)
                {
                    tokenDB = new TokenDatabaseController();
                }
                return tokenDB;
            }
        }
        public static UserDatabaseController UserDB
        {
            get
            {
                if (userDB == null)
                {
                    userDB = new UserDatabaseController();
                }
                return userDB;
            }
        }
    }
}
