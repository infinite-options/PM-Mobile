using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using PM.Model;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PM.ViewModel
{
    public partial class OwnerProfile : ContentPage
    {
        public OwnerProfile()
        {
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;

            InitializeComponent();

            paypalBox.Source = "checkedBox.png";
            appleBox.Source = "checkedBox.png";
            zelleBox.Source = "checkedBox.png";
        }

        async void clickedBack(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopAsync();
        }

        async void clickedEdit(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new EditOwnerProfile());
        }
    }

}
