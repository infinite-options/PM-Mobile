using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PM.ViewModel
{
    public partial class SignUpFinish : ContentPage
    {
        public SignUpFinish()
        {
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;

            InitializeComponent();

            managerRadio.IsVisible = Preferences.Get("manager_role", false);
            ownerRadio.IsVisible = Preferences.Get("owner_role", false);
            tenantRadio.IsVisible = Preferences.Get("tenant_role", false);
            maintRadio.IsVisible = Preferences.Get("maintenance_role", false);
        }

        void clickedBack(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new OwnerSetup();
        }

        void roleSelected(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {

        }

        void clickedLogin(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new PropertySummary());
        }

    }
}
