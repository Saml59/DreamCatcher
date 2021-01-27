using System;
using System.Collections.Generic;
using Xamarin.Forms;
namespace DC.Models
{
    public class Constant
    {
        public static bool isDev = true;
        public static Color BackgroundColor = Color.FromRgb(1, 125, 32);
        public static Color MainTextColor = Color.FromRgb(0, 0, 0);
        public static int LogoHeight = 120;
        public static string NoInternetText = "No Internet connectivity, please check your settings";
        public static bool LoggedIn = false;


        //Backend info
        public static string BaseURL = "http://localhost:5000/api";


        public Constant()
        {
        }
    }
    public class Generic_Response<T>
    {
        public Boolean success { get; set; }
        public T data { get; set; }
        public string error { get; set; }
    }
}
