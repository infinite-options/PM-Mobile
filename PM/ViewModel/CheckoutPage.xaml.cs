using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;
using PM.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using static PM.ViewModel.EditAddressPage;
using static PM.ViewModel.FoodBackStore;

namespace PM.ViewModel
{
    
    public partial class CheckoutPage : ContentPage
    {
        public ObservableCollection<StoreItem> itemsSource = new ObservableCollection<StoreItem>();
        public Dictionary<StoreItem, int> itemAmounts = new Dictionary<StoreItem, int>();
        CheckoutPost checkoutobj = new CheckoutPost();
        FoodBanks chosenFb;
        string chosenTime, custAddress, custUnit, custCity, custState, custZip, custLat, custLong;
        string bankLat, bankLong;
        List<Date1> availableDates;
        DateTime todaysDate;
        string optionChosen;


        public CheckoutPage(FoodBanks foodbank, Dictionary<StoreItem, int> itmAmts)
        {
            optionChosen = "delivery";
            availableDates = new List<Date1>();
            chosenFb = foodbank;
            getBankLatLong();
            itemAmounts = itmAmts;
            InitializeComponent();
            getDates();
            getInfo();
            SetFoodBank(Preferences.Get("chosenBankName", ""), totalQuantity.ToString(), Preferences.Get("chosenBankImg", ""));
            SetCartItems();
            //SetPersonalInfo("Carlos", "Torres", "4158329643");
            //SetFullAddress("1658 Sacramento Street", "San Francisco", "CA", "94109");
            chosenTime = "10:00 AM - 12:00 PM";
            SetFullDeliveryInfo((string)Application.Current.Properties["date_chosen"], "10:00 AM - 12:00 PM");
        }

        void getDates()
        {
            //DateTime todayDate = DateTime.Now;
            //Debug.WriteLine("todays date: " + todayDate.Month + " " + todayDate.Day + " " + todayDate.DayOfWeek);
            for (int i = 0; i < 15; i++)
            {
                todaysDate = DateTime.Now;
                DateTime nextDate = todaysDate.AddDays(i);
                Debug.WriteLine("todays date: " + nextDate.ToLongDateString() + " endOfLongDate " + nextDate.Date);
                Date1 addingDate = new Date1();
                addingDate.BackgroundImg = "dateUnselected.png";
                addingDate.dotw = nextDate.DayOfWeek.ToString().Substring(0, 1);
                addingDate.day = nextDate.Day.ToString();
                addingDate.month = nextDate.ToLongDateString().Substring(nextDate.ToLongDateString().IndexOf(",") + 2, 3);
                addingDate.TextColor = Color.Black;
                addingDate.dateObj = nextDate;
                if ((nextDate.DayOfWeek.ToString() == "Monday" && chosenFb.mondayHours != "Closed") ||
                    (nextDate.DayOfWeek.ToString() == "Tuesday" && chosenFb.tuesdayHours != "Closed") ||
                    (nextDate.DayOfWeek.ToString() == "Wednesday" && chosenFb.wednesdayHours != "Closed") ||
                    (nextDate.DayOfWeek.ToString() == "Thursday" && chosenFb.thursdayHours != "Closed") ||
                    (nextDate.DayOfWeek.ToString() == "Friday" && chosenFb.fridayHours != "Closed") ||
                    (nextDate.DayOfWeek.ToString() == "Saturday" && chosenFb.saturdayHours != "Closed") ||
                    (nextDate.DayOfWeek.ToString() == "Sunday" && chosenFb.sundayHours != "Closed"))
                    availableDates.Add(addingDate);
                else continue;
            }

            dateCarousel.ItemsSource = availableDates;
        }

        void clickedDateModal(System.Object sender, System.EventArgs e)
        {
            dateModalFrame.IsVisible = true;

        }

        void dateChange(System.Object sender, System.EventArgs e)
        {
            Button button1 = (Button)sender;
            Date1 dateChosen = button1.BindingContext as Date1;
            dateModalFrame.IsVisible = false;
            SetFullDeliveryInfo(dateChosen.dateObj.ToLongDateString(), chosenTime);
        }

        public string GetXMLElement(XElement element, string name)
        {
            var el = element.Element(name);
            if (el != null)
            {
                return el.Value;
            }
            return "";
        }

        public string GetXMLAttribute(XElement element, string name)
        {
            var el = element.Attribute(name);
            if (el != null)
            {
                return el.Value;
            }
            return "";
        }

