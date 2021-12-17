using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using PM.Constants;
using PM.Model;
using PM.Model.Login.LoginClasses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Auth;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PM.ViewModel
{
    public partial class CreatePassword : ContentPage
    {
        Dictionary<string, string> signUpInput;

        public CreatePassword(Dictionary<string, string> input)
        {
            signUpInput = input;
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;
            Console.WriteLine("Width = " + width.ToString());
            Console.WriteLine("Height = " + height.ToString());

            InitializeComponent();
        }

        async void registrationClicked(System.Object sender, System.EventArgs e)
        {
            if (passEntry.Text == null || confirmPassEntry.Text == null)
            {
                await DisplayAlert("Oops", "Fill all of the fields before continuing.", "OK");
                return;
            }
            else if (passEntry.Text.ToString().Length < 8)
            {
                await DisplayAlert("Oops", "Your password must be at least 8 characters long.", "OK");
                return;
            }
            else if (passEntry.Text != confirmPassEntry.Text)
            {
                await DisplayAlert("Oops", "Your passwords don't match", "OK");
                return;
            }
            else
            {
                SignUpPost signUpObj = new SignUpPost();
                signUpObj.email = signUpInput["email"];
                signUpObj.first_name = signUpInput["first_name"];
                signUpObj.last_name = signUpInput["last_name"];
                signUpObj.phone_number = signUpInput["phone"];
                signUpObj.id_type = signUpInput["id_type"];
                signUpObj.id_number = signUpInput["id_num"];
                signUpObj.address = signUpInput["address"];
                signUpObj.unit = signUpInput["unit"];
                signUpObj.city = signUpInput["city"];
                signUpObj.state = signUpInput["state"];
                signUpObj.zip_code = signUpInput["zip"];
                signUpObj.latitude = signUpInput["latitude"];
                signUpObj.longitude = signUpInput["longitude"];
                signUpObj.referral_source = "MOBILE";
                signUpObj.affiliation = signUpInput["affiliation"];
                signUpObj.role = "CUSTOMER";
                signUpObj.social = "NULL";
                signUpObj.password = passEntry.Text.Trim();
                signUpObj.mobile_access_token = "FALSE";
                signUpObj.mobile_refresh_token = "FALSE";
                signUpObj.user_access_token = "FALSE";
                signUpObj.user_refresh_token = "FALSE";
                signUpObj.social_id = "NULL";
                signUpObj.cust_id = "";

                var directSignUpSerializedObject = JsonConvert.SerializeObject(signUpObj);
                var content = new StringContent(directSignUpSerializedObject, Encoding.UTF8, "application/json");
                System.Diagnostics.Debug.WriteLine("serialized sign up obj: " +  directSignUpSerializedObject);

                var signUpClient = new HttpClient();
                var RDSResponse = await signUpClient.PostAsync(Constant.SignUpUrl, content);
                Debug.WriteLine("RDSResponse for direct signup: " + RDSResponse.ToString());
                var RDSMessage = await RDSResponse.Content.ReadAsStringAsync();
                Debug.WriteLine("RDSMessage: " + RDSMessage.ToString());

                if (!RDSMessage.Contains("Email address has already been taken"))
                {
                    var result = await RDSResponse.Content.ReadAsStringAsync();

                    DirectSignUpResponse data = new DirectSignUpResponse();
                    data = JsonConvert.DeserializeObject<DirectSignUpResponse>(result);

                    Application.Current.Properties["user_id"] = data.result.customer_uid;
                    Debug.WriteLine("new user's customer uid: " + data.result.customer_uid);
                    Application.Current.Properties["platform"] = "DIRECT";
                    Application.Current.MainPage = new CongratsPage();
                }
                else
                {
                    DisplayAlert("Oops", "This email address is already taken by an existing account.", "OK");
                }


                
            }
            /*
            SignUpPost newSignUp = new SignUpPost();
            newSignUp.email = "10";
            newSignUp.first_name = signUpInput["first_name"];
            newSignUp.last_name = signUpInput["last_name"];
            newSignUp.phone_number = signUpInput["phone"];
            newSignUp.address = signUpInput["address"];
            newSignUp.unit = signUpInput["unit"];
            newSignUp.city = signUpInput["city"];
            newSignUp.state = signUpInput["state"];
            newSignUp.zip_code = signUpInput["zip"];
            newSignUp.latitude = signUpInput["latitude"];
            newSignUp.longitude = signUpInput["longitude"];
            newSignUp.referral_source = "";
            newSignUp.role = "CUSTOMER";
            newSignUp.social = "DIRECT";
            newSignUp.password = passEntry.Text;
            newSignUp.mobile_access_token = "";
            newSignUp.mobile_refresh_token = "";
            newSignUp.user_access_token = "";
            newSignUp.user_refresh_token = "";
            newSignUp.social_id = "";
            newSignUp.cust_id = "";

            var directSignUpSerializedObject = JsonConvert.SerializeObject(newSignUp);
            var content = new StringContent(directSignUpSerializedObject, Encoding.UTF8, "application/json");

            System.Diagnostics.Debug.WriteLine(directSignUpSerializedObject);

            var signUpclient = new HttpClient();
            var RDSResponse = await signUpclient.PostAsync(Constant.SignUpUrl, content);
            Debug.WriteLine("RDSResponse: " + RDSResponse.ToString());
            var RDSMessage = await RDSResponse.Content.ReadAsStringAsync();
            Debug.WriteLine("RDSMessage: " + RDSMessage.ToString());

            // if Sign up is has successfully ie 200 response code
            if (RDSResponse.IsSuccessStatusCode)
            {
                try
                {
                    var RDSData = JsonConvert.DeserializeObject<SignUpResponse>(RDSMessage);
                    Debug.WriteLine("RDSData: " + RDSData.ToString());
                    DateTime today = DateTime.Now;
                    DateTime expDate = today.AddDays(Constant.days);
                    // Local Variables in Xamarin that can be used throughout the App
                    Application.Current.Properties["user_id"] = RDSData.result.customer_uid;
                    Application.Current.Properties["time_stamp"] = expDate;
                    Application.Current.Properties["platform"] = "DIRECT";
                    System.Diagnostics.Debug.WriteLine("UserID is:" + (string)Application.Current.Properties["user_id"]);
                    System.Diagnostics.Debug.WriteLine("Time Stamp is:" + Application.Current.Properties["time_stamp"].ToString());
                    System.Diagnostics.Debug.WriteLine("platform is:" + (string)Application.Current.Properties["platform"]);

                    //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                    //var request3 = new HttpRequestMessage();
                    //request3.RequestUri = new Uri(url);
                    //request3.Method = HttpMethod.Get;
                    //var client2 = new HttpClient();
                    //HttpResponseMessage response = await client2.SendAsync(request3);
                    //HttpContent content2 = response.Content;
                    //Console.WriteLine("content: " + content2);
                    //var userString = await content2.ReadAsStringAsync();
                    //JObject info_obj2 = JObject.Parse(userString);
                    //this.NewMainPage.Clear();
                    //Preferences.Set("profilePicLink", null);
                    //// Go to Subscripton page
                    //// Application.Current.MainPage = new SubscriptionPage();

                    ////send email to verify email
                    //emailVerifyPost emailVer = new emailVerifyPost();
                    //emailVer.email = emailEntry.Text.Trim();
                    //var emailVerSerializedObj = JsonConvert.SerializeObject(emailVer);
                    //var content4 = new StringContent(emailVerSerializedObj, Encoding.UTF8, "application/json");
                    //var client3 = new HttpClient();
                    //var response3 = client3.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/email_verification", content4);
                    //Console.WriteLine("RESPONSE TO CHECKOUT   " + response3.Result);
                    //Console.WriteLine("CHECKOUT JSON OBJECT BEING SENT: " + emailVerSerializedObj);

                    Application.Current.MainPage = new CongratsPage();
                }
                catch
                {
                    var RDSData = JsonConvert.DeserializeObject<SignUpExisted>(RDSMessage);
                    Debug.WriteLine("RDSData: " + RDSData.ToString());
                    if (RDSData.message.Contains("taken"))
                    {
                        DisplayAlert("Error", "email address is already in use", "OK");
                        return;
                    }
                }
                //var RDSData = JsonConvert.DeserializeObject<SignUpResponse>(RDSMessage);
                //Debug.WriteLine("RDSData: " + RDSData.ToString());
                //DateTime today = DateTime.Now;
                //DateTime expDate = today.AddDays(Constant.days);

                //if (RDSData.message.Contains("taken"))
                //{
                //    DisplayAlert("Error", "email address is already in use", "OK");
                //}
                //else
                //{
                //    // Local Variables in Xamarin that can be used throughout the App
                //    //Application.Current.Properties["user_id"] = RDSData.result.customer_uid;
                //    Application.Current.Properties["time_stamp"] = expDate;
                //    Application.Current.Properties["platform"] = "DIRECT";
                //    System.Diagnostics.Debug.WriteLine("UserID is:" + (string)Application.Current.Properties["user_id"]);
                //    System.Diagnostics.Debug.WriteLine("Time Stamp is:" + Application.Current.Properties["time_stamp"].ToString());
                //    System.Diagnostics.Debug.WriteLine("platform is:" + (string)Application.Current.Properties["platform"]);

                //    //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                //    //var request3 = new HttpRequestMessage();
                //    //request3.RequestUri = new Uri(url);
                //    //request3.Method = HttpMethod.Get;
                //    //var client2 = new HttpClient();
                //    //HttpResponseMessage response = await client2.SendAsync(request3);
                //    //HttpContent content2 = response.Content;
                //    //Console.WriteLine("content: " + content2);
                //    //var userString = await content2.ReadAsStringAsync();
                //    //JObject info_obj2 = JObject.Parse(userString);
                //    //this.NewMainPage.Clear();
                //    //Preferences.Set("profilePicLink", null);
                //    //// Go to Subscripton page
                //    //// Application.Current.MainPage = new SubscriptionPage();

                //    ////send email to verify email
                //    //emailVerifyPost emailVer = new emailVerifyPost();
                //    //emailVer.email = emailEntry.Text.Trim();
                //    //var emailVerSerializedObj = JsonConvert.SerializeObject(emailVer);
                //    //var content4 = new StringContent(emailVerSerializedObj, Encoding.UTF8, "application/json");
                //    //var client3 = new HttpClient();
                //    //var response3 = client3.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/email_verification", content4);
                //    //Console.WriteLine("RESPONSE TO CHECKOUT   " + response3.Result);
                //    //Console.WriteLine("CHECKOUT JSON OBJECT BEING SENT: " + emailVerSerializedObj);

                //    Application.Current.MainPage = new CongratsPage();
                //}
            }
            */
            

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
                                // HERE YOU NEED TO SUBSTITUTE MY SOCIAL SIGN UP PAGE WITH PM SOCIAL SIGN UP
                                // NOTE THAT THIS SOCIAL SIGN UP PAGE NEEDS A CONSTRUCTOR LIKE THE FOLLOWING ONE
                                // SocialSignUp(string socialId, string firstName, string lastName, string emailAddress, string accessToken, string refreshToken, string platform)

                                SignUpPost signUpObj = new SignUpPost();
                                signUpObj.email = googleData.email;
                                signUpObj.first_name = googleData.given_name;
                                signUpObj.last_name = googleData.family_name;
                                signUpObj.phone_number = signUpInput["phone"];
                                signUpObj.id_type = signUpInput["id_type"];
                                signUpObj.id_number = signUpInput["id_num"];
                                signUpObj.address = signUpInput["address"];
                                signUpObj.unit = signUpInput["unit"];
                                signUpObj.city = signUpInput["city"];
                                signUpObj.state = signUpInput["state"];
                                signUpObj.zip_code = signUpInput["zip"];
                                signUpObj.latitude = signUpInput["latitude"];
                                signUpObj.longitude = signUpInput["longitude"];
                                signUpObj.referral_source = "MOBILE";
                                signUpObj.role = "CUSTOMER";
                                signUpObj.social = "GOOGLE";
                                signUpObj.password = "";
                                signUpObj.mobile_access_token = accessToken;
                                signUpObj.mobile_refresh_token = refreshToken;
                                signUpObj.user_access_token = "FALSE";
                                signUpObj.user_refresh_token = "FALSE";
                                signUpObj.social_id = googleData.id;
                                signUpObj.cust_id = "";

                                var directSignUpSerializedObject = JsonConvert.SerializeObject(signUpObj);
                                var content = new StringContent(directSignUpSerializedObject, Encoding.UTF8, "application/json");
                                System.Diagnostics.Debug.WriteLine("serialized sign up obj: " + directSignUpSerializedObject);

                                var signUpClient = new HttpClient();
                                var RDSResponse2 = await signUpClient.PostAsync(Constant.SignUpUrl, content);
                                Debug.WriteLine("RDSResponse for direct signup: " + RDSResponse2.ToString());
                                var RDSMessage = await RDSResponse2.Content.ReadAsStringAsync();
                                Debug.WriteLine("RDSMessage: " + RDSMessage.ToString());

                                if (!RDSMessage.Contains("Email address has already been taken"))
                                {
                                    var result = await RDSResponse2.Content.ReadAsStringAsync();

                                    DirectSignUpResponse data = new DirectSignUpResponse();
                                    data = JsonConvert.DeserializeObject<DirectSignUpResponse>(result);

                                    Application.Current.Properties["user_id"] = data.result.customer_uid;
                                    Debug.WriteLine("new user's customer uid: " + data.result.customer_uid);
                                    Application.Current.Properties["platform"] = "GOOGLE";
                                    Application.Current.MainPage = new CongratsPage();
                                }
                                else
                                {
                                    DisplayAlert("Oops", "This email address is already taken by an existing account.", "OK");
                                }
                                //Application.Current.MainPage = new CarlosSocialSignUp(googleData.id, googleData.given_name, googleData.family_name, googleData.email, accessToken, refreshToken, "GOOGLE");
                                //Application.Current.MainPage = new Registration("GOOGLE", info);
                        }
                        //else if (responseContent.Contains(Constant.AutheticatedSuccesful))
                        else if (data5.code.ToString() == Constant.AutheticatedSuccesful)
                        {
                            await DisplayAlert("Oops", "This email is already taken by an existing account.", "OK");
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

        void clickedSeePassword(System.Object sender, System.EventArgs e)
        {
            if (passEntry.IsPassword == true)
                passEntry.IsPassword = false;
            else passEntry.IsPassword = true;
        }

        void clickedSeeConfirmPassword(System.Object sender, System.EventArgs e)
        {
            if (confirmPassEntry.IsPassword == true)
                confirmPassEntry.IsPassword = false;
            else confirmPassEntry.IsPassword = true;
        }

        async void backClicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopAsync();
        }

        //menu functions
        void registerClicked(System.Object sender, System.EventArgs e)
        {
            Dictionary<string, string> holder = new Dictionary<string, string>();
            Application.Current.MainPage = new NavigationPage(new Registration("DIRECT", holder));
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
    }
}
