using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PM.ViewModel
{
    public partial class AddExpense : ContentPage
    {
        string newItemType = "";

        public AddExpense(string item)
        {
            newItemType = item;
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;

            InitializeComponent();

            propertiesImg.BackgroundColor = Color.FromHex("#007AFF");
            propertiesLabel.TextColor = Color.FromHex("#007AFF");

            if (item == "Tax")
                addNewLabel.Text = "Add New Tax Payment";
            else if (item == "Mortgage")
                addNewLabel.Text = "Add New Mortgage Payment";
            else addNewLabel.Text = "Add New " + item;
            saveButton.Text = "Save " + item;
            if (item == "Tax")
                saveButton.WidthRequest = 90;
        }

        void clickedCategory(System.Object sender, System.EventArgs e)
        {
            Debug.WriteLine("clicked category");
        }

        async void clickedBack(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
