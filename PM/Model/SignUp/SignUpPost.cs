using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PM.Model.SignUp
{

    // object to send to database when user attempts to sign up 
    // link: https://c1zwsl05s5.execute-api.us-west-1.amazonaws.com/dev/api/v2/createAccount
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
}