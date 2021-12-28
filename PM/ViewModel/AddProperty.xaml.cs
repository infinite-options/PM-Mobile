using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using PM.Model;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PM.ViewModel
{
    public partial class AddProperty : ContentPage
    {
        public ObservableCollection<Appliance> totalAppliances = new ObservableCollection<Appliance>();


        public AddProperty()
        {
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;

            InitializeComponent();

            fillApplianceColl();
        }

        void fillApplianceColl()
        {

            Appliance newApp = new Appliance();
            newApp.checkboxImg = "blankBox.png";
            newApp.appName = "Microwave";
            newApp.isChecked = false;

            totalAppliances.Add(newApp);

            Appliance newApp2 = new Appliance();
            newApp2.checkboxImg = "blankBox.png";
            newApp2.appName = "Dishwasher";
            newApp2.isChecked = false;

            totalAppliances.Add(newApp2);

            Appliance newApp3 = new Appliance();
            newApp3.checkboxImg = "blankBox.png";
            newApp3.appName = "Refrigerator";
            newApp3.isChecked = false;

            totalAppliances.Add(newApp3);

            Appliance newApp4 = new Appliance();
            newApp4.checkboxImg = "blankBox.png";
            newApp4.appName = "Washer";
            newApp4.isChecked = false;

            totalAppliances.Add(newApp4);

            Appliance newApp5 = new Appliance();
            newApp5.checkboxImg = "blankBox.png";
            newApp5.appName = "Dryer";
            newApp5.isChecked = false;

            totalAppliances.Add(newApp5);

            Appliance newApp6 = new Appliance();
            newApp6.checkboxImg = "blankBox.png";
            newApp6.appName = "Range";
            newApp6.isChecked = false;

            totalAppliances.Add(newApp6);

            applianceColl.ItemsSource = totalAppliances;
            applianceColl.HeightRequest = 33 * 6;
        }

        void checkAppliance(System.Object sender, System.EventArgs e)
        {
            ImageButton button1 = (ImageButton)sender;
            Appliance appChosen = button1.BindingContext as Appliance;

            if (appChosen.isChecked == false) //the appliance has been selected
            {
                appChosen.isChecked = true;
                appChosen.checkboxImg = "checkedBox.png";
            }
            else
            {
                appChosen.isChecked = false;
                appChosen.checkboxImg = "blankBox.png";
            }
        }

        void checkUtility(System.Object sender, System.EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            Debug.WriteLine("radiobutton group: " + rb.GroupName.ToString());
            Debug.WriteLine("radiobutton value: " + rb.Value.ToString());


        }

        async void clickedBack(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
