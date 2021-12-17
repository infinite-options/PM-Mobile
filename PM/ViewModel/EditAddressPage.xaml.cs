using System;
using System.Collections.Generic;
using PM.Model;
using Xamarin.Forms;

namespace PM.ViewModel
{
    public partial class EditAddressPage : ContentPage
    {
        public Dictionary<StoreItem, int> itemAmounts = new Dictionary<StoreItem, int>();
        private Address addr = new Address();
        public static AddressAutocomplete addressToValidate = null;
        FoodBanks chosenFb;

        public EditAddressPage(FoodBanks foodbank, Dictionary<StoreItem, int> itmAmts)
        {
            chosenFb = foodbank;
            itemAmounts = itmAmts;
            InitializeComponent();
            BackgroundColor = Color.FromHex("AB000000");
        }

        void CloseModalNavigateToCheckoutPage(System.Object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync(false);
        }

        async void signUpAddress1Entry_TextChanged(System.Object sender, EventArgs eventArgs)
        {
            if (!String.IsNullOrEmpty(signUpAddress1Entry.Text))
            {
                if (addressToValidate != null)
                {
                    if (addressToValidate.Street != signUpAddress1Entry.Text)
                    {
                        SignUpAddressList.ItemsSource = await addr.GetPlacesPredictionsAsync(signUpAddress1Entry.Text);
                        signUpAddress1Entry_Focused(sender, eventArgs);
                    }
                }
                else
                {
                    SignUpAddressList.ItemsSource = await addr.GetPlacesPredictionsAsync(signUpAddress1Entry.Text);
                    signUpAddress1Entry_Focused(sender, eventArgs);
                }
            }
            else
            {
                signUpAddress1Entry_Unfocused(sender, eventArgs);
                addressToValidate = null;
            }
        }

        void signUpAddress1Entry_Focused(System.Object sender, EventArgs eventArgs)
        {
            if (!String.IsNullOrEmpty(signUpAddress1Entry.Text))
            {
                addr.addressEntryFocused(SignUpAddressList, signUpAddressFrame);
            }
        }

        void signUpAddress1Entry_Unfocused(System.Object sender, EventArgs eventArgs)
        {
            addr.addressEntryUnfocused(SignUpAddressList, signUpAddressFrame);
        }

        async void SignUpAddressList_ItemSelected(System.Object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            signUpAddress1Entry.TextChanged -= signUpAddress1Entry_TextChanged;
            addressToValidate = addr.addressSelected(SignUpAddressList, signUpAddress1Entry, signUpAddressFrame);
            var zipcode = await addr.getZipcode(addressToValidate.PredictionID);
            if (zipcode != null)
            {
                addressToValidate.ZipCode = zipcode;
            }
            addr.addressSelectedFillEntries(addressToValidate, signUpAddress1Entry, signUpAddress2Entry, signUpCityEntry, signUpStateEntry, signUpZipcodeEntry);
            addr.addressEntryUnfocused(SignUpAddressList, signUpAddressFrame);
            signUpAddress1Entry.TextChanged += signUpAddress1Entry_TextChanged;

        }

        async void SaveAddress(System.Object sender, System.EventArgs e)
        {
            if (addressToValidate != null)
            {
                var client = new AddressValidation();
                var message = client.ValidateAddressString(addressToValidate.Street, addressToValidate.Unit == null ? "" : addressToValidate.Unit, addressToValidate.City, addressToValidate.State, addressToValidate.ZipCode);

                await DisplayAlert("USPS Code", message, "OK");

                if (message == null)
                {
                    await DisplayAlert("Invalid Address", "The address you entered couldn't be confirmed. Please enter another one.", "OK");
                    return;
                }
                else if (message == "D")
                {
                    await DisplayAlert("Missing Info", "Please enter your unit/apartment number into the appropriate field.", "OK");
                    return;
                }

                addressToValidate.isValidated = true;

                var storePage = Application.Current.MainPage.Navigation.NavigationStack[0];
                var cartPage = Application.Current.MainPage.Navigation.NavigationStack[1];
                var updatedNavigationPage = new NavigationPage(storePage);

                await updatedNavigationPage.PushAsync(cartPage, false);
                await updatedNavigationPage.PushAsync(new CheckoutPage(chosenFb, itemAmounts), false);

                Application.Current.MainPage = updatedNavigationPage;
            }
            else
            {
                await DisplayAlert("Oops", "Address is empty", "OK");
            }
        }
    }
}
