using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using PM.Model;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace PM.ViewModel
{
    public partial class Filter : ContentPage
    {
        //List<Date1> availableDates;
        //Date1 selectedDate;
        //allowing multiple date selections
        //List<Date1> selectedDates;
        List<ImageButton> selectedTypes;
        //holds all of the banks (doesn't get changed)
        public ObservableCollection<FoodBanks> totalBanksColl = new ObservableCollection<FoodBanks>();
        //holds all of the banks, the values store the amount of filters applied to hide a certain foodbank (ex: not delivery and not fruit)
        Dictionary<FoodBanks, int> totalBanks = new Dictionary<FoodBanks, int>();
        //the current list of banks showing on the screen
        ObservableCollection<FoodBanks> currentList = new ObservableCollection<FoodBanks>();
        double deviceLat = 0;
        double deviceLong = 0;
        Location deviceLoca;
        double savedfbListHeight = 0;
        DateTime todaysDate;
        string orderDateChosen;

        public Filter()
        {
            orderDateChosen = "";
            Debug.WriteLine("user id: " + Application.Current.Properties["user_id"]);

            selectedTypes = new List<ImageButton>();
            //availableDates = new List<Date1>();
            //allowing multiple date selections
            //selectedDates = new List<Date1>();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;
            Console.WriteLine("Width = " + width.ToString());
            Console.WriteLine("Height = " + height.ToString());

            InitializeComponent();
            if (Preferences.Get("foodBankListHeight", "") != "")
                foodBankColl.HeightRequest = double.Parse(Preferences.Get("foodBankListHeight", ""));

            getDates();
            getFoodBanks();
            //Debug.WriteLine("availableDates size: " + availableDates.Count);

            Debug.WriteLine("main grid height: " + mainGrid.HeightRequest);
            //Debug.WriteLine("topstack height: " +;
            Debug.WriteLine("foodbankcoll height: " + foodBankColl.HeightRequest);
            //foodBankColl.HeightRequest = height - 500;
            Debug.WriteLine("foodbankcoll height after: " + foodBankColl.HeightRequest);
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

        CancellationTokenSource cts;

        async Task GetCurrentLocation(string bankLatitude, string bankLongitude, FoodBanks bank)
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);

                if (location != null)
                {
                    Debug.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    deviceLat = location.Latitude;
                    deviceLong = location.Longitude;
                    //process the distance between the food bank and the phone
                    Location fbLocation = new Location(Double.Parse(bankLatitude), Double.Parse(bankLongitude));
                    Location deviceLocation = new Location(location.Latitude, location.Longitude);
                    deviceLoca = deviceLocation;
                    double miles = Location.CalculateDistance(fbLocation, deviceLocation, DistanceUnits.Miles);
                    miles = Math.Round(miles, 1);
                    Debug.WriteLine("distance between food bank and device: " + miles.ToString());
                    bank.distance = miles.ToString() + " miles";
                }
                else
                {
                    bank.distance = "-----";
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                await DisplayAlert("Oops", "Location permissions are not supported on this device.", "OK");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                await DisplayAlert("Oops", "Location permissions aren't enabled on this device.", "OK");
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                await DisplayAlert("Oops", "Please enable location permissions.", "OK");
            }
            catch (Exception ex)
            {
                // Unable to get location
                await DisplayAlert("Oops", "A problem occurred when trying to get this device's location.", "OK");
            }
        }

        async void getFoodBanks()
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://c1zwsl05s5.execute-api.us-west-1.amazonaws.com/dev/api/v2/businesses");
            Debug.WriteLine("BUSINESSES ENDPOINT TRYING TO BE REACHED: " + "https://c1zwsl05s5.execute-api.us-west-1.amazonaws.com/dev/api/v2/businesses");
            request.Method = HttpMethod.Get;
            var client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);
            var RDSMessage = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("RDSMessage from businesses endpoint: " + RDSMessage.ToString());
            var data = JsonConvert.DeserializeObject<BusinessesResponse>(RDSMessage);

            foreach (var bus in data.result.result)
            {
                FoodBanks bank = new FoodBanks();
                //try to get the latitude and longitude of the business
                try
                {

                    // Setting request for USPS API

                    Debug.WriteLine("INPUTS: ADDRESS1: {0}, ADDRESS2: {1}, CITY: {2}, STATE: {3}, ZIPCODE: {4}", bus.business_address, bus.business_unit, bus.business_city, bus.business_state, bus.business_zip);
                    XDocument requestDoc = new XDocument(
                        new XElement("AddressValidateRequest",
                        new XAttribute("USERID", "400INFIN1745"),
                        new XElement("Revision", "1"),
                        new XElement("Address",
                        new XAttribute("ID", "0"),
                        new XElement("Address1", bus.business_address),
                        new XElement("Address2", bus.business_unit != null ? bus.business_unit : ""),
                        new XElement("City", bus.business_city),
                        new XElement("State", bus.business_state),
                        new XElement("Zip5", bus.business_zip),
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
                            if ((GetXMLElement(element, "DPVConfirmation").Equals("Y") || GetXMLElement(element, "DPVConfirmation").Equals("S") || GetXMLElement(element, "DPVConfirmation").Equals("D")) && GetXMLElement(element, "Zip5").Equals(bus.business_zip) && GetXMLElement(element, "City").Equals(bus.business_city.ToUpper()))
                            {
                                Geocoder geoCoder = new Geocoder();

                                IEnumerable<Position> approximateLocations = await geoCoder.GetPositionsForAddressAsync(bus.business_address + "," + bus.business_city + "," + bus.business_state);
                                Position position = approximateLocations.FirstOrDefault();

                                string bankLatitude = $"{position.Latitude}";
                                string bankLongitude = $"{position.Longitude}";
                                Debug.Write("bank latitude: " + bankLatitude);
                                Debug.Write("bank longitude: " + bankLongitude);

                                if (deviceLat == 0)
                                {
                                    await GetCurrentLocation(bankLatitude, bankLongitude, bank);
                                }
                                else
                                {
                                    Location fbLocation = new Location(Double.Parse(bankLatitude), Double.Parse(bankLongitude));

                                    double miles = Location.CalculateDistance(fbLocation, deviceLoca, DistanceUnits.Miles);
                                    miles = Math.Round(miles, 1);
                                    Debug.WriteLine("(else) distance between food bank and device: " + miles.ToString());
                                    bank.distance = miles.ToString() + " miles";
                                }
                            }
                            else
                            {
                                bank.distance = "-----";
                            }
                        }
                        else
                        {
                            bank.distance = "-----";
                        }
                    }
                }
                catch
                {
                    bank.distance = "-----";
                }


                //FoodBanks bank = new FoodBanks();
                bank.name = bus.business_name;
                bank.business_uid = bus.business_uid;
                bank.bankImg = bus.business_image;
                bank.TotalVisible = true;
                bank.HoursVisible = false;
                bank.address = bus.business_address;
                bank.unit = bus.business_unit;
                bank.city = bus.business_city;
                bank.state = bus.business_state;
                bank.zip = bus.business_zip;

                if (bus.delivery != null)
                {
                    if (bus.delivery == "1")
                    {
                        bank.delivery = true;
                        bank.desc = "Delivery.";
                    }
                    else bank.delivery = false;
                }

                if (bus.pick_up != null)
                {
                    if (bus.pick_up == "1")
                    {
                        bank.pickup = true;
                        bank.desc += " Pick-Up.";
                    }
                    else bank.pickup = false;
                }

                //bank.pickup = true;

                try
                {
                    bank.itemLimit = int.Parse(bus.limit_per_person);
                    bank.desc += " Limited to " + bus.limit_per_person + " items.";
                }
                catch
                {
                    bank.itemLimit = 0;
                    bank.desc += " Limited to 0 items.";
                }

                if (bus.business_accepting_hours != null)
                {
                    try
                    {
                        Debug.WriteLine("business hours: " + bus.business_accepting_hours);
                        var hoursdata = JsonConvert.DeserializeObject<AcceptingHours>(bus.business_accepting_hours);

                        if (hoursdata.Monday[0] == "N/A")
                            bank.mondayHours = "Closed";
                        else bank.mondayHours = hoursdata.Monday[0].Substring(0, 5) + " - " + hoursdata.Monday[1].Substring(0, 5);

                        if (hoursdata.Tuesday[0] == "N/A")
                            bank.tuesdayHours = "Closed";
                        else bank.tuesdayHours = hoursdata.Tuesday[0].Substring(0, 5) + " - " + hoursdata.Tuesday[1].Substring(0, 5);

                        if (hoursdata.Wednesday[0] == "N/A")
                            bank.wednesdayHours = "Closed";
                        else bank.wednesdayHours = hoursdata.Wednesday[0].Substring(0, 5) + " - " + hoursdata.Wednesday[1].Substring(0, 5);

                        if (hoursdata.Thursday[0] == "N/A")
                            bank.thursdayHours = "Closed";
                        else bank.thursdayHours = hoursdata.Thursday[0].Substring(0, 5) + " - " + hoursdata.Thursday[1].Substring(0, 5);

                        if (hoursdata.Friday[0] == "N/A")
                            bank.fridayHours = "Closed";
                        else bank.fridayHours = hoursdata.Friday[0].Substring(0, 5) + " - " + hoursdata.Friday[1].Substring(0, 5);

                        if (hoursdata.Saturday[0] == "N/A")
                            bank.saturdayHours = "Closed";
                        else bank.saturdayHours = hoursdata.Saturday[0].Substring(0, 5) + " - " + hoursdata.Saturday[1].Substring(0, 5);

                        if (hoursdata.Sunday[0] == "N/A")
                            bank.sundayHours = "Closed";
                        else bank.sundayHours = hoursdata.Sunday[0].Substring(0, 5) + " - " + hoursdata.Sunday[1].Substring(0, 5);
                    }
                    catch
                    {
                        bank.mondayHours = "Closed";
                        bank.tuesdayHours = "Closed";
                        bank.wednesdayHours = "Closed";
                        bank.thursdayHours = "Closed";
                        bank.fridayHours = "Closed";
                        bank.saturdayHours = "Closed";
                        bank.sundayHours = "Closed";
                    }
                }

                if (bus.item_types != null)
                {
                    Debug.WriteLine("item types: " + bus.item_types);
                    try
                    {
                        var typesdata = JsonConvert.DeserializeObject<Types>(bus.item_types);
                        bank.fruits = typesdata.fruits;
                        bank.vegetables = typesdata.vegetables;
                        bank.meals = typesdata.meals;
                        bank.desserts = typesdata.desserts;
                        bank.beverages = typesdata.beverages;
                        bank.dairy = typesdata.dairy;
                        bank.snacks = typesdata.snacks;
                        bank.cannedFoods = typesdata.cannedFoods;
                    }
                    catch
                    {
                        Debug.WriteLine("didn't work");
                        bank.fruits = false;
                        bank.vegetables = false;
                        bank.meals = false;
                        bank.desserts = false;
                        bank.beverages = false;
                        bank.dairy = false;
                        bank.snacks = false;
                        bank.cannedFoods = false;
                    }
                }

                

                bank.FilterCount = 0;
                bank.Height = 90;
                //bank.distance = "1.5 Miles";

                totalBanksColl.Add(bank);
                totalBanks.Add(bank, 0);
            }

            //for (int i = 0; i < 11; i++)
            //{
            //    Banks.Add(new FoodBanks
            //    {
            //        name = "Feeding Orange County",
            //        HoursVisible = false
            //    });
            //}
            //loadingText.IsVisible = false;
            //foodBankColl.ItemsSource = totalBanksColl;
            //currentList = totalBanksColl;
            Debug.WriteLine("visible now");
            foodBankColl.ItemsSource = totalBanksColl;
            currentList = totalBanksColl;
            //loadingText.IsVisible = false;
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
                //Date1 addingDate = new Date1();
                //addingDate.BackgroundImg = "dateUnselected.png";
                //addingDate.dotw = nextDate.DayOfWeek.ToString().Substring(0, 1);
                //addingDate.day = nextDate.Day.ToString();
                //addingDate.month = nextDate.ToLongDateString().Substring(nextDate.ToLongDateString().IndexOf(",") + 2, 3);
                //addingDate.TextColor = Color.Black;
                //addingDate.dateObj = nextDate;
                //availableDates.Add(addingDate);
            }


            //Date1 newDate = new Date1();
            //newDate.BackgroundImg = "dateUnselected.png";
            //newDate.dotw = "S";
            //newDate.day = "27";
            //newDate.month = "Jun";
            //newDate.TextColor = Color.Black;
            //availableDates.Add(newDate);

            //for (int i = 0; i < 11; i++)
            //{
            //    availableDates.Add(new Date1
            //    {
            //        BackgroundImg = "dateUnselected.png",
            //        dotw = "S",
            //        day = "27",
            //        month = "Jun",
            //        TextColor = Color.Black

            //    });
            //}

            //dateCarousel.ItemsSource = availableDates;
        }


        //private void dateChange(object sender, EventArgs e)
        //{
        //    Button button1 = (Button)sender;
        //    Date1 dateChosen = button1.BindingContext as Date1;

        //    //unselected -> selected
        //    if (dateChosen.BackgroundImg == "dateUnselected.png")
        //    {
        //        orderDateChosen = dateChosen.dateObj.ToLongDateString();

        //        if (selectedDate != null)
        //        {
        //            selectedDate.BackgroundImg = "dateUnselected.png";
        //            selectedDate.TextColor = Color.Black;
        //        }
        //        selectedDate = dateChosen;
        //        dateChosen.BackgroundImg = "dateSelected.png";
        //        dateChosen.TextColor = Color.White;
        //        pickDateButton.Text = dateChosen.month + " " + dateChosen.day;
        //        //allowing multiple date selections
        //        //dateChosen.BackgroundImg = "dateSelected.png";
        //        //dateChosen.TextColor = Color.White;
        //        //selectedDates.Add(dateChosen);

        //        pickDateFrame.BackgroundColor = Color.FromHex("#E7404A"); 
        //        pickDateButton.TextColor = Color.White;

        //        ObservableCollection<FoodBanks> newBanks = new ObservableCollection<FoodBanks>();

        //        foreach (var bank in totalBanksColl)
        //        {
        //            if ((dateChosen.dateObj.DayOfWeek.ToString() == "Monday" && bank.mondayHours == "Closed") ||
        //                (dateChosen.dateObj.DayOfWeek.ToString() == "Tuesday" && bank.tuesdayHours == "Closed") ||
        //                (dateChosen.dateObj.DayOfWeek.ToString() == "Wednesday" && bank.wednesdayHours == "Closed") ||
        //                (dateChosen.dateObj.DayOfWeek.ToString() == "Thursday" && bank.thursdayHours == "Closed") ||
        //                (dateChosen.dateObj.DayOfWeek.ToString() == "Friday" && bank.fridayHours == "Closed") ||
        //                (dateChosen.dateObj.DayOfWeek.ToString() == "Saturday" && bank.saturdayHours == "Closed") ||
        //                (dateChosen.dateObj.DayOfWeek.ToString() == "Sunday" && bank.sundayHours == "Closed"))
        //            {
        //                totalBanks[bank] += 1;
        //            }
        //            else if (currentList.Contains(bank))
        //            {
        //                newBanks.Add(bank);
        //            }
        //        }

        //        currentList = newBanks;
        //        foodBankColl.ItemsSource = newBanks;

        //    }
        //    //selected -> unselected
        //    else
        //    {
        //        orderDateChosen = "";

        //        selectedDate = null;
        //        pickDateFrame.BackgroundColor = Color.White;
        //        pickDateButton.TextColor = Color.FromHex("#E7404A");
        //        pickDateButton.Text = "Pick a date";
        //        //allowing multiple date selections
        //        dateChosen.BackgroundImg = "dateUnselected.png";
        //        dateChosen.TextColor = Color.Black;
        //        //selectedDates.Remove(dateChosen);

        //        //if (selectedDates.Count == 0)
        //        //{
        //        //    pickDateFrame.BackgroundColor = Color.White;
        //        //    pickDateButton.TextColor = Color.FromHex("#E7404A");
        //        //}
        //        ObservableCollection<FoodBanks> newBanks = new ObservableCollection<FoodBanks>();

        //        foreach (var bank in totalBanksColl)
        //        {
        //            if (totalBanks[bank] > 1 &&
        //                ((dateChosen.dateObj.DayOfWeek.ToString() == "Monday" && bank.mondayHours == "Closed") ||
        //                (dateChosen.dateObj.DayOfWeek.ToString() == "Tuesday" && bank.tuesdayHours == "Closed") ||
        //                (dateChosen.dateObj.DayOfWeek.ToString() == "Wednesday" && bank.wednesdayHours == "Closed") ||
        //                (dateChosen.dateObj.DayOfWeek.ToString() == "Thursday" && bank.thursdayHours == "Closed") ||
        //                (dateChosen.dateObj.DayOfWeek.ToString() == "Friday" && bank.fridayHours == "Closed") ||
        //                (dateChosen.dateObj.DayOfWeek.ToString() == "Saturday" && bank.saturdayHours == "Closed") ||
        //                (dateChosen.dateObj.DayOfWeek.ToString() == "Sunday" && bank.sundayHours == "Closed")))
        //            {
        //                totalBanks[bank]--;
        //            }
        //            else if (totalBanks[bank] == 1 &&
        //                ((dateChosen.dateObj.DayOfWeek.ToString() == "Monday" && bank.mondayHours == "Closed") ||
        //                (dateChosen.dateObj.DayOfWeek.ToString() == "Tuesday" && bank.tuesdayHours == "Closed") ||
        //                (dateChosen.dateObj.DayOfWeek.ToString() == "Wednesday" && bank.wednesdayHours == "Closed") ||
        //                (dateChosen.dateObj.DayOfWeek.ToString() == "Thursday" && bank.thursdayHours == "Closed") ||
        //                (dateChosen.dateObj.DayOfWeek.ToString() == "Friday" && bank.fridayHours == "Closed") ||
        //                (dateChosen.dateObj.DayOfWeek.ToString() == "Saturday" && bank.saturdayHours == "Closed") ||
        //                (dateChosen.dateObj.DayOfWeek.ToString() == "Sunday" && bank.sundayHours == "Closed")))
        //            {
        //                totalBanks[bank]--;
        //                newBanks.Add(bank);
        //            }
        //            else if (totalBanks[bank] == 0)
        //                newBanks.Add(bank);

                  
        //        }

        //        currentList = newBanks;
        //        foodBankColl.ItemsSource = newBanks;

        //    }
        //}

        void clickedOpenNow(System.Object sender, System.EventArgs e)
        {
            //foodBankColl.IsVisible = false;
            foodBankColl.ScrollTo(0);
            //if the button is already selected, selected --> unselected
            if (openNowButton.TextColor == Color.White)
            {
                openNowFrame.BackgroundColor = Color.White;
                openNowButton.TextColor = Color.FromHex("#E7404A");
                ObservableCollection<FoodBanks> newBanks = new ObservableCollection<FoodBanks>();

                foreach (var bank in totalBanksColl)
                {
                    //only show it if there are no more filters on it
                    if (totalBanks[bank] > 1 &&
                        ((todaysDate.DayOfWeek.ToString() == "Monday" && bank.mondayHours == "Closed") ||
                        (todaysDate.DayOfWeek.ToString() == "Tuesday" && bank.tuesdayHours == "Closed") ||
                        (todaysDate.DayOfWeek.ToString() == "Wednesday" && bank.wednesdayHours == "Closed") ||
                        (todaysDate.DayOfWeek.ToString() == "Thursday" && bank.thursdayHours == "Closed") ||
                        (todaysDate.DayOfWeek.ToString() == "Friday" && bank.fridayHours == "Closed") ||
                        (todaysDate.DayOfWeek.ToString() == "Saturday" && bank.saturdayHours == "Closed") ||
                        (todaysDate.DayOfWeek.ToString() == "Sunday" && bank.sundayHours == "Closed")))
                    {
                        totalBanks[bank]--;
                    }
                    else if (totalBanks[bank] == 1 &&
                        ((todaysDate.DayOfWeek.ToString() == "Monday" && bank.mondayHours == "Closed") ||
                        (todaysDate.DayOfWeek.ToString() == "Tuesday" && bank.tuesdayHours == "Closed") ||
                        (todaysDate.DayOfWeek.ToString() == "Wednesday" && bank.wednesdayHours == "Closed") ||
                        (todaysDate.DayOfWeek.ToString() == "Thursday" && bank.thursdayHours == "Closed") ||
                        (todaysDate.DayOfWeek.ToString() == "Friday" && bank.fridayHours == "Closed") ||
                        (todaysDate.DayOfWeek.ToString() == "Saturday" && bank.saturdayHours == "Closed") ||
                        (todaysDate.DayOfWeek.ToString() == "Sunday" && bank.sundayHours == "Closed")))
                    {
                        totalBanks[bank]--;
                        newBanks.Add(bank);
                    }
                    else if (totalBanks[bank] == 0)
                        newBanks.Add(bank);

                }

                currentList = newBanks;
                foodBankColl.ItemsSource = newBanks;

            }
            else //unselected --> selected
            {
                openNowFrame.BackgroundColor = Color.FromHex("#E7404A");
                openNowButton.TextColor = Color.White;
                ObservableCollection<FoodBanks> newBanks = new ObservableCollection<FoodBanks>();


                foreach (var bank in totalBanksColl)
                {
                    if ((todaysDate.DayOfWeek.ToString() == "Monday" && bank.mondayHours == "Closed") ||
                        (todaysDate.DayOfWeek.ToString() == "Tuesday" && bank.tuesdayHours == "Closed") ||
                        (todaysDate.DayOfWeek.ToString() == "Wednesday" && bank.wednesdayHours == "Closed") ||
                        (todaysDate.DayOfWeek.ToString() == "Thursday" && bank.thursdayHours == "Closed") ||
                        (todaysDate.DayOfWeek.ToString() == "Friday" && bank.fridayHours == "Closed") ||
                        (todaysDate.DayOfWeek.ToString() == "Saturday" && bank.saturdayHours == "Closed") ||
                        (todaysDate.DayOfWeek.ToString() == "Sunday" && bank.sundayHours == "Closed"))
                    {
                        totalBanks[bank] += 1;
                    }
                    else if (currentList.Contains(bank))
                    {
                        newBanks.Add(bank);
                    }
                }

                currentList = newBanks;
                foodBankColl.ItemsSource = newBanks;
            }
        }

        void clickedDelivery(System.Object sender, System.EventArgs e)
        {
            //foodBankColl.IsVisible = false;
            foodBankColl.ScrollTo(0);
            //if the button is already selected, selected --> unselected
            if (deliveryButton.TextColor == Color.White)
            {
                deliveryFrame.BackgroundColor = Color.White;
                deliveryButton.TextColor = Color.FromHex("#E7404A");
                ObservableCollection<FoodBanks> newBanks = new ObservableCollection<FoodBanks>();

                foreach (var bank in totalBanksColl)
                {
                    //only show it if there are no more filters on it
                    if (!bank.delivery && totalBanks[bank] > 1)
                    {
                        totalBanks[bank]--;
                    }
                    else if (!bank.delivery && totalBanks[bank] == 1)
                    {
                        totalBanks[bank]--;
                        newBanks.Add(bank);
                    }
                    else if (totalBanks[bank] == 0)
                        newBanks.Add(bank);
                }

                currentList = newBanks;
                foodBankColl.ItemsSource = newBanks;

                //foreach (var bank in totalBanksColl)
                //{
                //    //only show it if there are no more filters on it
                //    if (!bank.desc.Contains("Delivery") && bank.FilterCount == 1)
                //    {
                //        bank.TotalVisible = true;
                //        //bank.Height = 90;
                //        bank.FilterCount--;
                //    }
                //}
                //foodBankColl.ItemsSource = Banks;
            }
            else //unselected --> selected
            {
                deliveryFrame.BackgroundColor = Color.FromHex("#E7404A");
                deliveryButton.TextColor = Color.White;
                ObservableCollection<FoodBanks> newBanks = new ObservableCollection<FoodBanks>();

                //foreach(var bank in totalBanksColl)
                //{
                //    newBanks.Add(bank);
                //}

                foreach (var bank in totalBanksColl)
                {
                    if (!bank.delivery)
                    {
                        totalBanks[bank] += 1;
                    }
                    else if (currentList.Contains(bank))
                        newBanks.Add(bank);
                }

                currentList = newBanks;
                foodBankColl.ItemsSource = newBanks;
                //for (int i = 0; i < totalBanks.Keys.Count; i++)
                //{
                //    if (totalBanks.Keys)
                //}

                //foreach (var bank in Banks)
                //{
                //    //bank.FilterCount++;

                //    if (!bank.delivery)
                //    {
                //        bank.TotalVisible = false;
                //        bank.FilterCount++;
                //    }

                //}
            }

            //foodBankColl.IsVisible = true;
            //foodBankColl.VerticalOptions = LayoutOptions.Fill;
        }

        void clickedPickup(System.Object sender, System.EventArgs e)
        {
            //foodBankColl.IsVisible = false;
            foodBankColl.ScrollTo(0);
            //if the button is already selected, selected --> unselected
            if (pickupButton.TextColor == Color.White)
            {
                pickupFrame.BackgroundColor = Color.White;
                pickupButton.TextColor = Color.FromHex("#E7404A");
                ObservableCollection<FoodBanks> newBanks = new ObservableCollection<FoodBanks>();

                foreach (var bank in totalBanksColl)
                {
                    //only show it if there are no more filters on it
                    if (!bank.pickup && totalBanks[bank] > 1)
                    {
                        totalBanks[bank]--;
                    }
                    else if (!bank.pickup && totalBanks[bank] == 1)
                    {
                        totalBanks[bank]--;
                        newBanks.Add(bank);
                    }
                    else if (totalBanks[bank] == 0)
                        newBanks.Add(bank);
                }

                currentList = newBanks;
                foodBankColl.ItemsSource = newBanks;

            }
            else //unselected --> selected
            {
                pickupFrame.BackgroundColor = Color.FromHex("#E7404A");
                pickupButton.TextColor = Color.White;
                ObservableCollection<FoodBanks> newBanks = new ObservableCollection<FoodBanks>();

                //foreach(var bank in totalBanksColl)
                //{
                //    newBanks.Add(bank);
                //}

                foreach (var bank in totalBanksColl)
                {
                    if (!bank.pickup)
                    {
                        totalBanks[bank] += 1;
                    }
                    else if (currentList.Contains(bank))
                        newBanks.Add(bank);
                }

                currentList = newBanks;
                foodBankColl.ItemsSource = newBanks;

            }

            //foodBankColl.IsVisible = true;
            //foodBankColl.VerticalOptions = LayoutOptions.Fill;
        }


        void clickedShowDates(System.Object sender, System.EventArgs e)
        {
            if (dateCarousel.IsVisible == true)
            {
                dateCarousel.IsVisible = false;
                clearDatesGrid.IsVisible = false;
            }
            else
            {
                if (TypesGrid.IsVisible == true)
                {
                    TypesGrid.IsVisible = false;
                    clearTypesGrid.IsVisible = false;
                }

                dateCarousel.IsVisible = true;
                clearDatesGrid.IsVisible = true;
            }
        }

        void clickedClearDates(System.Object sender, System.EventArgs e)
        {
            orderDateChosen = "";
            //selectedDate.BackgroundImg = "dateUnselected.png";
            //selectedDate.TextColor = Color.Black;
            pickDateButton.Text = "Pick a date";
            //allowing multiple date selections
            //foreach (Date1 date in selectedDates)
            //{
            //    date.BackgroundImg = "dateUnselected.png";
            //    date.TextColor = Color.Black;
            //}
            //selectedDates.Clear();
            pickDateFrame.BackgroundColor = Color.White;
            pickDateButton.TextColor = Color.FromHex("#E7404A");
        }

        void clickedShowTypes(System.Object sender, System.EventArgs e)
        {
            if (TypesGrid.IsVisible == true)
            {
                TypesGrid.IsVisible = false;
                clearTypesGrid.IsVisible = false;
            }
            else
            {
                if (dateCarousel.IsVisible == true)
                {
                    dateCarousel.IsVisible = false;
                    clearDatesGrid.IsVisible = false;
                }

                TypesGrid.IsVisible = true;
                clearTypesGrid.IsVisible = true;
            }
        }

        void clickedChooseType(System.Object sender, System.EventArgs e)
        {
            ImageButton imgButton = (ImageButton)sender;
            Debug.WriteLine("source of imgbutton: " + imgButton.Source.ToString());
            //if the item is unfilled, unselected --> selected
            if (imgButton.Source.ToString().IndexOf("Unfilled") != -1)
            {
                string source = imgButton.Source.ToString().Substring(6);
                source = source.Substring(0, source.IndexOf("Unfilled")) + "Filled.png";
                //Debug.WriteLine("source to change to: $" + source + "$");
                imgButton.Source = source;
                selectedTypes.Add(imgButton);

                pickTypeFrame.BackgroundColor = Color.FromHex("#E7404A");
                pickTypeButton.TextColor = Color.White;
                ObservableCollection<FoodBanks> newBanks = new ObservableCollection<FoodBanks>();

                if (imgButton.Source.ToString().IndexOf("fruits") != -1)
                {
                    foreach(var bank in totalBanksColl)
                    {
                        if (!bank.fruits)
                        {
                            totalBanks[bank] += 1;
                        }
                        else if (currentList.Contains(bank))
                            newBanks.Add(bank);
                    }

                    //foreach (var bank in currentList)
                    //{
                    //    if (!bank.fruits)
                    //    {
                    //        totalBanks[bank] += 1;
                    //    }
                    //    else newBanks.Add(bank);
                    //}

                    currentList = newBanks;
                    foodBankColl.ItemsSource = newBanks;
                }
                else if (imgButton.Source.ToString().IndexOf("vegetables") != -1)
                {
                    //foreach (var bank in currentList)
                    //{
                    //    if (!bank.vegetables)
                    //    {
                    //        totalBanks[bank] += 1;
                    //    }
                    //    else newBanks.Add(bank);
                    //}

                    foreach (var bank in totalBanksColl)
                    {
                        if (!bank.vegetables)
                        {
                            totalBanks[bank] += 1;
                        }
                        else if (currentList.Contains(bank))
                            newBanks.Add(bank);
                    }

                    currentList = newBanks;
                    foodBankColl.ItemsSource = newBanks;
                }
                else if (imgButton.Source.ToString().IndexOf("meals") != -1)
                {
                    foreach (var bank in totalBanksColl)
                    {
                        if (!bank.meals)
                        {
                            totalBanks[bank] += 1;
                        }
                        else if (currentList.Contains(bank))
                            newBanks.Add(bank);
                    }

                    currentList = newBanks;
                    foodBankColl.ItemsSource = newBanks;
                }
                else if (imgButton.Source.ToString().IndexOf("desserts") != -1)
                {
                    foreach (var bank in totalBanksColl)
                    {
                        if (!bank.desserts)
                        {
                            totalBanks[bank] += 1;
                        }
                        else if (currentList.Contains(bank))
                            newBanks.Add(bank);
                    }

                    currentList = newBanks;
                    foodBankColl.ItemsSource = newBanks;
                }
                else if (imgButton.Source.ToString().IndexOf("beverages") != -1)
                {
                    foreach (var bank in totalBanksColl)
                    {
                        if (!bank.beverages)
                        {
                            totalBanks[bank] += 1;
                        }
                        else if (currentList.Contains(bank))
                            newBanks.Add(bank);
                    }

                    currentList = newBanks;
                    foodBankColl.ItemsSource = newBanks;
                }
                else if (imgButton.Source.ToString().IndexOf("dairy") != -1)
                {
                    foreach (var bank in totalBanksColl)
                    {
                        if (!bank.dairy)
                        {
                            totalBanks[bank] += 1;
                        }
                        else if (currentList.Contains(bank))
                            newBanks.Add(bank);
                    }

                    currentList = newBanks;
                    foodBankColl.ItemsSource = newBanks;
                }
                else if (imgButton.Source.ToString().IndexOf("snacks") != -1)
                {
                    foreach (var bank in totalBanksColl)
                    {
                        if (!bank.snacks)
                        {
                            totalBanks[bank] += 1;
                        }
                        else if (currentList.Contains(bank))
                            newBanks.Add(bank);
                    }

                    currentList = newBanks;
                    foodBankColl.ItemsSource = newBanks;
                }
                else if (imgButton.Source.ToString().IndexOf("cannedFoods") != -1)
                {
                    foreach (var bank in totalBanksColl)
                    {
                        if (!bank.cannedFoods)
                        {
                            totalBanks[bank] += 1;
                        }
                        else if (currentList.Contains(bank))
                            newBanks.Add(bank);
                    }

                    currentList = newBanks;
                    foodBankColl.ItemsSource = newBanks;
                }
            }
            else
            {
                string source = imgButton.Source.ToString().Substring(6);
                source = source.Substring(0, source.IndexOf("Filled")) + "Unfilled.png";
                imgButton.Source = source;
                selectedTypes.Remove(imgButton);
                ObservableCollection<FoodBanks> newBanks = new ObservableCollection<FoodBanks>();

                if (imgButton.Source.ToString().IndexOf("fruits") != -1)
                {
                    foreach (var bank in totalBanksColl)
                    {
                        //only show it if there are no more filters on it
                        if (!bank.fruits && totalBanks[bank] > 1)
                        {
                            totalBanks[bank]--;
                        }
                        else if (!bank.fruits && totalBanks[bank] == 1)
                        {
                            totalBanks[bank]--;
                            newBanks.Add(bank);
                        }
                        else if (totalBanks[bank] == 0)
                            newBanks.Add(bank);
                    }

                    currentList = newBanks;
                    foodBankColl.ItemsSource = newBanks;
                }
                else if (imgButton.Source.ToString().IndexOf("vegetables") != -1)
                {
                    foreach (var bank in totalBanksColl)
                    {
                        //only show it if there are no more filters on it
                        if (!bank.fruits && totalBanks[bank] > 1)
                        {
                            totalBanks[bank]--;
                        }
                        else if (!bank.vegetables && totalBanks[bank] == 1)
                        {
                            totalBanks[bank]--;
                            newBanks.Add(bank);
                        }
                        else if (totalBanks[bank] == 0)
                            newBanks.Add(bank);
                    }

                    currentList = newBanks;
                    foodBankColl.ItemsSource = newBanks;
                }
                else if (imgButton.Source.ToString().IndexOf("meals") != -1)
                {
                    foreach (var bank in totalBanksColl)
                    {
                        //only show it if there are no more filters on it
                        if (!bank.meals && totalBanks[bank] > 1)
                        {
                            totalBanks[bank]--;
                        }
                        else if (!bank.meals && totalBanks[bank] == 1)
                        {
                            totalBanks[bank]--;
                            newBanks.Add(bank);
                        }
                        else if (totalBanks[bank] == 0)
                            newBanks.Add(bank);
                    }

                    currentList = newBanks;
                    foodBankColl.ItemsSource = newBanks;
                }
                else if (imgButton.Source.ToString().IndexOf("desserts") != -1)
                {
                    foreach (var bank in totalBanksColl)
                    {
                        //only show it if there are no more filters on it
                        if (!bank.desserts && totalBanks[bank] > 1)
                        {
                            totalBanks[bank]--;
                        }
                        else if (!bank.desserts && totalBanks[bank] == 1)
                        {
                            totalBanks[bank]--;
                            newBanks.Add(bank);
                        }
                        else if (totalBanks[bank] == 0)
                            newBanks.Add(bank);
                    }

                    currentList = newBanks;
                    foodBankColl.ItemsSource = newBanks;
                }
                else if (imgButton.Source.ToString().IndexOf("beverages") != -1)
                {
                    foreach (var bank in totalBanksColl)
                    {
                        //only show it if there are no more filters on it
                        if (!bank.beverages && totalBanks[bank] > 1)
                        {
                            totalBanks[bank]--;
                        }
                        else if (!bank.beverages && totalBanks[bank] == 1)
                        {
                            totalBanks[bank]--;
                            newBanks.Add(bank);
                        }
                        else if (totalBanks[bank] == 0)
                            newBanks.Add(bank);
                    }

                    currentList = newBanks;
                    foodBankColl.ItemsSource = newBanks;
                }
                else if (imgButton.Source.ToString().IndexOf("dairy") != -1)
                {
                    foreach (var bank in totalBanksColl)
                    {
                        //only show it if there are no more filters on it
                        if (!bank.dairy && totalBanks[bank] > 1)
                        {
                            totalBanks[bank]--;
                        }
                        else if (!bank.dairy && totalBanks[bank] == 1)
                        {
                            totalBanks[bank]--;
                            newBanks.Add(bank);
                        }
                        else if (totalBanks[bank] == 0)
                            newBanks.Add(bank);
                    }

                    currentList = newBanks;
                    foodBankColl.ItemsSource = newBanks;
                }
                else if (imgButton.Source.ToString().IndexOf("snacks") != -1)
                {
                    foreach (var bank in totalBanksColl)
                    {
                        //only show it if there are no more filters on it
                        if (!bank.snacks && totalBanks[bank] > 1)
                        {
                            totalBanks[bank]--;
                        }
                        else if (!bank.snacks && totalBanks[bank] == 1)
                        {
                            totalBanks[bank]--;
                            newBanks.Add(bank);
                        }
                        else if (totalBanks[bank] == 0)
                            newBanks.Add(bank);
                    }

                    currentList = newBanks;
                    foodBankColl.ItemsSource = newBanks;
                }
                else if (imgButton.Source.ToString().IndexOf("cannedFoods") != -1)
                {
                    foreach (var bank in totalBanksColl)
                    {
                        //only show it if there are no more filters on it
                        if (!bank.cannedFoods && totalBanks[bank] > 1)
                        {
                            totalBanks[bank]--;
                        }
                        else if (!bank.cannedFoods && totalBanks[bank] == 1)
                        {
                            totalBanks[bank]--;
                            newBanks.Add(bank);
                        }
                        else if (totalBanks[bank] == 0)
                            newBanks.Add(bank);
                    }

                    currentList = newBanks;
                    foodBankColl.ItemsSource = newBanks;
                }

                //foodBankColl.VerticalOptions = LayoutOptions.Fill;

                if (selectedTypes.Count == 0)
                {
                    pickTypeFrame.BackgroundColor = Color.White;
                    pickTypeButton.TextColor = Color.FromHex("#E7404A");
                }
            }
        }

        void clickedClearTypes(System.Object sender, System.EventArgs e)
        {
            foreach (ImageButton imgButton in selectedTypes)
            {
                string source = imgButton.Source.ToString().Substring(6);
                source = source.Substring(0, source.IndexOf("Filled")) + "Unfilled.png";
                imgButton.Source = source;
            }
            selectedTypes.Clear();
            pickTypeFrame.BackgroundColor = Color.White;
            pickTypeButton.TextColor = Color.FromHex("#E7404A");
        }

        

        void clickedShowHours(System.Object sender, System.EventArgs e)
        {
            ImageButton button1 = (ImageButton)sender;
            FoodBanks fbChosen = button1.BindingContext as FoodBanks;
            if (fbChosen.HoursVisible == true)
                fbChosen.HoursVisible = false;
            else fbChosen.HoursVisible = true;
        }
        
        async void clickedFoodBank(System.Object sender, System.EventArgs e)
        {
            Button button1 = (Button)sender;
            FoodBanks bankChosen = button1.BindingContext as FoodBanks;

            if (orderDateChosen == "")
            {
                bool dateAvailable = false;
                //just check a little over a week's worth of dates
                for (int i = 0; i < 8; i++)
                {
                    //Debug.WriteLine("available date: " + availableDates[i].dateObj.ToLongDateString());
                    //if ((availableDates[i].dateObj.DayOfWeek.ToString() == "Monday" && bankChosen.mondayHours != "Closed") ||
                    //    (availableDates[i].dateObj.DayOfWeek.ToString() == "Tuesday" && bankChosen.tuesdayHours != "Closed") ||
                    //    (availableDates[i].dateObj.DayOfWeek.ToString() == "Wednesday" && bankChosen.wednesdayHours != "Closed") ||
                    //    (availableDates[i].dateObj.DayOfWeek.ToString() == "Thursday" && bankChosen.thursdayHours != "Closed") ||
                    //    (availableDates[i].dateObj.DayOfWeek.ToString() == "Friday" && bankChosen.fridayHours != "Closed") ||
                    //    (availableDates[i].dateObj.DayOfWeek.ToString() == "Saturday" && bankChosen.saturdayHours != "Closed") ||
                    //    (availableDates[i].dateObj.DayOfWeek.ToString() == "Sunday" && bankChosen.sundayHours != "Closed"))
                    //{
                    //    dateAvailable = true;
                    //    Application.Current.Properties["date_chosen"] = availableDates[i].dateObj.ToLongDateString();
                    //    break;
                    //}
                }

                //if none of the available dates work
                if (dateAvailable == false)
                {
                    await DisplayAlert("Oops", "This food bank isn't available right now.", "OK");
                    return;
                }
                
            }
            else Application.Current.Properties["date_chosen"] = orderDateChosen;
            //else Preferences.Set("date_chosen", orderDateChosen);

            Debug.WriteLine("date chosen in filter pg: " + (string)Application.Current.Properties["date_chosen"]);

            //Application.Current.Properties["date_chosen"] = orderDateChosen;


            Application.Current.Properties["chosen_business_uid"] = bankChosen.business_uid;
            Debug.WriteLine("sending name: " + bankChosen.name);
            Debug.WriteLine("sending distance: " + bankChosen.distance);
            Debug.WriteLine("sending bankImg: " + bankChosen.bankImg);
            Debug.WriteLine("sending itemLimit: " + bankChosen.itemLimit);
            Debug.WriteLine("sending business_uid: " + bankChosen.business_uid);
            //await Navigation.PushAsync(new FoodBackStore(bankChosen, bankChosen.name, bankChosen.distance, bankChosen.bankImg, bankChosen.itemLimit, bankChosen.business_uid));
        }

        //menu functions
        void profileClicked(System.Object sender, System.EventArgs e)
        {
            //Application.Current.MainPage = new NavigationPage(new UserProfile());
            Navigation.PushAsync(new UserProfile());
        }

        void menuClicked(System.Object sender, System.EventArgs e)
        {
            //Debug.WriteLine("height of foodbank list: " + foodBankColl.Height);
            double savedHeight = foodBankColl.Height;
            savedfbListHeight = savedHeight;
            openMenuGrid.IsVisible = true;
            whiteCover.IsVisible = true;
            foodBankColl.HeightRequest = savedHeight;
            //Debug.WriteLine("height of foodbank list after reset: " + foodBankColl.Height);
            //foodBankColl.VerticalOptions = LayoutOptions.EndAndExpand;
            //Debug.WriteLine("height of foodbank list after vertical options set: " + foodBankColl.Height);
        }

        void openedMenuClicked(System.Object sender, System.EventArgs e)
        {
            openMenuGrid.IsVisible = false;
            whiteCover.IsVisible = false;
        }

        void orderClicked(System.Object sender, System.EventArgs e)
        {
            //Application.Current.MainPage = new FoodBanksMap();
            Navigation.PushAsync(new Filter());
        }

        void faqClicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new FAQPage(false));
        }

        void browseClicked(System.Object sender, System.EventArgs e)
        {
            //Application.Current.MainPage = new FoodBanksMap();
            //Navigation.PushAsync(new FoodBanksMap());
        }

        void loginClicked(System.Object sender, System.EventArgs e)
        {
            openMenuGrid.IsVisible = false;
            whiteCover.IsVisible = false;
            Preferences.Set("foodBankListHeight", foodBankColl.Height + 25);
            Application.Current.Properties["platform"] = "GUEST";
            Application.Current.Properties.Remove("user_id");
            Application.Current.MainPage = new LoginPage();
        }

        //end of menu functions
    }
}
