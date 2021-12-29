using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PM.ViewModel
{
    public partial class MaintenanceSetup : ContentPage
    {
        public MaintenanceSetup()
        {
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);

            InitializeComponent();
        }

        void clickedBack(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        void clickedDone(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new SignUpFinish();
        }
    }
}
