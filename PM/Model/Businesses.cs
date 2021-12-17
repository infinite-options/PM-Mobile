using System;
namespace PM.Model
{
    public class BusinessesResponse
    {
        public string message { get; set; }
        public InnerResult result { get; set; }
    }

    public class InnerResult
    {
        public string message { get; set; }
        public int code { get; set; }
        public Business[] result { get; set; }
    }

    public class Business
    {
        public string business_uid { get; set; }
        public string business_name { get; set; }
        public string business_type { get; set; }
        public string business_desc { get; set; }
        public string business_contact_first_name { get; set; }
        public string business_contact_last_name { get; set; }
        public string business_phone_num { get; set; }
        public string business_phone_num2 { get; set; }
        //try to convert into AcceptingHours
        public string business_accepting_hours { get; set; }
        public string business_address { get; set; }
        public string business_unit { get; set; }
        public string business_city { get; set; }
        public string business_state { get; set; }
        public string business_zip { get; set; }
        public string can_cancel { get; set; }
        public string delivery { get; set; }
        public string pick_up { get; set; }
        public string reusable { get; set; }
        public string business_image { get; set; }
        public string business_status { get; set; }
        public string business_facebook_url { get; set; }
        public string business_instagram_url { get; set; }
        public string business_twitter_url { get; set; }
        public string business_website_url { get; set; }
        public string limit_per_person { get; set; }
        //try to convert into Types
        public string item_types { get; set; }
    }

    public class AcceptingHours
    {
        public string[] Friday { get; set; }
        public string[] Monday { get; set; }
        public string[] Sunday { get; set; }
        public string[] Tuesday { get; set; }
        public string[] Saturday { get; set; }
        public string[] Thursday { get; set; }
        public string[] Wednesday { get; set; }
    }

    public class Types
    {
        public bool fruits { get; set; }
        public bool vegetables { get; set; }
        public bool meals { get; set; }
        public bool desserts { get; set; }
        public bool beverages { get; set; }
        public bool dairy { get; set; }
        public bool snacks { get; set; }
        public bool cannedFoods { get; set; }
    }
}
