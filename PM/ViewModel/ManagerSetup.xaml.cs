using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PM.ViewModel
{
    public partial class ManagerSetup : ContentPage
    {
        public ManagerSetup()
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
            if (Preferences.Get("owner_role", false) == true)
                Application.Current.MainPage = new OwnerSetup();
            else if (Preferences.Get("tenant_role", false) == true)
                Application.Current.MainPage = new TenantSetup();
            else if (Preferences.Get("maintenance_role", false) == true)
                Application.Current.MainPage = new MaintenanceSetup();
            else Application.Current.MainPage = new SignUpFinish();
        }
    }
}
