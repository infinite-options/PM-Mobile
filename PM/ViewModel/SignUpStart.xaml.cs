using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PM.ViewModel
{
    public partial class SignUpStart : ContentPage
    {
        public SignUpStart()
        {
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;

            InitializeComponent();

            Preferences.Set("manager_role", false);
            Preferences.Set("owner_role", false);
            Preferences.Set("tenant_role", false);
            Preferences.Set("maintenance_role", false);
        }

        void clickedBack(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        void checkCheckbox(System.Object sender, System.EventArgs e)
        {
            ImageButton img = (ImageButton)sender;
            if (img.Source.ToString() == "File: blankBox.png") //check the item
            {
                img.Source = "checkedBox.png";
                if (img.AnchorX == 1)
                    Preferences.Set("manager_role", true);
                else if (img.AnchorX == 2)
                    Preferences.Set("owner_role", true);
                else if (img.AnchorX == 3)
                    Preferences.Set("tenant_role", true);
                else if (img.AnchorX == 4)
                    Preferences.Set("maintenance_role", true);
            }
            else
            {
                img.Source = "blankBox.png";
                if (img.AnchorX == 1)
                    Preferences.Set("manager_role", false);
                else if (img.AnchorX == 2)
                    Preferences.Set("owner_role", false);
                else if (img.AnchorX == 3)
                    Preferences.Set("tenant_role", false);
                else if (img.AnchorX == 4)
                    Preferences.Set("maintenance_role", false);
            }

        }

        void clickedNext(System.Object sender, System.EventArgs e)
        {
            roleSelectionGrid.IsVisible = false;
            starterInfoGrid.IsVisible = true;
        }

        void showRoleSelection(System.Object sender, System.EventArgs e)
        {
            roleSelectionGrid.IsVisible = true;
            starterInfoGrid.IsVisible = false;
        }

        void showStarterInfo(System.Object sender, System.EventArgs e)
        {
            starterInfoGrid.IsVisible = true;
            loginMethodGrid.IsVisible = false;
        }

        void clickedProceed(System.Object sender, System.EventArgs e)
        {
            starterInfoGrid.IsVisible = false;
            loginMethodGrid.IsVisible = true;
        }

        void loginClicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new LoginPage();
        }

        void continueEmail(System.Object sender, System.EventArgs e)
        {
            innerStack.IsVisible = false;
            emailStack.IsVisible = true;
        }

        void clickedSetupProfiles(System.Object sender, System.EventArgs e)
        {
            if (Preferences.Get("manager_role", false) == true)
                Application.Current.MainPage = new ManagerSetup();
            else if (Preferences.Get("owner_role", false) == true)
                Application.Current.MainPage = new OwnerSetup();
            else if (Preferences.Get("tenant_role", false) == true)
                Application.Current.MainPage = new TenantSetup();
            else if (Preferences.Get("maintenance_role", false) == true)
                Application.Current.MainPage = new MaintenanceSetup();

            //Application.Current.MainPage = new OwnerSetup();
        }
    }
}
