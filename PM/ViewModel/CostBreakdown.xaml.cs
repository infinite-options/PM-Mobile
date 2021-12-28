using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PM.Model;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PM.ViewModel
{
    public partial class CostBreakdown : ContentPage
    {
        public ObservableCollection<CostDetails> totalDetails = new ObservableCollection<CostDetails>();

        public CostBreakdown()
        {
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;

            InitializeComponent();

            fillBreakdowns();
        }

        void fillBreakdowns()
        {
            CostDetails newBreakdown = new CostDetails();
            newBreakdown.isTitleVisible = true;
            newBreakdown.titleText = "December 2021";
            newBreakdown.titleAmt = "$25.00";
            newBreakdown.headingText = "Paint - kitchen wall";
            newBreakdown.bodyText = "Wall was scraped - needed fresh coat of paint. Requested by new tenants";
            newBreakdown.dateText = "Dec 12, 2021";
            newBreakdown.amtText = "$25.00";

            totalDetails.Add(newBreakdown);

            CostDetails newBreakdown2 = new CostDetails();
            newBreakdown2.isTitleVisible = true;
            newBreakdown2.titleText = "November 2021";
            newBreakdown2.titleAmt = "$50.00";
            newBreakdown2.headingText = "Paint - living room";
            newBreakdown2.bodyText = "Wall was scraped - needed fresh coat of paint. Requested by new tenants";
            newBreakdown2.dateText = "Nov 12, 2021";
            newBreakdown2.amtText = "$25.00";

            totalDetails.Add(newBreakdown2);

            CostDetails newBreakdown3 = new CostDetails();
            newBreakdown3.isTitleVisible = false;
            newBreakdown3.headingText = "Wallpaper";
            newBreakdown3.bodyText = "Wall was scraped - needed fresh coat of paint. Requested by new tenants";
            newBreakdown3.dateText = "Nov 06, 2021";
            newBreakdown3.amtText = "$25.00";

            totalDetails.Add(newBreakdown3);

            breakdownColl.ItemsSource = totalDetails;
            breakdownColl.HeightRequest = (3 * 130) + (2 * 50); //add the box size vs the extra title size if applicable
        }

        async void clickedBack(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
