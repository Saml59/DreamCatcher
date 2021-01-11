using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DC.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {

        public DateTime Date => DateTime.MinValue;
            
        public RegisterViewModel()
        {
            Title = "About";
           
        }

        public ICommand OpenWebCommand { get; }
    }
}