        async void getBankLatLong()
        {
            try
            {

                // Setting request for USPS API

                Debug.WriteLine("INPUTS: ADDRESS1: {0}, ADDRESS2: {1}, CITY: {2}, STATE: {3}, ZIPCODE: {4}", chosenFb.address, chosenFb.unit, chosenFb.city, chosenFb.state, chosenFb.zip);
                XDocument requestDoc = new XDocument(
                    new XElement("AddressValidateRequest",
                    new XAttribute("USERID", "400INFIN1745"),
                    new XElement("Revision", "1"),
                    new XElement("Address",
                    new XAttribute("ID", "0"),
                    new XElement("Address1", chosenFb.address),
                    new XElement("Address2", chosenFb.unit != null ? chosenFb.unit : ""),
                    new XElement("City", chosenFb.city),
                    new XElement("State", chosenFb.state),
                    new XElement("Zip5", chosenFb.zip),
                    new XElement("Zip4", "")
                         )
                     )
                 );

                // This endpoint needs to change
                var Addurl = "https://production.shippingapis.com/ShippingAPI.dll?API=Verify&XML=" + requestDoc;
                var Addclient = new WebClient();
                var Addresponse = Addclient.DownloadString(Addurl);
                var xdoc = XDocument.Parse(Addresponse.ToString());
                Debug.WriteLine("RESULT FROM USPS: " + xdoc);
                foreach (XElement element in xdoc.Descendants("Address"))
                {
                    if (GetXMLElement(element, "Error").Equals(""))
                    {
                        if ((GetXMLElement(element, "DPVConfirmation").Equals("Y") || GetXMLElement(element, "DPVConfirmation").Equals("S") || GetXMLElement(element, "DPVConfirmation").Equals("D")) && GetXMLElement(element, "Zip5").Equals(chosenFb.zip) && GetXMLElement(element, "City").Equals(chosenFb.city.ToUpper()))
                        {
                            Geocoder geoCoder = new Geocoder();

                            IEnumerable<Position> approximateLocations = await geoCoder.GetPositionsForAddressAsync(chosenFb.address + "," + chosenFb.city + "," + chosenFb.state);
                            Position position = approximateLocations.FirstOrDefault();

                            string bankLatitude = $"{position.Latitude}";
                            string bankLongitude = $"{position.Longitude}";
                            bankLat = $"{position.Latitude}";
                            bankLong = $"{position.Longitude}";
                            Debug.Write("bank latitude: " + bankLatitude);
                            Debug.Write("bank longitude: " + bankLongitude);

                        }
                        else
                        {
                            bankLat = "0";
                            bankLong = "0";
                        }
                    }
                    else
                    {
                        bankLat = "0";
                        bankLong = "0";
                    }
                }
            }
            catch
            {
                bankLat = "0";
                bankLong = "0";
            }
        }

        

        async void getInfo()
        {
            var request = new HttpRequestMessage();
            Console.WriteLine("user_id: " + (string)Application.Current.Properties["user_id"]);
            string url = "https://c1zwsl05s5.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
            Debug.WriteLine("profile endpoint: " + url);
            request.RequestUri = new Uri(url);
            request.Method = HttpMethod.Get;
            var client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                HttpContent content = response.Content;
                Debug.WriteLine("content: " + content);
                var userString = await content.ReadAsStringAsync();
                Debug.WriteLine("userString: " + userString);
                var info_obj = JObject.Parse(userString);

                //pass the displayed info into the checkout obj
                checkoutobj.delivery_first_name = (info_obj["result"])[0]["customer_first_name"].ToString();
                checkoutobj.delivery_last_name = (info_obj["result"])[0]["customer_last_name"].ToString();
                checkoutobj.delivery_phone_num = (info_obj["result"])[0]["customer_phone_num"].ToString();
                checkoutobj.delivery_email = (info_obj["result"])[0]["customer_email"].ToString();
                checkoutobj.delivery_address = (info_obj["result"])[0]["customer_address"].ToString();
                custAddress = (info_obj["result"])[0]["customer_address"].ToString();
                checkoutobj.delivery_unit = (info_obj["result"])[0]["customer_unit"].ToString();
                custUnit = (info_obj["result"])[0]["customer_unit"].ToString();
                checkoutobj.delivery_city = (info_obj["result"])[0]["customer_city"].ToString();
                custCity = (info_obj["result"])[0]["customer_city"].ToString();
                checkoutobj.delivery_state = (info_obj["result"])[0]["customer_state"].ToString();
                custState = (info_obj["result"])[0]["customer_state"].ToString();
                checkoutobj.delivery_zip = (info_obj["result"])[0]["customer_zip"].ToString();
                custZip = (info_obj["result"])[0]["customer_zip"].ToString();
                checkoutobj.delivery_latitude = (info_obj["result"])[0]["customer_lat"].ToString();
                custLat = (info_obj["result"])[0]["customer_lat"].ToString();
                checkoutobj.delivery_longitude = (info_obj["result"])[0]["customer_long"].ToString();
                custLong = (info_obj["result"])[0]["customer_long"].ToString();

                SetPersonalInfo((info_obj["result"])[0]["customer_first_name"].ToString(), (info_obj["result"])[0]["customer_last_name"].ToString(), (info_obj["result"])[0]["customer_phone_num"].ToString());
                SetFullAddress((info_obj["result"])[0]["customer_address"].ToString(), (info_obj["result"])[0]["customer_city"].ToString(),
                    (info_obj["result"])[0]["customer_state"].ToString(), (info_obj["result"])[0]["customer_zip"].ToString());
            }
        }

        

