using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using PM.Model;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace PM.ViewModel
{
    public partial class FoodBackStore : ContentPage
    {
        public ObservableCollection<StoreItem> itemSource = new ObservableCollection<StoreItem>();
        public ObservableCollection<FilterItem> filterSource = new ObservableCollection<FilterItem>();
        public static Dictionary<string, StoreItem> cart = new Dictionary<string, StoreItem>();
        //used to store how much of each item the user has selected in their cart
        public static Dictionary<StoreItem, int> itemAmounts = new Dictionary<StoreItem, int>();

        public Color filterBackgroundColorSelected = Color.FromHex("#E7404A");
        public Color filterBackgroundColorNonselected = Color.White;
        public Color filterTextColorSelected = Color.White;
        public Color filterTextColorNonselected = Color.FromHex("#E7404A");

        public static int totalQuantity = 0;
        public int threshold = 5;
        public string bankName, bankDistance, bankImg, bankUid;
        public int bankItemLimit;
        FoodBanks chosenFb;

        public FoodBackStore(FoodBanks foodbank, string fbName, string fbDistance, string fbImage, int itemLimit, string businessUid)
        {
            chosenFb = foodbank;
            bankName = fbName; bankDistance = fbDistance; bankImg = fbImage; bankUid = businessUid;
            threshold = itemLimit;
            //Debug.WriteLine("itemLimit: " + itemLimit.ToString());
            //limit.Text = "You can pick up to " + itemLimit.ToString() + " items";

            //for testing:
            threshold = 7;
            InitializeComponent();

            limit.Text = "You can pick up to " + threshold.ToString() + " items";
            SetFoodBank();
            SetFilters();
            SetItems();
            SetCartQuantity();
        }

        void SetFoodBank()
        {
            foodBankName.Text = bankName;
            foodBankDistance.Text = bankDistance;
            foodBankPicture.Source = bankImg;
        }

        void SetFilters()
        {
            try
            {
                filterList.ItemsSource = filterSource;

                var filterArray = new string[] { "fruit", "vegetable", "meal", "dessert", "beverage", "dairy", "snack", "canned" };

                foreach (var type in filterArray)
                {
                    filterSource.Add(new FilterItem()
                    {
                        filterName = type,
                        filterColor = filterBackgroundColorNonselected,
                        filterTextColor = filterTextColorNonselected,
                    });
                }

                //if (filterSource.Count > 0)
                //{
                //    filterSource[0].filterColor = filterBackgroundColorSelected;
                //    filterSource[0].filterTextColor = filterTextColorSelected;
                //}

            }
            catch (Exception issue)
            {
                Debug.WriteLine(issue.Message);
            }
        }

        async void SetItems()
        {
            try
            {
                string url = "https://c1zwsl05s5.execute-api.us-west-1.amazonaws.com/dev/api/v2/getItems?receive_business_uid=" + bankUid;
                Debug.WriteLine("url for getItems: " + url);
                var request3 = new HttpRequestMessage();
                request3.RequestUri = new Uri(url);
                request3.Method = HttpMethod.Get;
                var client2 = new HttpClient();
                HttpResponseMessage response = await client2.SendAsync(request3);
                var message = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("RDSResponse from getItems endpoint: " + response.ToString());
                Debug.WriteLine("RDSMessage from getItems endpoint: " + message.ToString());
                var data = JsonConvert.DeserializeObject<GetItemsResponse>(message);

                //List<string> foodTypes = new List<string>();
                //List<string> bankIds = new List<string>();
                //bankIds.Add(bankUid);
                //GetItems getItemsObj = new GetItems();
                //getItemsObj.types = foodTypes;
                //getItemsObj.ids = bankIds;

                //var getItemsSerializedObject = JsonConvert.SerializeObject(getItemsObj);
                //var content = new StringContent(getItemsSerializedObject, Encoding.UTF8, "application/json");

                //System.Diagnostics.Debug.WriteLine("getItems obj: " + getItemsSerializedObject);

                //var getItemsClient = new HttpClient();
                //var RDSResponse = await getItemsClient.PostAsync("https://c1zwsl05s5.execute-api.us-west-1.amazonaws.com/dev/api/v2/getItems", content);
                //var RDSMessage = await RDSResponse.Content.ReadAsStringAsync();
                //Debug.WriteLine("RDSResponse from getItems endpoint: " + RDSResponse.ToString());
                //Debug.WriteLine("RDSMessage from getItems endpoint: " + RDSMessage.ToString());
                //var data = JsonConvert.DeserializeObject<GetItemsResponse>(RDSMessage);
                //BusinessInfo[] filler = new BusinessInfo[0];
                if (data.result.Length == 0)
                {
                    await DisplayAlert("Oops", "This food bank currently has no items available.", "OK");
                    //commented out for testing purposes
                    //await Navigation.PopAsync();
                    return;
                }

                itemsList.ItemsSource = itemSource;

                foreach (var business in data.result)
                {
                    int item_quantity;
                    if (business.sup_num == "")
                    {
                        //set quantity to 5 for testing to checkout with items
                        item_quantity = 5;
                        //item_quantity = 0;
                    }
                        
                    else item_quantity = int.Parse(business.sup_num);

                    var item = new StoreItem()
                    {
                        image = business.item_photo,
                        name = business.item_name,
                        measure = business.detailed_num + " " + business.detailed_measure,
                        quantity = 0,
                        type = business.item_type,
                        availableAmt = item_quantity,
                        unit = business.detailed_unit,
                        price = 1,
                        item_uid = business.item_uid,
                        itm_business_uid = business.receive_business_uid
                    };

                    //if (cart.Count != 0)
                    //{
                    //    if (cart.ContainsKey(item.name))
                    //    {
                    //        item.quantity = cart[item.name].quantity;
                    //    }
                    //}

                    itemSource.Add(item);
                }

            }
            catch
            {
                //await DisplayAlert("Oops", "This food bank currently has no items available.", "OK");

                //commented out for testing
                //await Navigation.PopAsync();
            }

            //itemsList.ItemsSource = itemSource;

            //var items = new Dictionary<string, string>();

            //items.Add("item1", "Fruits");
            //items.Add("item2", "Fruits");
            //items.Add("item3", "Fruits");
            //items.Add("item4", "Vegetables");
            //items.Add("item5", "Vegetables");
            //items.Add("item6", "Meals");
            //items.Add("item7", "Meals");
            //items.Add("item8", "Meals");
            //items.Add("item9", "Meals");
            //items.Add("item10", "Desserts");

            //foreach (string name in items.Keys)
            //{
            //    var item = new StoreItem()
            //    {
            //        image = "itemImage",
            //        name = name,
            //        quantity = 0,
            //        type = items[name],
            //    };

            //    if (cart.Count != 0)
            //    {
            //        if (cart.ContainsKey(item.name))
            //        {
            //            item.quantity = cart[item.name].quantity;
            //        }
            //    }

            //    itemSource.Add(item);
            //}
        }

        void AddRemoveFilter(System.Object sender, System.EventArgs e)
        {
            var frame = (Frame)sender;
            var recognizer = (TapGestureRecognizer)frame.GestureRecognizers[0];
            var filter = (FilterItem)recognizer.CommandParameter;

            if (filter.filterColor == filterBackgroundColorSelected)
            {
                filter.filterColorUpdate = filterBackgroundColorNonselected;
                filter.filterTextColorUpdate = filterTextColorNonselected;
            }
            else
            {
                filter.filterColorUpdate = filterBackgroundColorSelected;
                filter.filterTextColorUpdate = filterTextColorSelected;
            }

            var allFilterTypes = new List<string>();

            foreach (FilterItem item in filterSource)
            {
                if (item.filterColor == filterBackgroundColorSelected)
                {
                    allFilterTypes.Add(item.filterName);
                }
            }

            if (allFilterTypes.Count != 0)
            {
                var tempList = new ObservableCollection<StoreItem>();
                foreach (string type in allFilterTypes)
                {
                    foreach (StoreItem item in itemSource)
                    {
                        if (type == item.type)
                        {
                            tempList.Add(item);
                        }
                    }
                }
                itemsList.ItemsSource = tempList;
            }
            else
            {
                itemsList.ItemsSource = itemSource;
            }
        }

        void AddItemToCart(System.Object sender, System.EventArgs e)
        {
            var label = (Label)sender;
            var recognizer = (TapGestureRecognizer)label.GestureRecognizers[0];
            var item = (StoreItem)recognizer.CommandParameter;

            if (item.quantity == item.availableAmt)
            {
                DisplayAlert("Oops", "You've already selected the max amount of this item that we currently have available.", "OK");
                return;
            }
                

            if (totalQuantity < threshold)
            {
                item.quantityUpdate = item.quantity + 1;

                totalQuantity = totalQuantity + 1;

                if (!cart.ContainsKey(item.name))
                {
                    cart.Add(item.name, item);
                }
                else
                {
                    cart[item.name] = item;
                }

                if (itemAmounts.ContainsKey(item))
                    itemAmounts[item]++;
                else itemAmounts[item] = 1;
            }

            SetCartQuantity();
        }

        void RemoveItemFromCart(System.Object sender, System.EventArgs e)
        {
            var label = (Label)sender;
            var recognizer = (TapGestureRecognizer)label.GestureRecognizers[0];
            var item = (StoreItem)recognizer.CommandParameter;

            if (item.quantity != 0)
            {
                if (itemAmounts.ContainsKey(item) && itemAmounts[item] == 1)
                    itemAmounts.Remove(item);
                else itemAmounts[item]--;

                item.quantityUpdate = item.quantity - 1;

                totalQuantity = totalQuantity - 1;

                if (item.quantity != 0)
                {
                    if (!cart.ContainsKey(item.name))
                    {
                        cart.Add(item.name, item);
                    }
                    else
                    {
                        cart[item.name] = item;
                    }
                }
                else
                {
                    if (cart.ContainsKey(item.name))
                    {
                        cart.Remove(item.name);
                    }
                }
            }

            SetCartQuantity();
        }

        void SetCartQuantity()
        {
            cartQuantity.Text = totalQuantity.ToString();

            if (totalQuantity == threshold)
            {
                DisplayAlert("Oops", "You have reached the maximum number of items", "OK");
            }
        }

        void NavigateToCartPage(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new CartPage(chosenFb, bankName, bankDistance, bankImg, threshold, itemAmounts), false);
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

        void filterClicked(System.Object sender, System.EventArgs e)
        {
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
            Debug.WriteLine("logout clicked");
            Application.Current.MainPage = new LoginPage();
        }

        //end of menu functions
    }
}
