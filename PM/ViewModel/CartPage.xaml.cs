using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using PM.Model;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using static PM.ViewModel.FoodBackStore;

namespace PM.ViewModel
{
    public partial class CartPage : ContentPage
    {
        public ObservableCollection<StoreItem> itemsSource = new ObservableCollection<StoreItem>();
        public Dictionary<StoreItem, int> itemAmounts = new Dictionary<StoreItem, int>();
        int threshold;
        string fbName, fbDist, fbImg;
        bool formFilled;
        FoodBanks chosenFb;

        public CartPage(FoodBanks foodbank, string bankName, string bankDistance, string bankImage, int storeThreshold, Dictionary<StoreItem, int> itmAmts)
        {
            chosenFb = foodbank;
            formFilled = false;
            threshold = storeThreshold;
            Preferences.Set("chosenBankName", bankName);
            Preferences.Set("chosenBankImg", bankImage);
            fbName = bankName;
            fbDist = bankDistance;
            fbImg = bankImage;
            itemAmounts = itmAmts;
            InitializeComponent();

            SetFoodBank(bankName, bankDistance, bankImage);
            SetCartItems();

            checkClientIntakeFormCompletion();
        }

        async void checkClientIntakeFormCompletion()
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://c1zwsl05s5.execute-api.us-west-1.amazonaws.com/dev/api/v2/households");
            request.Method = HttpMethod.Get;
            var client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var message = await response.Content.ReadAsStringAsync();
                try
                {
                    var data = JsonConvert.DeserializeObject<FormsGet>(message);
                    var receivedList = data.result;
                    for (int i = receivedList.Count - 1; i >= 0; i--)
                    {
                        if (receivedList[i].customer_uid == (string)Application.Current.Properties["user_id"] && receivedList[i].dob != "" && receivedList[i].dob == null)
                        {
                            Debug.WriteLine("the client intake form has been filled out before");
                            formFilled = true;
                        }
                    }
                }
                catch
                {
                    
                }
            }
        }

        void SetCartItems()
        {
            itemsSource.Clear();

            foreach (string itemName in cart.Keys)
            {
                itemsSource.Add(cart[itemName]);
            }

            cartItemList.ItemsSource = itemsSource;
        }

        void SetFoodBank(string name, string distance, string picture)
        {
            foodBankName.Text = name;
            foodBankDistance.Text = distance;
            foodBankPicture.Source = picture;
        }

        void NavigateToFoodBankStore(System.Object sender, System.EventArgs e)
        {
            //Application.Current.MainPage = new NavigationPage(new FoodBackStore());
            Navigation.PopAsync();
        }

        void DeleteItem(System.Object sender, System.EventArgs e)
        {
            var swipe = (SwipeItem)sender;
            var recognizer = swipe.CommandParameter;
            var item = (StoreItem)recognizer;

            totalQuantity -= item.quantity;
            item.quantityUpdate = 0;

            if (cart.ContainsKey(item.name))
            {
                cart.Remove(item.name);
            }

            if (itemAmounts.ContainsKey(item))
                itemAmounts.Remove(item);

            SetCartItems();
        }

        void AddItemToCart(System.Object sender, System.EventArgs e)
        {
            var label = (Label)sender;
            var recognizer = (TapGestureRecognizer)label.GestureRecognizers[0];
            var item = (StoreItem)recognizer.CommandParameter;

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

            CheckTotalQuantity();
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

            CheckTotalQuantity();
        }

        void CheckTotalQuantity()
        {
            if (totalQuantity == threshold)
            {
                DisplayAlert("Oops", "You have reached the maximum number of items", "OK");
            }
        }

        void NavigateToCheckoutPage(System.Object sender, System.EventArgs e)
        {
            //if (itemAmounts.Count == 0)
            //    DisplayAlert("Oops", "Choose items before checking out", "OK");
            //else Navigation.PushAsync(new ClientIntakeForm(itemAmounts), false);

            //set variable that will determine whether or not to show the user the West Valley Form on the way to checkout
            if (fbName == "West Valley Community Services")
                Preferences.Set("isWV", true);
            else Preferences.Set("isWV", false);
            if (!formFilled) { }
                //Navigation.PushAsync(new ClientIntakeForm(false, chosenFb, itemAmounts), false);
            else Navigation.PushAsync(new CheckoutPage(chosenFb, itemAmounts), false);
        }

        void backClicked(System.Object sender, System.EventArgs e)
        {
            //Application.Current.MainPage = new MainPage();
            Navigation.PopAsync(false);
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
    }
}
