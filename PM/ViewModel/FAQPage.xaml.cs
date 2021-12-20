using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using PM.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PM.ViewModel
{
    public partial class FAQPage : ContentPage
    {
        //public ObservableCollection<faqElement> questionColl = new ObservableCollection<faqElement>();
        int qNum;
        bool isGuest;

        public FAQPage(bool guestNav)
        {
            isGuest = guestNav;
            qNum = 1;
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;
            Console.WriteLine("Width = " + width.ToString());
            Console.WriteLine("Height = " + height.ToString());

            InitializeComponent();
            //fillList();
        }

        //async void fillList()
        //{
        //    for (int i = 0; i < 10; i++)
        //    {
        //        string quest = "What food banks are there?";
        //        string ans = "We have a variety!";
        //        questionColl.Add(new faqElement
        //        {
        //            question = qNum.ToString() + ") " + quest,
        //            answer = ans
        //        });

        //        faqCollView.ItemsSource = questionColl;
        //        if (qNum != 1)
        //            faqCollView.HeightRequest += 100;
        //        qNum++;
        //    }
        //}

        void backClicked(System.Object sender, System.EventArgs e)
        {
            //Application.Current.MainPage = new MainPage();
            Navigation.PopAsync();
        }

        //guest menu functions
        void registerClicked(System.Object sender, System.EventArgs e)
        {
            Dictionary<string, string> holder = new Dictionary<string, string>();
            Application.Current.MainPage = new NavigationPage(new Registration("DIRECT", holder));
        }

        void menuClicked(System.Object sender, System.EventArgs e)
        {
            if ((string)Application.Current.Properties["platform"] == "GUEST")
                openGuestMenuGrid.IsVisible = true;
            else openMenuGrid.IsVisible = true;
            //whiteCover.IsVisible = true;
            menu.IsVisible = false;
        }

        void openedMenuClicked(System.Object sender, System.EventArgs e)
        {
            if ((string)Application.Current.Properties["platform"] == "GUEST")
                openGuestMenuGrid.IsVisible = false;
            else openMenuGrid.IsVisible = false;
            //openMenuGrid.IsVisible = false;
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

        void loginClicked(System.Object sender, System.EventArgs e)
        {
            //Application.Current.Properties["platform"] = "GUEST";
            //Application.Current.Properties.Remove("user_id");
            Application.Current.MainPage = new LoginPage();
        }

        //end of guest menu functions

        //logged in menu functions
        void profileClicked(System.Object sender, System.EventArgs e)
        {
            //Application.Current.MainPage = new NavigationPage(new UserProfile());
            Navigation.PushAsync(new UserProfile());
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
