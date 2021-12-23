using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using PM.Model;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PM.ViewModel
{
    public partial class PropertySummary : ContentPage
    {
        public ObservableCollection<PropSummary> totalProps = new ObservableCollection<PropSummary>();

        public PropertySummary()
        {
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;

            InitializeComponent();
            propertiesImg.BackgroundColor = Color.FromHex("#007AFF");
            propertiesLabel.TextColor = Color.FromHex("#007AFF");

            fillPropertyColl();
        }

        void fillPropertyColl()
        {
            PropSummary newProp = new PropSummary();
            newProp.propImg = "";
            newProp.income = "$2,000 / mo";
            newProp.address = "213 Parkland Ave, San Jose, CA 90820";
            newProp.statusImg = "rentedButton.png";
            newProp.pmName = "Manager: Jane Doe";

            totalProps.Add(newProp);

            PropSummary newProp2 = new PropSummary();
            newProp2.propImg = "";
            newProp2.income = "$2,000 / mo";
            newProp2.address = "213 Parkland Ave, San Jose, CA 90820";
            newProp2.statusImg = "rentedButton.png";
            newProp2.pmName = "Manager: Jane Doe";

            totalProps.Add(newProp2);

            PropSummary newProp3 = new PropSummary();
            newProp3.propImg = "";
            newProp3.income = "$2,000 / mo";
            newProp3.address = "213 Parkland Ave, San Jose, CA 90820";
            newProp3.statusImg = "rentedButton.png";
            newProp3.pmName = "Manager: Jane Doe";

            totalProps.Add(newProp3);

            PropSummary newProp4 = new PropSummary();
            newProp4.propImg = "";
            newProp4.income = "$2,000 / mo";
            newProp4.address = "213 Parkland Ave, San Jose, CA 90820";
            newProp4.statusImg = "rentedButton.png";
            newProp4.pmName = "Manager: Jane Doe";

            totalProps.Add(newProp4);

            PropSummary newProp5 = new PropSummary();
            newProp5.propImg = "";
            newProp5.income = "$2,000 / mo";
            newProp5.address = "213 Parkland Ave, San Jose, CA 90820";
            newProp5.statusImg = "rentedButton.png";
            newProp5.pmName = "Manager: Jane Doe";

            totalProps.Add(newProp5);
            totalProps.Add(newProp5);
            totalProps.Add(newProp5);

            propertyColl.ItemsSource = totalProps;
            propertyColl.HeightRequest = 130 * 5;

        }

        async void clickedProperty(System.Object sender, System.EventArgs e)
        {
            Button button1 = (Button)sender;
            PropSummary propChosen = button1.BindingContext as PropSummary;

            Debug.WriteLine("property selected: " + propChosen.address);

            await Navigation.PushAsync(new PropertyDetails());
        }
    }
}
