using System;
using System.Collections.Generic;
using DC.Models;
using DC.Views.DetailViews;
using Xamarin.Forms;

namespace DC.Views.Menu
{
    public partial class Dashboard : ContentPage
    {
        public Dashboard()
        {
            InitializeComponent();
            Init();
        }


        void Init()
        {
            BackgroundColor = Constant.BackgroundColor;
            
        }

        async void SelectedScreen1(Object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new InfoScreen1());
        }
    }
}
