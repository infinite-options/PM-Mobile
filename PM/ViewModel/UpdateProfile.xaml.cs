using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using PM.Model;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace PM.ViewModel
{
    public partial class UpdateProfile : ContentPage
    {
        Address addr;
        ObservableCollection<idType> idTypes = new ObservableCollection<idType>();
        Dictionary<string, string> autofillInfoDict = new Dictionary<string, string>();

        public UpdateProfile(Dictionary<string,string> updateProfileInfoDict)
        {
            autofillInfoDict = updateProfileInfoDict;
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);

            InitializeComponent();
            autofillInfo();
            addr = new Address();

            idTypes.Add(new idType
            {
                type = "Social Security"
            });
            idTypes.Add(new idType
            {
                type = "Driver License"
            });
            idTypes.Add(new idType
            {
                type = "Passport"
            });
            idTypes.Add(new idType
            {
                type = "Real ID"
            });
            idList.ItemsSource = idTypes;
        }

        void autofillInfo()
        {
            try
            {
                if (autofillInfoDict.ContainsKey("first_name"))
                {
                    name.Text = autofillInfoDict["first_name"] + " " + autofillInfoDict["last_name"];
                    phoneEntry.Text = autofillInfoDict["phone_num"];
                    schoolAffilEntry.Text = autofillInfoDict["affiliation"];
                    idTypeButton.Text = autofillInfoDict["id_type"];
                    idNumEntry.Text = autofillInfoDict["id_number"];
                    //AddressEntry.Text = autofillInfoDict["address"];
                    if (autofillInfoDict["unit"] != "")
                        AptEntry.Text = autofillInfoDict["unit"];
                    CityEntry.Text = autofillInfoDict["city"];
                    StateEntry.Text = autofillInfoDict["state"];
                    ZipEntry.Text = autofillInfoDict["zip"];
                    AddressEntry.Text = autofillInfoDict["address"];
                }
            }
            catch
            {

            }

            AddressEntry.TextChanged += OnAddressChanged;
        }

        void idTypeClicked(System.Object sender, System.EventArgs e)
        {
            if (idGrid.IsVisible == true)
                idGrid.IsVisible = false;
            else idGrid.IsVisible = true;

            idTypeFrame.HeightRequest = 32;
            idNum.HeightRequest = 32;
        }

        void idListItemSelected(System.Object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            idGrid.IsVisible = false;
            idTypeButton.Text = ((idType)idList.SelectedItem).type;

            idTypeFrame.HeightRequest = 32;
            idNum.HeightRequest = 32;
        }

        //address autocomplete start
        // Auto-complete
        private ObservableCollection<AddressAutocomplete> _addresses;
        public ObservableCollection<AddressAutocomplete> Addresses
        {
            get => _addresses ?? (_addresses = new ObservableCollection<AddressAutocomplete>());
            set
            {
                if (_addresses != value)
                {
                    _addresses = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _addressText;
        public string AddressText
        {
            get => _addressText;
            set
            {
                if (_addressText != value)
                {
                    _addressText = value;
                    OnPropertyChanged();
                }
            }
        }

        private async void OnAddressChanged(object sender, TextChangedEventArgs eventArgs)
        {
            addressGrid.IsVisible = true;
            //these 2 lines commented out for testing
            //UnitCity.IsVisible = false;
            //StateZip.IsVisible = false;
            unit.BorderColor = Color.White;
            city.BorderColor = Color.White;
            state.BorderColor = Color.White;
            zipCode.BorderColor = Color.White;

            //UnitCityState.IsVisible = false;
            //ZipPhone.IsVisible = false;
            addressList.ItemsSource = await addr.GetPlacesPredictionsAsync(AddressEntry.Text);
            //addr.OnAddressChanged(addressList, Addresses, _addressText);
        }

        private void addressEntryFocused(object sender, EventArgs eventArgs)
        {
            //unit.BorderColor = Color.White;
            //city.BorderColor = Color.White;
            //state.BorderColor = Color.White;
            //zipCode.BorderColor = Color.White;
            //addr.addressEntryFocused(addressList, new Grid[] { UnitCityState, ZipPhone });
        }

        private void addressEntryUnfocused(object sender, EventArgs eventArgs)
        {
            unit.BorderColor = Color.FromHex("#707070");
            city.BorderColor = Color.FromHex("#707070");
            state.BorderColor = Color.FromHex("#707070");
            zipCode.BorderColor = Color.FromHex("#707070");
            addr.addressEntryUnfocused(addressList, new Grid[] { UnitCity, StateZip });
        }

        private void addressSelected(System.Object sender, System.EventArgs e)
        {
            addr.addressSelected(addressList, new Grid[] { UnitCity, StateZip }, AddressEntry, CityEntry, StateEntry, ZipEntry);
            addressGrid.IsVisible = false;
            //UnitCity.IsVisible = true;
            //StateZip.IsVisible = true;
            unit.BorderColor = Color.FromHex("#707070");
            city.BorderColor = Color.FromHex("#707070");
            state.BorderColor = Color.FromHex("#707070");
            zipCode.BorderColor = Color.FromHex("#707070");
        }
        //address autocomplete end

        //async void saveChanges(System.Object sender, System.EventArgs e)
        //{
        //    EditProfile editprof = new EditProfile();
        //    editprof.first_name = autofillInfoDict["first_name"];
        //    editprof.last_name = autofillInfoDict["last_name"];
        //    editprof.phone_num = phoneEntry.Text;
        //    editprof.email = autofillInfoDict["email"];
        //    editprof.id_type = idTypeButton.Text;
        //    editprof.id_number = idNumEntry.Text;
        //    editprof.address = AddressEntry.Text;
        //    if (AptEntry.Text == null)
        //        editprof.unit = "";
        //    else editprof.unit = AptEntry.Text;
        //    editprof.city = CityEntry.Text;
        //    editprof.state = StateEntry.Text;
        //    editprof.zip = ZipEntry.Text;
        //    editprof.uid = autofillInfoDict["cust_uid"];
        //    editprof.noti = autofillInfoDict["notification"];

        //    var editProfJSONString = JsonConvert.SerializeObject(editprof);
        //    // Console.WriteLine("newPaymentJSONString" + newPaymentJSONString);
        //    var editProfContent = new StringContent(editProfJSONString, Encoding.UTF8, "application/json");
        //    Console.WriteLine("edit profile Content: " + editProfContent);
        //    var client = new HttpClient();
        //    var response = client.PostAsync("https://c1zwsl05s5.execute-api.us-west-1.amazonaws.com/dev/api/v2/UpdateProfile", editProfContent);
        //    await DisplayAlert("Success", "Profile updated!", "OK");
        //    Console.WriteLine("RESPONSE TO UPDATEPROFILE   " + response.Result);
        //    Console.WriteLine("UPDATEPROFILE JSON OBJECT BEING SENT: " + editProfJSONString);
        //}

        //menu functions
        void profileClicked(System.Object sender, System.EventArgs e)
        {
            //Application.Current.MainPage = new NavigationPage(new UserProfile());
            Navigation.PushAsync(new UserProfile());
        }

        void menuClicked(System.Object sender, System.EventArgs e)
        {
            openMenuGrid.IsVisible = true;
            //whiteCover.IsVisible = true;
            menu.IsVisible = false;
        }

        void openedMenuClicked(System.Object sender, System.EventArgs e)
        {
            openMenuGrid.IsVisible = false;
            //whiteCover.IsVisible = false;
            menu.IsVisible = true;
        }

        void orderClicked(System.Object sender, System.EventArgs e)
        {
            //Application.Current.MainPage = new FoodBanksMap();
            Navigation.PushAsync(new Filter());
        }

        void browseClicked(System.Object sender, System.EventArgs e)
        {
            //Application.Current.MainPage = new FoodBanksMap();
            //Navigation.PushAsync(new FoodBanksMap());
        }

        void logoutClicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.Properties["platform"] = "GUEST";
            Application.Current.Properties.Remove("user_id");
            Application.Current.MainPage = new LoginPage();
        }
        //end of menu functions
    }
}
