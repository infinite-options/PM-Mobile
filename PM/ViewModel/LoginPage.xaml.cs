using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PM.Constants;
using PM.LogInClasses;
using PM.Model;
using PM.Model.Login;
using PM.Model.Login.LoginClasses;
using PM.Model.Login.LoginClasses.Apple;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Auth;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PM.ViewModel
{
    public partial class LoginPage : ContentPage
    {
        public bool createAccount = false;
        string deviceId;
        int directEmailVerified = 0;
        public HttpClient client = new HttpClient();
        LoginViewModel vm = new LoginViewModel();
        public event EventHandler SignIn;
        bool wrongPass;
        bool forgotPassSent;

        public LoginPage()
        {
            Application.Current.Properties["platform"] = "GUEST";
            forgotPassSent = false;
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;
            Console.WriteLine("Width = " + width.ToString());
            Console.WriteLine("Height = " + height.ToString());
            wrongPass = false;

            InitializeComponent();

            vm.AppleError += AppleError;
        }

        void backClicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        async void forgotPassClicked(System.Object sender, System.EventArgs e)
        {
            string result = await DisplayPromptAsync("Forgot Password", "Enter your email and we will send you a temporary password.", keyboard: Keyboard.Email);

            ForgotPassword forgotPassObj = new ForgotPassword();
            forgotPassObj.email = result;
            string forgotpassContentJson = JsonConvert.SerializeObject(forgotPassObj); // make orderContent into json
            var httpContent = new StringContent(forgotpassContentJson, Encoding.UTF8, "application/json"); // encode orderContentJson into format to send to database
            var response = await client.PostAsync(Constant.BaseUrl + "/api/v2/set_temp_password", httpContent); // try to post to database
            var message = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine("forgotpass message: " + message);
            if (message.Contains("A temporary password has been sent"))
            {
                await DisplayAlert("Success", "A temporary password has been sent to your email.", "OK");
                forgotPassSent = true;
            }
            else await DisplayAlert("Oops", "There was an error when we were trying to send you a temp password.", "OK");
        }


        //menu functions
        void registerClicked(System.Object sender, System.EventArgs e)
        {
            Dictionary<string, string> holder = new Dictionary<string, string>();
            Application.Current.MainPage = new NavigationPage(new Registration("DIRECT", holder));
        }

        void menuClicked(System.Object sender, System.EventArgs e)
        {
            //openMenuGrid.IsVisible = true;
            //menu.IsVisible = false;
        }

        void openedMenuClicked(System.Object sender, System.EventArgs e)
        {
            //openMenuGrid.IsVisible = false;
            //menu.IsVisible = true;
        }

        void browseClicked(System.Object sender, System.EventArgs e)
        {
            //Application.Current.MainPage = new FoodBanksMap();
            //Navigation.PushAsync(new FoodBanksMap());
        }

        void loginClicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new LoginPage();
        }

        //end of menu functions

        void clickedSeePassword(System.Object sender, System.EventArgs e)
        {
            //if (passEntry.IsPassword == true)
            //    passEntry.IsPassword = false;
            //else passEntry.IsPassword = true;
        }

        async void tempLogin(System.Object sender, System.EventArgs e)
        {
            SHA512 sHA512 = new SHA512Managed();
            byte[] data = sHA512.ComputeHash(Encoding.UTF8.GetBytes(""));//passEntry.Text.Trim())); // take the password and account salt to generate hash
            string hashedPassword = BitConverter.ToString(data).Replace("-", string.Empty).ToLower(); // convert hash to hex
            Debug.WriteLine("hashedpassword without salt: " + hashedPassword);

            SHA512 sHA5122 = new SHA512Managed();
            byte[] data2 = sHA5122.ComputeHash(Encoding.UTF8.GetBytes(""));// passEntry.Text.Trim() + "2021-09-09 05:36:40")); // take the password and account salt to generate hash
            string hashedPassword2 = BitConverter.ToString(data2).Replace("-", string.Empty).ToLower(); // convert hash to hex
            Debug.WriteLine("hashedpassword with first salt: " + hashedPassword2);

            SHA512 sHA51222 = new SHA512Managed();
            byte[] data3 = sHA5122.ComputeHash(Encoding.UTF8.GetBytes(""));// passEntry.Text.Trim() + "2021-09-07 16:58:56")); // take the password and account salt to generate hash
            string hashedPassword3 = BitConverter.ToString(data3).Replace("-", string.Empty).ToLower(); // convert hash to hex
            Debug.WriteLine("hashedpassword with last salt: " + hashedPassword3);
        }

        async void clickedLogin(System.Object sender, System.EventArgs e)
        {
            var accountSalt = await retrieveAccountSalt("");// nameEntry.Text.ToLower().Trim());

            if (accountSalt != null)
            {
                wrongPass = false;
                var loginAttempt = await LogInUser("","", accountSalt);//nameEntry.Text.ToLower(), passEntry.Text, accountSalt);


                //if (directEmailVerified == 0)
                //{
                //    DisplayAlert("Please Verify Email", "Please click the link in the email sent to " + nameEntry.Text + ". Check inbox and spam folders.", "OK");

                //    //send email to verify email
                //    emailVerifyPost emailVer = new emailVerifyPost();
                //    emailVer.email = nameEntry.Text.Trim();
                //    var emailVerSerializedObj = JsonConvert.SerializeObject(emailVer);
                //    var content4 = new StringContent(emailVerSerializedObj, Encoding.UTF8, "application/json");
                //    var client3 = new HttpClient();
                //    var response3 = client3.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/email_verification", content4);
                //    Console.WriteLine("RESPONSE TO CHECKOUT   " + response3.Result);
                //    Console.WriteLine("CHECKOUT JSON OBJECT BEING SENT: " + emailVerSerializedObj);

                //    loginButton.IsEnabled = true;
                //}

                if (loginAttempt == null && wrongPass == false)
                {
                    await DisplayAlert("Oops", "Phone number couldn't be found in our records.", "OK");
                }
                else if (loginAttempt != null && loginAttempt.message == "Wrong password")
                {
                    await DisplayAlert("Oops", "Wrong password", "OK");
                }
                else if (loginAttempt != null && loginAttempt.message.ToLower().Contains("email not found"))
                {
                    await DisplayAlert("Oops", "Email not found. Please sign up", "OK");
                }
                else if (loginAttempt != null)
                {
                    Debug.WriteLine("USER'S DATA");
                    Debug.WriteLine("USER CUSTOMER_UID: " + loginAttempt.result[0].customer_uid);
                    Debug.WriteLine("USER FIRST NAME: " + loginAttempt.result[0].customer_first_name);
                    Debug.WriteLine("USER LAST NAME: " + loginAttempt.result[0].customer_last_name);
                    Debug.WriteLine("USER EMAIL: " + loginAttempt.result[0].customer_email);
                    Application.Current.Properties["user_id"] = loginAttempt.result[0].customer_uid;
                    //Application.Current.Properties["user_email"] = loginAttempt.result[0].customer_email;

                    DateTime today = DateTime.Now;
                    DateTime expDate = today.AddDays(Constant.days);

                    Application.Current.Properties["time_stamp"] = expDate;
                    Application.Current.Properties["platform"] = "DIRECT";


                    //if the user clicked forgot password and they were sent an email with the temporary one
                    if (forgotPassSent)
                    {
                        string result = await DisplayPromptAsync("Update Password", "Enter a password you would like to use to replace the temporary one we gave you.", accept: "Confirm");
                        
                        UpdatePassword updatePassObj = new UpdatePassword();
                        updatePassObj.customer_uid = loginAttempt.result[0].customer_uid;
                        updatePassObj.old_password = "";// passEntry.Text.Trim();
                        updatePassObj.new_password = result;
                        string updatepassContentJson = JsonConvert.SerializeObject(updatePassObj); // make orderContent into json
                        var httpContent = new StringContent(updatepassContentJson, Encoding.UTF8, "application/json"); // encode orderContentJson into format to send to database
                        var response = await client.PostAsync(Constant.BaseUrl + "/api/v2/change_password", httpContent); // try to post to database
                        var message = await response.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.WriteLine("changepass message: " + message);
                        if (message.Contains("Password updated"))
                        {
                            await DisplayAlert("Success", "Your password has been updated.", "OK");
                            forgotPassSent = true;
                        }
                        else await DisplayAlert("Oops", "We could not update your password at this time.", "OK");
                    }
                    

                    Application.Current.MainPage = new NavigationPage(new Filter());


                    //System.Diagnostics.Debug.WriteLine("USER'S DATA");
                    //System.Diagnostics.Debug.WriteLine("USER CUSTOMER_UID: " + loginAttempt.result[0].customer_uid);
                    //System.Diagnostics.Debug.WriteLine("USER FIRST NAME: " + loginAttempt.result[0].customer_first_name);
                    //System.Diagnostics.Debug.WriteLine("USER LAST NAME: " + loginAttempt.result[0].customer_last_name);
                    //System.Diagnostics.Debug.WriteLine("USER EMAIL: " + loginAttempt.result[0].customer_email);

                    //DateTime today = DateTime.Now;
                    //DateTime expDate = today.AddDays(Constant.days);

                    //Application.Current.Properties["user_id"] = loginAttempt.result[0].customer_uid;
                    //Application.Current.Properties["time_stamp"] = expDate;
                    //Application.Current.Properties["platform"] = "DIRECT";

                    //// Application.Current.MainPage = new CarlosHomePage();
                    //// This statement initializes the stack to Subscription Page
                    ////check to see if user has already selected a meal plan before
                    //var request = new HttpRequestMessage();
                    //Console.WriteLine("user_id: " + (string)Application.Current.Properties["user_id"]);
                    //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=" + (string)Application.Current.Properties["user_id"];
                    ////old db
                    ////string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=" + (string)Application.Current.Properties["user_id"];
                    ////string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + (string)Application.Current.Properties["user_id"];
                    ////string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + "100-000256";
                    //Console.WriteLine("url: " + url);
                    //request.RequestUri = new Uri(url);
                    ////request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/get_delivery_info/400-000453");
                    //request.Method = HttpMethod.Get;
                    //var client = new HttpClient();
                    //HttpResponseMessage response = await client.SendAsync(request);

                    //if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    //{
                    //    HttpContent content = response.Content;
                    //    Console.WriteLine("content: " + content);
                    //    var userString = await content.ReadAsStringAsync();
                    //    Console.WriteLine(userString);

                    //    //writing guid to db
                    //    if (Preferences.Get("setGuid" + (string)Application.Current.Properties["user_id"], false) == false)
                    //    {
                    //        if (Device.RuntimePlatform == Device.iOS)
                    //        {
                    //            deviceId = Preferences.Get("guid", null);
                    //            if (deviceId != null) { Debug.WriteLine("This is the iOS GUID from Log in: " + deviceId); }
                    //        }
                    //        else
                    //        {
                    //            deviceId = Preferences.Get("guid", null);
                    //            if (deviceId != null) { Debug.WriteLine("This is the Android GUID from Log in " + deviceId); }
                    //        }

                    //        if (deviceId != null)
                    //        {
                    //            GuidPost notificationPost = new GuidPost();

                    //            notificationPost.uid = (string)Application.Current.Properties["user_id"];
                    //            notificationPost.guid = deviceId.Substring(5);
                    //            Application.Current.Properties["guid"] = deviceId.Substring(5);
                    //            notificationPost.notification = "TRUE";

                    //            var notificationSerializedObject = JsonConvert.SerializeObject(notificationPost);
                    //            Debug.WriteLine("Notification JSON Object to send: " + notificationSerializedObject);

                    //            var notificationContent = new StringContent(notificationSerializedObject, Encoding.UTF8, "application/json");

                    //            var clientResponse = await client.PostAsync(Constant.GuidUrl, notificationContent);

                    //            Debug.WriteLine("Status code: " + clientResponse.IsSuccessStatusCode);

                    //            if (clientResponse.IsSuccessStatusCode)
                    //            {
                    //                System.Diagnostics.Debug.WriteLine("We have post the guid to the database");
                    //                Preferences.Set("setGuid" + (string)Application.Current.Properties["user_id"], true);
                    //            }
                    //            else
                    //            {
                    //                await DisplayAlert("Ooops!", "Something went wrong. We are not able to send you notification at this moment", "OK");
                    //            }
                    //        }
                    //    }
                    //    //written


                    //}

                    //Application.Current.MainPage = new NavigationPage(new Filter());
                }
            }
            else
            {
                await DisplayAlert("Oops", "Phone number couldn't be found in our records.", "OK");
            }
            //loginButton.IsEnabled = true;
        }

        public async Task<LogInResponse> LogInUser(string userPhone, string userPassword, AccountSalt accountSalt)
        {
                SHA512 sHA512 = new SHA512Managed();
                byte[] data = sHA512.ComputeHash(Encoding.UTF8.GetBytes(userPassword + accountSalt.password_salt)); // take the password and account salt to generate hash
                string hashedPassword = BitConverter.ToString(data).Replace("-", string.Empty).ToLower(); // convert hash to hex

                LogInPost loginPostContent = new LogInPost();
                loginPostContent.phone = userPhone;
                loginPostContent.password = hashedPassword;
                loginPostContent.social_id = "";
                loginPostContent.signup_platform = "";
                Preferences.Set("hashed_password", hashedPassword);
                Preferences.Set("user_password", userPassword);
                Console.WriteLine("accountSalt: " + accountSalt.password_salt);
                Console.WriteLine("userPassword: " + userPassword);

                string loginPostContentJson = JsonConvert.SerializeObject(loginPostContent); // make orderContent into json

                var httpContent = new StringContent(loginPostContentJson, Encoding.UTF8, "application/json"); // encode orderContentJson into format to send to database
                var response = await client.PostAsync(Constant.LogInUrl, httpContent); // try to post to database
                var message = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine("LogInUser message: " + message);
            //string emailVerify = message.Substring(message.IndexOf("email_verified") + 18, 1);
            //Debug.WriteLine("emailVerify: " + emailVerify);
            //if (emailVerify == "1")
            //    directEmailVerified = 1;
            //if (message.IndexOf("email_verified") == -1)
            //    directEmailVerified = -1;


            try
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonConvert.DeserializeObject<LogInResponse>(responseContent);
                return loginResponse;
            }
            catch
            {
                if (message.Contains("Wrong password"))
                {
                    wrongPass = true;
                    await DisplayAlert("Oops", "Wrong password", "OK");
                }

                return null;
            }
            //if (message.Contains(Constant.AutheticatedSuccesful))
            //{
            //    var responseContent = await response.Content.ReadAsStringAsync();
            //    var loginResponse = JsonConvert.DeserializeObject<LogInResponse>(responseContent);
            //    return loginResponse;
            //}
            //else
            //{
            //    return null;
            //}
        }

        private async Task<AccountSalt> retrieveAccountSalt(string userPhone)
        {
            System.Diagnostics.Debug.WriteLine(userPhone);

            SaltPost saltPost = new SaltPost();
            saltPost.phone = userPhone;

            var saltPostSerilizedObject = JsonConvert.SerializeObject(saltPost);
            var saltPostContent = new StringContent(saltPostSerilizedObject, Encoding.UTF8, "application/json");

            System.Diagnostics.Debug.WriteLine(saltPostSerilizedObject);

            var client = new HttpClient();
            var DRSResponse = await client.PostAsync(Constant.AccountSaltUrl, saltPostContent);
            var DRSMessage = await DRSResponse.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine(DRSMessage);

            AccountSalt userInformation = null;

            if (DRSResponse.IsSuccessStatusCode)
            {
                var result = await DRSResponse.Content.ReadAsStringAsync();

                AcountSaltCredentials data = new AcountSaltCredentials();
                data = JsonConvert.DeserializeObject<AcountSaltCredentials>(result);

                //if (DRSMessage.Contains(Constant.UseSocialMediaLogin))
                //{
                //    Debug.WriteLine("check if social login already exists for this email");
                //    createAccount = true;
                //    System.Diagnostics.Debug.WriteLine(DRSMessage);
                //    await DisplayAlert("Oops!", data.message, "OK");
                //}
                //else if (DRSMessage.Contains(Constant.EmailNotFound))
                //{
                //    await DisplayAlert("Oops!", "Our records show that you don't have an accout. Please sign up!", "OK");
                //}
                //else
                //{
                //    userInformation = new AccountSalt
                //    {
                //        password_algorithm = data.result[0].password_algorithm,
                //        password_salt = data.result[0].password_salt
                //    };
                //}
                try
                {

                    userInformation = new AccountSalt
                    {
                        password_algorithm = data.result[0].password_algorithm,
                        password_salt = data.result[0].password_salt
                    };
                }
                catch
                {
                    return null;
                }
            }

            return userInformation;
        }


        // GOOGLE LOGIN CLICK
        public async void googleLoginButtonClicked(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("googleLoginButtonClicked entered");

                string clientId = string.Empty;
                string redirectUri = string.Empty;

                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        clientId = Constant.GoogleiOSClientID;
                        redirectUri = Constant.GoogleRedirectUrliOS;
                        break;

                    case Device.Android:
                        clientId = Constant.GoogleAndroidClientID;
                        redirectUri = Constant.GoogleRedirectUrlAndroid;
                        break;
                }

                Console.WriteLine("after switch entered");

                var authenticator = new OAuth2Authenticator(clientId, string.Empty, Constant.GoogleScope, new Uri(Constant.GoogleAuthorizeUrl), new Uri(redirectUri), new Uri(Constant.GoogleAccessTokenUrl), null, true);
                var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();

                Console.WriteLine("after vars entered");

                authenticator.Completed += GoogleAuthenticatorCompleted;
                authenticator.Error += GoogleAuthenticatorError;

                Console.WriteLine("after completed/error entered");

                AuthenticationState.Authenticator = authenticator;

                Console.WriteLine("before Login entered");
                presenter.Login(authenticator);
                Console.WriteLine("after Login entered");
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        private async void GoogleAuthenticatorCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            try
            {
                Console.WriteLine("googleAuthenticatorCompleted entered");
                //Application.Current.MainPage = new Landing("", "", "");

                var authenticator = sender as OAuth2Authenticator;

                if (authenticator != null)
                {
                    authenticator.Completed -= GoogleAuthenticatorCompleted;
                    authenticator.Error -= GoogleAuthenticatorError;
                }

                Console.WriteLine("Authenticator authenticated:" + e.IsAuthenticated);

                if (e.IsAuthenticated)
                {
                    GoogleUserProfileAsync(e.Account.Properties["access_token"], e.Account.Properties["refresh_token"], e);
                }
                else
                {
                    await DisplayAlert("Error", "Google was not able to autheticate your account", "OK");
                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        public async void GoogleUserProfileAsync(string accessToken, string refreshToken, AuthenticatorCompletedEventArgs e)
        {
            try
            {
                Console.WriteLine("googleUserProfileAsync entered");

                //testing with loading page
                //Application.Current.MainPage = new Loading();

                var client = new HttpClient();
                var socialLogInPost = new SocialLogInPost();
                //generic url
                var request = new OAuth2Request("GET", new Uri(Constant.GoogleUserInfoUrl), null, e.Account);
                var GoogleResponse = await request.GetResponseAsync();
                Debug.WriteLine("google response: " + GoogleResponse);
                var userData = GoogleResponse.GetResponseText();
                Debug.WriteLine("user Data: " + userData);
                //Application.Current.MainPage = new NavigationPage(new Loading());

                System.Diagnostics.Debug.WriteLine(userData);
                GoogleResponse googleData = JsonConvert.DeserializeObject<GoogleResponse>(userData);
                Debug.WriteLine("googleData: " + googleData);
                socialLogInPost.email = googleData.email;
                socialLogInPost.password = "";
                socialLogInPost.social_id = googleData.id;
                socialLogInPost.signup_platform = "GOOGLE";

                var socialLogInPostSerialized = JsonConvert.SerializeObject(socialLogInPost);
                var postContent = new StringContent(socialLogInPostSerialized, Encoding.UTF8, "application/json");

                System.Diagnostics.Debug.WriteLine(socialLogInPostSerialized);

                var RDSResponse = await client.PostAsync(Constant.SocialLogInUrl, postContent);
                var responseContent = await RDSResponse.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine(responseContent);
                System.Diagnostics.Debug.WriteLine(RDSResponse.IsSuccessStatusCode);

                if (RDSResponse.IsSuccessStatusCode)
                {
                    if (responseContent != null)
                    {
                        var data5 = JsonConvert.DeserializeObject<SuccessfulSocialLogIn>(responseContent);
                        //if (responseContent.Contains(Constant.EmailNotFound))
                        if (data5.code.ToString() == Constant.EmailNotFound)
                        {
                            //testing with loading page
                            Application.Current.MainPage = new MainPage();

                            var signUp = await Application.Current.MainPage.DisplayAlert("Message", "It looks like you don't have a Serving Now account. Please sign up!", "OK", "Cancel");
                            if (signUp)
                            {
                                // HERE YOU NEED TO SUBSTITUTE MY SOCIAL SIGN UP PAGE WITH PM SOCIAL SIGN UP
                                // NOTE THAT THIS SOCIAL SIGN UP PAGE NEEDS A CONSTRUCTOR LIKE THE FOLLOWING ONE
                                // SocialSignUp(string socialId, string firstName, string lastName, string emailAddress, string accessToken, string refreshToken, string platform)
                                Dictionary<string, string> info = new Dictionary<string, string>();
                                info["first_name"] = googleData.given_name;
                                info["last_name"] = googleData.family_name;
                                info["id"] = googleData.id;
                                info["email"] = googleData.email;
                                info["access_token"] = accessToken;
                                info["refresh_token"] = refreshToken;
                                //Application.Current.MainPage = new CarlosSocialSignUp(googleData.id, googleData.given_name, googleData.family_name, googleData.email, accessToken, refreshToken, "GOOGLE");
                                Application.Current.MainPage = new Registration("GOOGLE", info);
                            }
                        }
                        //else if (responseContent.Contains(Constant.AutheticatedSuccesful))
                        else if (data5.code.ToString() == Constant.AutheticatedSuccesful)
                        {
                            var loginResponse = JsonConvert.DeserializeObject<LogInResponse>(responseContent);
                            Debug.WriteLine("logged in with this id: " + loginResponse.result[0].customer_uid);
                            Debug.WriteLine("user name: " + googleData.given_name + " " + googleData.family_name);
                            Debug.WriteLine("user email: " + googleData.email);
                            Application.Current.Properties["user_id"] = loginResponse.result[0].customer_uid;
                            DateTime today = DateTime.Now;
                            DateTime expDate = today.AddDays(Constant.days);

                            Application.Current.Properties["time_stamp"] = expDate;
                            Application.Current.Properties["platform"] = "GOOGLE";

                            Application.Current.MainPage = new NavigationPage(new Filter());

                            //testing with loading page
                            //Application.Current.MainPage = new Loading();

                            //var data = JsonConvert.DeserializeObject<SuccessfulSocialLogIn>(responseContent);
                            //Debug.WriteLine("responseContent: " + responseContent.ToString());
                            //Debug.WriteLine("data: " + data.ToString());
                            //Application.Current.Properties["user_id"] = data.result[0].customer_uid;

                            //UpdateTokensPost updateTokesPost = new UpdateTokensPost();
                            //updateTokesPost.uid = data.result[0].customer_uid;
                            //updateTokesPost.mobile_access_token = accessToken;
                            //updateTokesPost.mobile_refresh_token = refreshToken;

                            //var updateTokesPostSerializedObject = JsonConvert.SerializeObject(updateTokesPost);
                            //var updateTokesContent = new StringContent(updateTokesPostSerializedObject, Encoding.UTF8, "application/json");
                            //var updateTokesResponse = await client.PostAsync(Constant.UpdateTokensUrl, updateTokesContent);
                            //var updateTokenResponseContent = await updateTokesResponse.Content.ReadAsStringAsync();
                            //System.Diagnostics.Debug.WriteLine(updateTokenResponseContent);

                            //if (updateTokesResponse.IsSuccessStatusCode)
                            //{
                            //    DateTime today = DateTime.Now;
                            //    DateTime expDate = today.AddDays(Constant.days);

                            //    Application.Current.Properties["time_stamp"] = expDate;
                            //    Application.Current.Properties["platform"] = "GOOGLE";
                            //    // Application.Current.MainPage = new SubscriptionPage();


                            //    //check to see if user has already selected a meal plan before
                            //    var request2 = new HttpRequestMessage();
                            //    Console.WriteLine("user_id: " + (string)Application.Current.Properties["user_id"]);
                            //    string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=" + (string)Application.Current.Properties["user_id"];
                            //    //old db
                            //    //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=" + (string)Application.Current.Properties["user_id"];
                            //    //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + (string)Application.Current.Properties["user_id"];
                            //    //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + "100-000256";
                            //    request2.RequestUri = new Uri(url);
                            //    //request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/get_delivery_info/400-000453");
                            //    request2.Method = HttpMethod.Get;
                            //    var client2 = new HttpClient();
                            //    HttpResponseMessage response = await client.SendAsync(request2);

                            //    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            //    {

                            //        HttpContent content = response.Content;
                            //        Console.WriteLine("content: " + content);
                            //        var userString = await content.ReadAsStringAsync();
                            //        Console.WriteLine(userString.ToString());

                            //        //writing guid to db
                            //        if (Preferences.Get("setGuid" + (string)Application.Current.Properties["user_id"], false) == false)
                            //        {

                            //            if (Device.RuntimePlatform == Device.iOS)
                            //            {
                            //                deviceId = Preferences.Get("guid", null);
                            //                if (deviceId != null) { Debug.WriteLine("This is the iOS GUID from Log in: " + deviceId); }
                            //            }
                            //            else
                            //            {
                            //                deviceId = Preferences.Get("guid", null);
                            //                if (deviceId != null) { Debug.WriteLine("This is the Android GUID from Log in " + deviceId); }
                            //            }

                            //            if (deviceId != null)
                            //            {
                            //                Debug.WriteLine("entered inside setting guid");
                            //                GuidPost notificationPost = new GuidPost();

                            //                notificationPost.uid = (string)Application.Current.Properties["user_id"];
                            //                notificationPost.guid = deviceId.Substring(5);
                            //                Application.Current.Properties["guid"] = deviceId.Substring(5);
                            //                notificationPost.notification = "TRUE";

                            //                var notificationSerializedObject = JsonConvert.SerializeObject(notificationPost);
                            //                Debug.WriteLine("Notification JSON Object to send: " + notificationSerializedObject);

                            //                var notificationContent = new StringContent(notificationSerializedObject, Encoding.UTF8, "application/json");

                            //                var clientResponse = await client.PostAsync(Constant.GuidUrl, notificationContent);

                            //                Debug.WriteLine("Status code: " + clientResponse.IsSuccessStatusCode);

                            //                if (clientResponse.IsSuccessStatusCode)
                            //                {
                            //                    System.Diagnostics.Debug.WriteLine("We have post the guid to the database");
                            //                    Preferences.Set("setGuid" + (string)Application.Current.Properties["user_id"], true);
                            //                }
                            //                else
                            //                {
                            //                    await DisplayAlert("Ooops!", "Something went wrong. We are not able to send you notification at this moment", "OK");
                            //                }
                            //            }
                            //        }
                            //        //written


                            //        //testing

                            //        JObject info_obj = JObject.Parse(userString);
                            //        //this.NewMainPage.Clear();

                            //        //ArrayList item_price = new ArrayList();
                            //        //ArrayList num_items = new ArrayList();
                            //        //ArrayList payment_frequency = new ArrayList();
                            //        //ArrayList groupArray = new ArrayList();

                            //        //int counter = 0;
                            //        //Console.WriteLine("testing: " + ((info_obj["result"]).Count().ToString()));
                            //        //Console.WriteLine("testing: " + ((info_obj["result"]).Last().ToString()));
                            //        //while (((info_obj["result"])[counter]) != null)
                            //        //{
                            //        //    Console.WriteLine("worked" + counter);
                            //        //    counter++;
                            //        //}

                            //        //check if the user hasn't entered any info before, if so put in the placeholders

                            //        Console.WriteLine("string: " + (info_obj["result"]).ToString());
                            //        //check if the user hasn't entered any info before, if so put in the placeholders
                            //        if ((info_obj["result"]).ToString() == "[]" || (info_obj["result"]).ToString() == "204" || (info_obj["result"]).ToString().Contains("ACTIVE") == false)
                            //        {
                            //            url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];

                            //            //old db
                            //            //url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                            //            var request3 = new HttpRequestMessage();
                            //            request3.RequestUri = new Uri(url);
                            //            request3.Method = HttpMethod.Get;
                            //            response = await client.SendAsync(request3);
                            //            content = response.Content;
                            //            Console.WriteLine("content: " + content);
                            //            userString = await content.ReadAsStringAsync();
                            //            JObject info_obj2 = JObject.Parse(userString);
                            //            //this.NewMainPage.Clear();
                            //            Console.WriteLine("google first: " + (info_obj2["result"])[0]["customer_first_name"].ToString());
                            //            Console.WriteLine("google last: " + (info_obj2["result"])[0]["customer_last_name"].ToString());
                            //            Console.WriteLine("google email: " + (info_obj2["result"])[0]["customer_email"].ToString());
                            //            Preferences.Set("user_latitude", (info_obj2["result"])[0]["customer_lat"].ToString());
                            //            Debug.WriteLine("user latitude" + Preferences.Get("user_latitude", ""));
                            //            Preferences.Set("user_longitude", (info_obj2["result"])[0]["customer_long"].ToString());
                            //            Debug.WriteLine("user longitude" + Preferences.Get("user_longitude", ""));

                            //            Uri apiRequestUri = new Uri("https://www.googleapis.com/oauth2/v2/userinfo?access_token=" + (info_obj2["result"])[0]["mobile_access_token"].ToString());
                            //            //request profile image
                            //            using (var webClient = new System.Net.WebClient())
                            //            {
                            //                var json = webClient.DownloadString(apiRequestUri);
                            //                var data2 = JsonConvert.DeserializeObject<profilePicLogIn>(json);
                            //                Debug.WriteLine(data2.ToString());
                            //                var userPicture = data2.picture;
                            //                //var holder = userPicture[0];
                            //                Debug.WriteLine(userPicture);
                            //                Preferences.Set("profilePicLink", userPicture);

                            //                //var data = JsonConvert.DeserializeObject<SuccessfulSocialLogIn>(responseContent);
                            //                //Application.Current.Properties["user_id"] = data.result[0].customer_uid;
                            //            }

                            //            Console.WriteLine("go to SubscriptionPage");
                            //            DisplayAlert("navigation", "sending to subscription", "close");
                            //            Preferences.Set("canChooseSelect", false);
                            //            await Application.Current.SavePropertiesAsync();
                            //            Application.Current.MainPage = new NavigationPage(new SubscriptionPage((info_obj2["result"])[0]["customer_first_name"].ToString(), (info_obj2["result"])[0]["customer_last_name"].ToString(), (info_obj2["result"])[0]["customer_email"].ToString()));
                            //        }
                            //        else
                            //        {
                            //            url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];

                            //            //old db
                            //            //url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                            //            var request3 = new HttpRequestMessage();
                            //            request3.RequestUri = new Uri(url);
                            //            request3.Method = HttpMethod.Get;
                            //            response = await client.SendAsync(request3);
                            //            content = response.Content;
                            //            Console.WriteLine("content: " + content);
                            //            userString = await content.ReadAsStringAsync();
                            //            JObject info_obj2 = JObject.Parse(userString);
                            //            //this.NewMainPage.Clear();
                            //            Preferences.Set("user_latitude", (info_obj2["result"])[0]["customer_lat"].ToString());
                            //            Debug.WriteLine("user latitude" + Preferences.Get("user_latitude", ""));
                            //            Preferences.Set("user_longitude", (info_obj2["result"])[0]["customer_long"].ToString());
                            //            Debug.WriteLine("user longitude" + Preferences.Get("user_longitude", ""));
                            //            Console.WriteLine("google first: " + (info_obj2["result"])[0]["customer_first_name"].ToString());
                            //            Console.WriteLine("google last: " + (info_obj2["result"])[0]["customer_last_name"].ToString());
                            //            Console.WriteLine("google email: " + (info_obj2["result"])[0]["customer_email"].ToString());
                            //            Debug.WriteLine("user access token: " + (info_obj2["result"])[0]["user_access_token"].ToString());
                            //            Debug.WriteLine("mobile access token: " + (info_obj2["result"])[0]["mobile_access_token"].ToString());
                            //            Uri apiRequestUri = new Uri("https://www.googleapis.com/oauth2/v2/userinfo?access_token=" + (info_obj2["result"])[0]["mobile_access_token"].ToString());
                            //            //request profile image
                            //            using (var webClient = new System.Net.WebClient())
                            //            {
                            //                var json = webClient.DownloadString(apiRequestUri);
                            //                var data2 = JsonConvert.DeserializeObject<profilePicLogIn>(json);
                            //                Debug.WriteLine(data2.ToString());
                            //                var userPicture = data2.picture;
                            //                //var holder = userPicture[0];
                            //                Debug.WriteLine(userPicture);
                            //                Preferences.Set("profilePicLink", userPicture);

                            //                //var data = JsonConvert.DeserializeObject<SuccessfulSocialLogIn>(responseContent);
                            //                //Application.Current.Properties["user_id"] = data.result[0].customer_uid;
                            //            }

                            //            DisplayAlert("navigation", "sending to select", "close");
                            //            Console.WriteLine("delivery first name: " + (info_obj["result"])[0]["delivery_first_name"].ToString());
                            //            Console.WriteLine("delivery last name: " + (info_obj["result"])[0]["delivery_last_name"].ToString());
                            //            Console.WriteLine("delivery email: " + (info_obj["result"])[0]["delivery_email"].ToString());
                            //            Preferences.Set("canChooseSelect", true);
                            //            //await Debug.WriteLine("a");
                            //            //navToSelect((info_obj2["result"])[0]["customer_first_name"].ToString(), (info_obj2["result"])[0]["customer_last_name"].ToString(), (info_obj2["result"])[0]["customer_email"].ToString());
                            //            Zones[] zones = new Zones[] { };
                            //            await Application.Current.SavePropertiesAsync();
                            //            //Application.Current.MainPage = new NavigationPage(new Select(zones, (info_obj2["result"])[0]["customer_first_name"].ToString(), (info_obj2["result"])[0]["customer_last_name"].ToString(), (info_obj2["result"])[0]["customer_email"].ToString()));
                            //            //Application.Current.MainPage = new NavigationPage(new CongratsPage());
                            //            //Application.Current.MainPage = new NavigationPage(new SubscriptionPage((info_obj2["result"])[0]["customer_first_name"].ToString(), (info_obj2["result"])[0]["customer_last_name"].ToString(), (info_obj2["result"])[0]["customer_email"].ToString()));
                            //            //Application.Current.MainPage = new NavigationPage(new SubscriptionPage((info_obj2["result"])[0]["customer_first_name"].ToString(), (info_obj2["result"])[0]["customer_last_name"].ToString(), (info_obj2["result"])[0]["customer_email"].ToString()));
                            //        }

                            //    }

                            //    // THIS IS HOW YOU CAN ACCESS YOUR USER ID FROM THE APP
                            //    // string userID = (string)Application.Current.Properties["user_id"];
                            //}
                            //else
                            //{
                            //    //testing with loading page
                            //    Application.Current.MainPage = new MainPage();

                            //    await Application.Current.MainPage.DisplayAlert("Oops", "We are facing some problems with our internal system. We weren't able to update your credentials", "OK");
                            //}
                        }
                        //else if (responseContent.Contains(Constant.ErrorPlatform))
                        else if (data5.code.ToString() == Constant.ErrorPlatform)
                        {
                            Debug.WriteLine("google login: check if the user's email is already used elsewhere");
                            //testing with loading page
                            Application.Current.MainPage = new MainPage();

                            var RDSCode = JsonConvert.DeserializeObject<RDSLogInMessage>(responseContent);
                            await Application.Current.MainPage.DisplayAlert("Message", RDSCode.message, "OK");
                        }
                        //else if (responseContent.Contains(Constant.ErrorUserDirectLogIn))
                        else if (data5.code.ToString() == Constant.ErrorUserDirectLogIn)
                        {

                            var data = JsonConvert.DeserializeObject<SuccessfulSocialLogIn>(responseContent);
                            Debug.WriteLine("responseContent direct login: " + responseContent.ToString());
                            //testing with loading page
                            //await Navigation.PopAsync();
                            Application.Current.MainPage = new MainPage();
                            //Navigation.RemovePage(this.Navigation.NavigationStack[0]);

                            await Application.Current.MainPage.DisplayAlert("Oops!", "You have an existing Serving Now account. Please use direct login", "OK");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        private async void GoogleAuthenticatorError(object sender, AuthenticatorErrorEventArgs e)
        {
            try
            {
                Console.WriteLine("googleAuthenticatorError entered");

                var authenticator = sender as OAuth2Authenticator;

                if (authenticator != null)
                {
                    authenticator.Completed -= GoogleAuthenticatorCompleted;
                    authenticator.Error -= GoogleAuthenticatorError;
                }

                await DisplayAlert("Authentication error: ", e.Message, "OK");
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        // FACEBOOK LOGIN CLICK
        public async void facebookLoginButtonClicked(object sender, EventArgs e)
        {
            try
            {
                // Initialize variables
                string clientID = string.Empty;
                string redirectURL = string.Empty;

                switch (Device.RuntimePlatform)
                {
                    // depending on the device, get constants from Login>Constants>Constants.cs file
                    case Device.iOS:
                        clientID = Constant.FacebookiOSClientID;
                        redirectURL = Constant.FacebookiOSRedirectUrl;
                        break;
                    case Device.Android:
                        clientID = Constant.FacebookAndroidClientID;
                        redirectURL = Constant.FacebookAndroidRedirectUrl;
                        break;
                }

                // Store all the information in a variable called authenticator (for client) and presenter for http client (who is going to present the credentials)
                var authenticator = new OAuth2Authenticator(clientID, Constant.FacebookScope, new Uri(Constant.FacebookAuthorizeUrl), new Uri(redirectURL), null, false);
                var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();

                // Creates Completed and Error Event Handler functions;  "+=" means create
                authenticator.Completed += FacebookAuthenticatorCompleted;
                authenticator.Error += FacebookAutheticatorError;


                // This is the actual call to Facebook
                presenter.Login(authenticator);
                // Facebooks sends back an authenticator that goes directly into the Event Handlers created above as "sender".  Data is stored in arguement "e" (account, user name, access token, etc).
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }




        // sender contains nothing then there is an error.  sender contains an authenticator from Facebook
        public async void FacebookAuthenticatorCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            try
            {
                var authenticator = sender as OAuth2Authenticator;
                Console.WriteLine("authenticator" + authenticator.ToString());
                if (authenticator != null)
                {
                    // Removes Event Handler functions;  "-=" means delete
                    authenticator.Completed -= FacebookAuthenticatorCompleted;
                    authenticator.Error -= FacebookAutheticatorError;
                }

                if (e.IsAuthenticated)
                {
                    // Uses access token from Facebook as an input to FacebookUserProfileAsync
                    FacebookUserProfileAsync(e.Account.Properties["access_token"]);
                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        public async void FacebookUserProfileAsync(string accessToken)
        {
            try
            {
                //email might not be found so loading page can't be here forever
                //testing with loading page FB
                Application.Current.MainPage = new Loading();
                var client = new HttpClient();

                var socialLogInPost = new SocialLogInPost();

                // Actual call to Facebooks end point now that we have the token (appending accessToken to URL in constants file)
                var facebookResponse = client.GetStringAsync(Constant.FacebookUserInfoUrl + accessToken);  // makes the call to Facebook and returns True/False
                var userData = facebookResponse.Result;  // returns Facebook email and social ID

                System.Diagnostics.Debug.WriteLine(facebookResponse);
                System.Diagnostics.Debug.WriteLine(userData);


                // Deserializes JSON object from info provided by Facebook
                FacebookResponse facebookData = JsonConvert.DeserializeObject<FacebookResponse>(userData);
                socialLogInPost.email = facebookData.email;
                socialLogInPost.password = "";
                socialLogInPost.social_id = facebookData.id;
                socialLogInPost.signup_platform = "FACEBOOK";

                // Create JSON object for Login Endpoint
                var socialLogInPostSerialized = JsonConvert.SerializeObject(socialLogInPost);
                var postContent = new StringContent(socialLogInPostSerialized, Encoding.UTF8, "application/json");

                System.Diagnostics.Debug.WriteLine(socialLogInPostSerialized);

                // Call to RDS database with endpoint and JSON data
                var RDSResponse = await client.PostAsync(Constant.LogInUrl, postContent);  //  True or False if Parva's endpoint ran preperly.
                var responseContent = await RDSResponse.Content.ReadAsStringAsync();  // Contains Parva's code containing all the user data including userid

                System.Diagnostics.Debug.WriteLine(RDSResponse.IsSuccessStatusCode);  // Response code is Yes/True if successful from httpclient system.net package
                System.Diagnostics.Debug.WriteLine(responseContent);  // Response JSON that RDS returns

                if (RDSResponse.IsSuccessStatusCode)
                {
                    if (responseContent != null)
                    {
                        var data5 = JsonConvert.DeserializeObject<SuccessfulSocialLogIn>(responseContent);
                        // Do I don't have the email in RDS
                        if (data5.code.ToString() == Constant.EmailNotFound)
                        {
                            //testing with loading page
                            Application.Current.MainPage = new MainPage();


                            var signUp = await Application.Current.MainPage.DisplayAlert("Message", "It looks like you don't have a PM account. Please sign up!", "OK", "Cancel");
                            if (signUp)
                            {
                                // HERE YOU NEED TO SUBSTITUTE MY SOCIAL SIGN UP PAGE WITH PM SOCIAL SIGN UP
                                // NOTE THAT THIS SOCIAL SIGN UP PAGE NEEDS A CONSTRUCTOR LIKE THE FOLLOWING ONE
                                // SocialSignUp(string socialId, string firstName, string lastName, string emailAddress, string accessToken, string refreshToken, string platform)
                                Preferences.Set("canChooseSelect", false);
                                Preferences.Set("profilePicLink", facebookData.picture.data.url);
                                Debug.WriteLine("fb profile pic:" + facebookData.picture.data.url);

                                Application.Current.MainPage = new CarlosSocialSignUp(facebookData.id, facebookData.name, "", facebookData.email, accessToken, accessToken, "FACEBOOK");
                                // need to write new statment here ...
                            }
                        }


                        // if Response content contains 200
                        else if (data5.code.ToString() == Constant.AutheticatedSuccesful)
                        {
                            var data = JsonConvert.DeserializeObject<SuccessfulSocialLogIn>(responseContent);
                            Application.Current.Properties["user_id"] = data.result[0].customer_uid;  // converts RDS data into appication data.

                            UpdateTokensPost updateTokensPost = new UpdateTokensPost();
                            updateTokensPost.uid = data.result[0].customer_uid;
                            updateTokensPost.mobile_access_token = accessToken;
                            updateTokensPost.mobile_refresh_token = accessToken;  // only get access token from Facebook so we store the data again

                            var updateTokensPostSerializedObject = JsonConvert.SerializeObject(updateTokensPost);
                            var updateTokensContent = new StringContent(updateTokensPostSerializedObject, Encoding.UTF8, "application/json");
                            var updateTokensResponse = await client.PostAsync(Constant.UpdateTokensUrl, updateTokensContent);  // This calls the database and returns True or False
                            var updateTokenResponseContent = await updateTokensResponse.Content.ReadAsStringAsync();
                            System.Diagnostics.Debug.WriteLine(updateTokenResponseContent);

                            if (updateTokensResponse.IsSuccessStatusCode)
                            {
                                DateTime today = DateTime.Now;
                                DateTime expDate = today.AddDays(Constant.days);  // Internal assignment - not from the database

                                Application.Current.Properties["time_stamp"] = expDate;
                                Application.Current.Properties["platform"] = "FACEBOOK";
                                // Application.Current.MainPage = new SubscriptionPage();


                                //check to see if user has already selected a meal plan before
                                var request2 = new HttpRequestMessage();
                                Console.WriteLine("user_id: " + (string)Application.Current.Properties["user_id"]);
                                string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=" + (string)Application.Current.Properties["user_id"];

                                //old db
                                //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=" + (string)Application.Current.Properties["user_id"]; 
                                //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + (string)Application.Current.Properties["user_id"];
                                //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + "100-000256";
                                request2.RequestUri = new Uri(url);
                                //request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/get_delivery_info/400-000453");
                                request2.Method = HttpMethod.Get;
                                var client2 = new HttpClient();
                                HttpResponseMessage response = await client.SendAsync(request2);


                                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                {

                                    HttpContent content = response.Content;
                                    Console.WriteLine("content: " + content);
                                    var userString = await content.ReadAsStringAsync();
                                    //Console.WriteLine(userString);


                                    //writing guid to db
                                    if (Preferences.Get("setGuid" + (string)Application.Current.Properties["user_id"], false) == false)
                                    {
                                        if (Device.RuntimePlatform == Device.iOS)
                                        {
                                            deviceId = Preferences.Get("guid", null);
                                            if (deviceId != null) { Debug.WriteLine("This is the iOS GUID from Log in: " + deviceId); }
                                        }
                                        else
                                        {
                                            deviceId = Preferences.Get("guid", null);
                                            if (deviceId != null) { Debug.WriteLine("This is the Android GUID from Log in " + deviceId); }
                                        }

                                        if (deviceId != null)
                                        {
                                            GuidPost notificationPost = new GuidPost();

                                            notificationPost.uid = (string)Application.Current.Properties["user_id"];
                                            notificationPost.guid = deviceId.Substring(5);
                                            Application.Current.Properties["guid"] = deviceId.Substring(5);
                                            notificationPost.notification = "TRUE";

                                            var notificationSerializedObject = JsonConvert.SerializeObject(notificationPost);
                                            Debug.WriteLine("Notification JSON Object to send: " + notificationSerializedObject);

                                            var notificationContent = new StringContent(notificationSerializedObject, Encoding.UTF8, "application/json");

                                            var clientResponse = await client.PostAsync(Constant.GuidUrl, notificationContent);

                                            Debug.WriteLine("Status code: " + clientResponse.IsSuccessStatusCode);

                                            if (clientResponse.IsSuccessStatusCode)
                                            {
                                                System.Diagnostics.Debug.WriteLine("We have post the guid to the database");
                                                Preferences.Set("setGuid" + (string)Application.Current.Properties["user_id"], true);
                                            }
                                            else
                                            {
                                                await DisplayAlert("Ooops!", "Something went wrong. We are not able to send you notification at this moment", "OK");
                                            }
                                        }
                                    }
                                    //written



                                    if (userString.ToString()[0] != '{')
                                    {
                                        url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];

                                        //old db
                                        //url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                                        var request3 = new HttpRequestMessage();
                                        request3.RequestUri = new Uri(url);
                                        request3.Method = HttpMethod.Get;
                                        response = await client.SendAsync(request3);
                                        content = response.Content;
                                        Console.WriteLine("content: " + content);
                                        userString = await content.ReadAsStringAsync();
                                        JObject info_obj2 = JObject.Parse(userString);
                                        //this.NewMainPage.Clear();
                                        Preferences.Set("user_latitude", (info_obj2["result"])[0]["customer_lat"].ToString());
                                        Debug.WriteLine("user latitude" + Preferences.Get("user_latitude", ""));
                                        Preferences.Set("user_longitude", (info_obj2["result"])[0]["customer_long"].ToString());
                                        Debug.WriteLine("user longitude" + Preferences.Get("user_longitude", ""));

                                        Preferences.Set("profilePicLink", facebookData.picture.data.url);
                                        Debug.WriteLine("fb profile pic:" + facebookData.picture.data.url);

                                        Console.WriteLine("go to SubscriptionPage");
                                        Preferences.Set("canChooseSelect", false);
                                        await Application.Current.SavePropertiesAsync();
                                        Application.Current.MainPage = new NavigationPage(new SubscriptionPage((info_obj2["result"])[0]["customer_first_name"].ToString(), (info_obj2["result"])[0]["customer_last_name"].ToString(), (info_obj2["result"])[0]["customer_email"].ToString()));
                                        return;
                                    }

                                    JObject info_obj = JObject.Parse(userString);
                                    //this.NewMainPage.Clear();

                                    //ArrayList item_price = new ArrayList();
                                    //ArrayList num_items = new ArrayList();
                                    //ArrayList payment_frequency = new ArrayList();
                                    //ArrayList groupArray = new ArrayList();

                                    Console.WriteLine("string: " + (info_obj["result"]).ToString());
                                    Debug.WriteLine("what is canChooseSelect set to?: " + Preferences.Get("canChooseSelect", false).ToString());
                                    //check if the user hasn't entered any info before, if so put in the placeholders
                                    if ((info_obj["result"]).ToString() == "[]" || (info_obj["result"]).ToString().Contains("ACTIVE") == false)
                                    {
                                        url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];

                                        //old db
                                        //url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                                        var request3 = new HttpRequestMessage();
                                        request3.RequestUri = new Uri(url);
                                        request3.Method = HttpMethod.Get;
                                        response = await client.SendAsync(request3);
                                        content = response.Content;
                                        Console.WriteLine("content: " + content);
                                        userString = await content.ReadAsStringAsync();
                                        JObject info_obj2 = JObject.Parse(userString);
                                        //this.NewMainPage.Clear();
                                        Preferences.Set("user_latitude", (info_obj2["result"])[0]["customer_lat"].ToString());
                                        Debug.WriteLine("user latitude" + Preferences.Get("user_latitude", ""));
                                        Preferences.Set("user_longitude", (info_obj2["result"])[0]["customer_long"].ToString());
                                        Debug.WriteLine("user longitude" + Preferences.Get("user_longitude", ""));

                                        Preferences.Set("profilePicLink", facebookData.picture.data.url);
                                        Debug.WriteLine("fb profile pic:" + facebookData.picture.data.url);

                                        Console.WriteLine("go to SubscriptionPage");
                                        Preferences.Set("canChooseSelect", false);
                                        await Application.Current.SavePropertiesAsync();
                                        Application.Current.MainPage = new NavigationPage(new SubscriptionPage((info_obj2["result"])[0]["customer_first_name"].ToString(), (info_obj2["result"])[0]["customer_last_name"].ToString(), (info_obj2["result"])[0]["customer_email"].ToString()));
                                    }
                                    else
                                    {
                                        url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];

                                        //old db
                                        //url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                                        var request3 = new HttpRequestMessage();
                                        request3.RequestUri = new Uri(url);
                                        request3.Method = HttpMethod.Get;
                                        response = await client.SendAsync(request3);
                                        content = response.Content;
                                        Console.WriteLine("content: " + content);
                                        userString = await content.ReadAsStringAsync();
                                        JObject info_obj2 = JObject.Parse(userString);
                                        //this.NewMainPage.Clear();
                                        Preferences.Set("user_latitude", (info_obj2["result"])[0]["customer_lat"].ToString());
                                        Debug.WriteLine("user latitude" + Preferences.Get("user_latitude", ""));
                                        Preferences.Set("user_longitude", (info_obj2["result"])[0]["customer_long"].ToString());
                                        Debug.WriteLine("user longitude" + Preferences.Get("user_longitude", ""));

                                        Preferences.Set("profilePicLink", facebookData.picture.data.url);
                                        Debug.WriteLine("fb profile pic:" + facebookData.picture.data.url);

                                        Preferences.Set("canChooseSelect", true);
                                        Zones[] zones = new Zones[] { };
                                        await Application.Current.SavePropertiesAsync();
                                        //Application.Current.MainPage = new NavigationPage(new Select(zones, (info_obj2["result"])[0]["customer_first_name"].ToString(), (info_obj2["result"])[0]["customer_last_name"].ToString(), (info_obj2["result"])[0]["customer_email"].ToString()));
                                    }
                                }

                                // THIS IS HOW YOU CAN ACCESS YOUR USER ID FROM THE APP
                                //string userID = (string)Application.Current.Properties["user_id"];
                                //printing id for testing
                                //System.Diagnostics.Debug.WriteLine("user ID after success: " + userID);
                            }
                            else
                            {
                                //testing with loading page
                                Application.Current.MainPage = new MainPage();

                                await Application.Current.MainPage.DisplayAlert("Oops", "We are facing some problems with our internal system. We weren't able to update your credentials", "OK");
                            }
                        }

                        // Wrong Platform message
                        else if (data5.code.ToString() == Constant.ErrorPlatform)
                        {
                            //testing with loading page
                            Application.Current.MainPage = new MainPage();

                            var RDSCode = JsonConvert.DeserializeObject<RDSLogInMessage>(responseContent);
                            await Application.Current.MainPage.DisplayAlert("Message", RDSCode.message, "OK");
                        }


                        // Wrong LOGIN method message
                        else if (data5.code.ToString() == Constant.ErrorUserDirectLogIn)
                        {
                            //testing with loading page
                            Application.Current.MainPage = new MainPage();

                            await Application.Current.MainPage.DisplayAlert("Oops!", "You have an existing PM account. Please use direct login", "OK");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }



        private async void FacebookAutheticatorError(object sender, AuthenticatorErrorEventArgs e)
        {
            try
            {
                var authenticator = sender as OAuth2Authenticator;
                if (authenticator != null)
                {
                    authenticator.Completed -= FacebookAuthenticatorCompleted;
                    authenticator.Error -= FacebookAutheticatorError;
                }

                await DisplayAlert("Authentication error: ", e.Message, "OK");
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        // APPLE LOGIN CLICK
        public async void appleLoginButtonClicked(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("appleLogin clicked");

                SignIn?.Invoke(sender, e);
                var c = (ImageButton)sender;
                Console.WriteLine("appleLogin c: " + c.ToString());
                c.Command?.Execute(c.CommandParameter);

                //testing
                var testingVar = new LoginViewModel();
                //testingVar.OnAppleSignInRequest();
                vm.OnAppleSignInRequest();
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        public void InvokeSignInEvent(object sender, EventArgs e)
            => SignIn?.Invoke(sender, e);

        private async void AppleError(object sender, EventArgs e)
        {
            await DisplayAlert("Error", "We weren't able to set an account for you", "OK");
        }
    }
}
