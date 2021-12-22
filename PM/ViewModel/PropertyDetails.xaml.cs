using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using PM.Model;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PM.ViewModel
{
    public partial class PropertyDetails : ContentPage
    {
        public ObservableCollection<PropListDetails> totalDetails = new ObservableCollection<PropListDetails>();

        public PropertyDetails()
        {
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;

            InitializeComponent();

            propertiesImg.BackgroundColor = Color.FromHex("#007AFF");
            propertiesLabel.TextColor = Color.FromHex("#007AFF");

            fillDetails();
        }

        void fillDetails()
        {
            PropListDetails newProp = new PropListDetails();
            newProp.gridMargin = new Thickness(0, 7);
            newProp.monthlyColor = Color.FromHex("#E3441F"); //red
            newProp.monthlyVisible = false;
            newProp.monthlyText = "$300.00";
            newProp.titleFontSize = 18;
            newProp.titleText = "Management";
            newProp.ytdColor = Color.FromHex("#3DB727"); //green
            newProp.ytdText = "$3,600.00";
            newProp.ytdVisible = false;
            newProp.newVisible = true;

            totalDetails.Add(newProp);

            PropListDetails newProp2 = new PropListDetails();
            newProp2.gridMargin = new Thickness(0, 3);
            newProp2.monthlyColor = Color.FromHex("#E3441F"); //red
            newProp2.monthlyVisible = true;
            newProp2.monthlyText = "$200.00";
            newProp2.titleFontSize = 14;
            newProp2.titleText = "Painting";
            newProp2.ytdColor = Color.FromHex("#E3441F"); //green
            newProp2.ytdText = "$400.00";
            newProp2.ytdVisible = true;
            newProp2.newVisible = false;

            totalDetails.Add(newProp2);

            PropListDetails newProp3 = new PropListDetails();
            newProp3.gridMargin = new Thickness(0, 3);
            newProp3.monthlyColor = Color.FromHex("#E3441F"); //red
            newProp3.monthlyVisible = true;
            newProp3.monthlyText = "$3,000.00";
            newProp3.titleFontSize = 14;
            newProp3.titleText = "Plumbing check";
            newProp3.ytdColor = Color.FromHex("#E3441F"); //green
            newProp3.ytdText = "$100.99";
            newProp3.ytdVisible = true;
            newProp3.newVisible = false;

            totalDetails.Add(newProp3);
            totalDetails.Add(newProp);
            totalDetails.Add(newProp);

            propertyColl.ItemsSource = totalDetails;
            propertyColl.HeightRequest = (25 * 5) + 42; //add the margin space separately
        }

        void clickedCashFlow(System.Object sender, System.EventArgs e)
        {
            Debug.WriteLine("cash flow arrow img source: " + cashFlowArrow.Source.ToString());
            if (cashFlowArrow.Source.ToString() == "File: downArrow.png") //open the list
            {
                cashFlowArrow.Source = "upArrow.png";
            }
            else //close the list
            {
                cashFlowArrow.Source = "downArrow.png";
            } 
        }

        void clickedExpenses(System.Object sender, System.EventArgs e)
        {
            Debug.WriteLine("expenses arrow img source: " + expensesArrow.Source.ToString());
            if (expensesArrow.Source.ToString() == "File: downArrow.png") //open the list
            {
                Debug.WriteLine("turn to up arrow");
                expensesArrow.Source = "upArrow.png";
                expenseStack.IsVisible = true;
            }
            else //close the list
            {
                Debug.WriteLine("turn to down arrow");
                expensesArrow.Source = "downArrow.png";
                expenseStack.IsVisible = false;
            }
        }

        void clickedTaxes(System.Object sender, System.EventArgs e)
        {
            Debug.WriteLine("taxes arrow img source: " + taxesArrow.Source.ToString());
            if (taxesArrow.Source.ToString() == "File: downArrow.png") //open the list
            {
                taxesArrow.Source = "upArrow.png";
            }
            else //close the list
            {
                taxesArrow.Source = "downArrow.png";
            }
        }

        void clickedMortgage(System.Object sender, System.EventArgs e)
        {
            Debug.WriteLine("mortgage arrow img source: " + mortgageArrow.Source.ToString());
            if (mortgageArrow.Source.ToString() == "File: downArrow.png") //open the list
            {
                mortgageArrow.Source = "upArrow.png";
            }
            else //close the list
            {
                mortgageArrow.Source = "downArrow.png";
            }
        }

        void clickedDetails(System.Object sender, System.EventArgs e)
        {
            Debug.WriteLine("details arrow img source: " + detailsArrow.Source.ToString());
            if (detailsArrow.Source.ToString() == "File: downArrow.png") //open the list
            {
                detailsArrow.Source = "upArrow.png";
            }
            else //close the list
            {
                detailsArrow.Source = "downArrow.png";
            }
        }

        async void clickedNewExpense(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new AddExpense("Expense"));
        }

        async void clickedNewTax(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new AddExpense("Tax"));
        }

        async void clickedNewMortgage(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new AddExpense("Mortgage"));
        }

        async void clickedNewRevenue(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new AddExpense("Revenue"));
        }

        async void clickedBack(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
