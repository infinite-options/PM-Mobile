using System;
using Xamarin.Forms;
using Xamarin.Essentials;
using PM.Model;
using PM.Model.Login.LoginClasses;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using PM.Constants;
using Newtonsoft.Json;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Xamarin.Forms.Maps;
using PM.ViewModel;

namespace PM.ViewModel
{
    public partial class CongratsPage : ContentPage
    {

        public CongratsPage()
        {
            try
            {
                var width = DeviceDisplay.MainDisplayInfo.Width;
                var height = DeviceDisplay.MainDisplayInfo.Height;
                InitializeComponent();

            }
            catch (Exception ex)
            {
                //Generic gen = new Generic();
                //gen.parseException(ex.ToString());
            }
        }

        async void clickedFilter(System.Object sender, System.EventArgs e)
        {
            //await Navigation.PushAsync(new NavigationPage(new Filter()));
            Application.Current.MainPage = new NavigationPage(new Filter());
        }

        //async void clickedMenu(System.Object sender, System.EventArgs e)
        //{
        //    await Navigation.PushAsync(new Menu(cust_firstName, cust_lastName, cust_email));
        //}


        //menu functions
        void profileClicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new UserProfile());
        }

        void menuClicked(System.Object sender, System.EventArgs e)
        {
            openMenuGrid.IsVisible = true;
            whiteCover.IsVisible = true;
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