        void SetFoodBank(string name, string totalQuantity, string picture)
        {
            foodBankName.Text = name;
            totalCartItems.Text = totalQuantity + " Items";
            foodBankPicture.Source = picture;
        }

        void SetPersonalInfo(string firstName, string lastName, string phone)
        {
            userName.Text = firstName + " " + lastName;
            userPhone.Text = phone;
        }

        void SetFullAddress(string address, string city, string state, string zipcode)
        {
            if (addressToValidate != null && addressToValidate.isValidated)
            {
                userAddress.Text = addressToValidate.Street;
                userCityStateZipcode.Text = addressToValidate.City + ", " + addressToValidate.State + " " + addressToValidate.ZipCode;
            }
            else
            {
                userAddress.Text = address;
                userCityStateZipcode.Text = city + ", " + state + " " + zipcode;
            }
        }

        void SetFullDeliveryInfo(string date, string time)
        {
            deliveryDate.Text = date;
            deliveryTime.Text = time;
        }

        void SetCartItems()
        {
            itemsSource.Clear();

            foreach (string itemName in cart.Keys)
            {
                itemsSource.Add(cart[itemName]);
            }

            itemList.ItemsSource = itemsSource;

            float size = itemsSource.Count;
            float rows = 0;
            float d = 3;

            if (size != 0)
            {
                if (size > 0 && size <= 3)
                {
                    rows = 1;
                }
                else
                {
                    rows = size / d;
                }
            }

            double height = (double)(Math.Ceiling(rows) * 155);

            itemList.HeightRequest = height;
        }

        void NavigateToCartPage(System.Object sender, System.EventArgs e)
        {
            Navigation.PopAsync(false);
        }

        async void NavigateToConfirmationPage(System.Object sender, System.EventArgs e)
        {
            //CheckoutPost checkoutobj = new CheckoutPost();
            Item[] itmList = new Item[itemAmounts.Count];
            int index = 0;
            double price = 0;
            foreach(var item in itemAmounts.Keys)
            {
                Item item1 = new Item();
                item1.img = item.image;
                item1.qty = item.quantity;
                item1.name = item.name;
                item1.unit = item.unit;
                item1.price = item.price;
                item1.item_uid = item.item_uid;
                item1.itm_business_uid = item.itm_business_uid;
                itmList[index] = item1;
                price += item.quantity * item.price;
                index++;
            }
            checkoutobj.pur_customer_uid = Application.Current.Properties["user_id"].ToString();
            checkoutobj.pur_business_uid = Application.Current.Properties["chosen_business_uid"].ToString();
            checkoutobj.items = itmList;
            checkoutobj.order_instructions = "";
            checkoutobj.delivery_instructions = "deliv instructions";
            checkoutobj.order_type = "food";
            checkoutobj.purchase_notes = "purch notes";
            checkoutobj.start_delivery_date = deliveryDate.Text;
            checkoutobj.pay_coupon_id = "";
            //checkoutobj.amount_due = price.ToString();
            checkoutobj.amount_due = "0";
            checkoutobj.amount_discount = "0";
            checkoutobj.amount_paid = "0";
            checkoutobj.info_is_Addon = "FALSE";
            checkoutobj.cc_num = "4242424242424242";
            checkoutobj.cc_exp_date = "2028-07-01 00:00:00";
            checkoutobj.cc_cvv = "222";
            checkoutobj.cc_zip = "95132";
            checkoutobj.charge_id = "";
            checkoutobj.payment_type = "STRIPE";
            checkoutobj.delivery_status = "FALSE";
            checkoutobj.subtotal = "0";
            checkoutobj.service_fee = "0";
            checkoutobj.delivery_fee = "0";
            checkoutobj.driver_tip = "0";
            checkoutobj.taxes = "0";
            checkoutobj.ambassador_code = "0";

            var getItemsSerializedObject = JsonConvert.SerializeObject(checkoutobj);
            var content = new StringContent(getItemsSerializedObject, Encoding.UTF8, "application/json");

            System.Diagnostics.Debug.WriteLine("checkout obj: " + getItemsSerializedObject);

            var getItemsClient = new HttpClient();
            var RDSResponse = await getItemsClient.PostAsync("https://c1zwsl05s5.execute-api.us-west-1.amazonaws.com/dev/api/v2/checkout_SN", content);
            var RDSMessage = await RDSResponse.Content.ReadAsStringAsync();
            Debug.WriteLine("RDSResponse from checkout endpoint: " + RDSResponse.ToString());
            Debug.WriteLine("RDSMessage from checkout endpoint: " + RDSMessage.ToString());

            await Navigation.PushAsync(new OrderConfirmationPage(optionChosen, deliveryDate.Text, deliveryTime.Text, userAddress.Text + ", " + userCityStateZipcode.Text), false);
        }

