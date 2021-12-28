using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using PM.Model;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PM.ViewModel
{
    public partial class EditOwnerProfile : ContentPage
    {
        public EditOwnerProfile()
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

        void checkMethod(System.Object sender, System.EventArgs e)
        {
            ImageButton button1 = (ImageButton)sender;

            if (button1.Source.ToString() == "File: blankBox.png") //the payment method has been selected
            {
                button1.Source = "checkedBox.png";
            }
            else
            {
                button1.Source = "blankBox.png";
                //appChosen.isChecked = false;
                //appChosen.checkboxImg = "blankBox.png";
            }
        }
    }
}
