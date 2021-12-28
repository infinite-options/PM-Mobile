using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using PM.Model;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PM.ViewModel
{
    public partial class EditPMProfile : ContentPage
    {
        public EditPMProfile()
        {
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;

            InitializeComponent();
        }

        async void clickedBack(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopAsync();
        }

        void showChargeOptions(System.Object sender, System.EventArgs e)
        {
            if (chargeOptionGrid.IsVisible == false) //show the options
            {
                chargeOptionGrid.IsVisible = true;
            }
            else
            {
                chargeOptionGrid.IsVisible = false;
            }
        }

        void clickedChargeOptions(System.Object sender, System.EventArgs e)
        {
            Button button1 = (Button)sender;

            if (button1.Text == "%")
            {
                chargeOptionGrid.IsVisible = false;
                percentGrid.IsVisible = true;
                dollarGrid.IsVisible = false;
                chargeTypeEntry.Text = "%";
            }
            else
            {
                chargeOptionGrid.IsVisible = false;
                percentGrid.IsVisible = false;
                dollarGrid.IsVisible = true;
                chargeTypeEntry.Text = "$";
            }
        }

        void showAddService(System.Object sender, System.EventArgs e)
        {
            addServiceStack.IsVisible = true;
            addService.IsVisible = false;
        }

        void hideAddService(System.Object sender, System.EventArgs e)
        {
            addServiceStack.IsVisible = false;
            addService.IsVisible = true;
        }
    }
}