        void NavigateToEditAddressPage(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage.Navigation.PushModalAsync(new EditAddressPage(chosenFb, itemAmounts), false);
        }

        void backClicked(System.Object sender, System.EventArgs e)
        {
            //Application.Current.MainPage = new MainPage();
            Navigation.PopAsync();
        }

        //menu functions
        void profileClicked(System.Object sender, System.EventArgs e)
        {
            //Application.Current.MainPage = new NavigationPage(new UserProfile());
            Navigation.PushAsync(new UserProfile());
        }

        void menuClicked(System.Object sender, System.EventArgs e)
        {
            openMenuGrid.IsVisible = true;
            menu.IsVisible = false;
        }

        void openedMenuClicked(System.Object sender, System.EventArgs e)
        {
            openMenuGrid.IsVisible = false;
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

        void loginClicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.Properties["platform"] = "GUEST";
            Application.Current.Properties.Remove("user_id");
            Application.Current.MainPage = new LoginPage();
        }


        //end of menu functions

        void delivPickupSwitch_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            //pickup chosen
            if (e.Value == true)
            {
                optionChosen = "pickup";
                deliveryPickupHeader.Text = "Pickup Information";
                pickupText.TextColor = Color.FromHex("#E7404A");
                delivText.TextColor = Color.Black;
                SetFullAddress(chosenFb.address, chosenFb.city, chosenFb.state, chosenFb.zip);

                checkoutobj.delivery_address = chosenFb.address;
                checkoutobj.delivery_unit = chosenFb.unit;
                checkoutobj.delivery_city = chosenFb.city;
                checkoutobj.delivery_state = chosenFb.state;
                checkoutobj.delivery_zip = chosenFb.zip;
                checkoutobj.delivery_latitude = bankLat;
                checkoutobj.delivery_longitude = bankLong;
            }
            else //delivery chosen if unselected
            {
                optionChosen = "delivery";
                deliveryPickupHeader.Text = "Delivery Information";
                delivText.TextColor = Color.FromHex("#E7404A");
                pickupText.TextColor = Color.Black;
                SetFullAddress(custAddress, custCity, custState, custZip);

                checkoutobj.delivery_address = custAddress;
                checkoutobj.delivery_unit = "";
                checkoutobj.delivery_city = custCity;
                checkoutobj.delivery_state = custState;
                checkoutobj.delivery_zip = custZip;
                checkoutobj.delivery_latitude = custLat;
                checkoutobj.delivery_longitude = custLong;
            }
        }

        //void clickedPickup(System.Object sender, System.EventArgs e)
        //{
        //    if (switchButton.Text == "Pickup")
        //    {
        //        optionChosen = "pickup";
        //        deliveryPickupHeader.Text = "Pickup Information";
        //        SetFullAddress(chosenFb.address, chosenFb.city, chosenFb.state, chosenFb.zip);
        //        switchButton.Text = "Delivery";

        //        checkoutobj.delivery_address = chosenFb.address;
        //        checkoutobj.delivery_unit = chosenFb.unit;
        //        checkoutobj.delivery_city = chosenFb.city;
        //        checkoutobj.delivery_state = chosenFb.state;
        //        checkoutobj.delivery_zip = chosenFb.zip;
        //        checkoutobj.delivery_latitude = bankLat;
        //        checkoutobj.delivery_longitude = bankLong;
        //    }
        //    //text == Delivery
        //    else
        //    {
        //        optionChosen = "delivery";
        //        deliveryPickupHeader.Text = "Delivery Information";
        //        SetFullAddress(custAddress, custCity, custState, custZip);
        //        switchButton.Text = "Pickup";

        //        checkoutobj.delivery_address = custAddress;
        //        checkoutobj.delivery_unit = "";
        //        checkoutobj.delivery_city = custCity;
        //        checkoutobj.delivery_state = custState;
        //        checkoutobj.delivery_zip = custZip;
        //        checkoutobj.delivery_latitude = custLat;
        //        checkoutobj.delivery_longitude = custLong;
        //    }
        //}
    }
}
