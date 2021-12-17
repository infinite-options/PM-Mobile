using System;
namespace PM.Model.Login.LoginClasses
{
    public class SignUpPost
    {
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string phone_number { get; set; }
        public string id_type { get; set; }
        public string id_number { get; set; }
        public string address { get; set; }
        public string unit { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip_code { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string referral_source { get; set; }
        public string role { get; set; }
        public string affiliation { get; set; }
        public string social { get; set; }
        public string password { get; set; }
        public string mobile_access_token { get; set; }
        public string mobile_refresh_token { get; set; }
        public string user_access_token { get; set; }
        public string user_refresh_token { get; set; }
        public string social_id { get; set; }
        public string cust_id { get; set; }

    }

    /*
     sample object:
    {
        "email": "aks7@gmail.com",
        "first_name": "anureet",
        "last_name": "sandhu",
        "phone_number": "2138581343",
        "id_type": "Driver License",
        "id_number": "F756047",
        "address": "8831 Ramona Street",
        "unit": "",
        "city": "Bellflower",
        "state": "CA",
        "zip_code": "90706",
        "latitude": "-14.3",
        "longitude": "94.3",
        "referral_source": "WEB",
        "role": "CUSTOMER",
        "social": "NULL",
        "password": "abc@123",
        "mobile_access_token": "FALSE",
        "mobile_refresh_token": "FALSE",
        "user_access_token": "FALSE",
        "user_refresh_token": "FALSE",
        "social_id": "NULL"
    }
     */

    /*
     possible responses:
     - successful signup: 
     {
        "message": "Signup successful",
        "code": 200,
        "result": {
            "first_name": "anureet",
            "last_name": "sandhu",
            "customer_uid": "100-000197",
            "access_token": "NULL",
            "refresh_token": "NULL",
            "social_id": "NULL"
        }
    }

    - email already taken:
    {
        "message": "Email address has already been taken.",
        "code": 409,
        "result": [{
            "customer_email": "j@gmail.com",
            "role": "CUSTOMER",
            "customer_uid": "100-000197"
        }]
    }
     */

    public class changePasswordPost
    {
        public string customer_uid { get; set; }
        public string old_password { get; set; }
        public string new_password { get; set; }
    }

    public class emailVerifyPost
    {
        public string email { get; set; }
    }
}
