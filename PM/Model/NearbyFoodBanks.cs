using System;
namespace PM.Model
{
    public class NearbyFoodBanks
    {
        public string result { get; set; }
        public BanksFound[] banks_found { get; set; }
    }

    public class BanksFound
    {
        public string business_uid { get; set; }
        public string business_created_at { get; set; }
        public string business_name { get; set; }
        public string business_type { get; set; }
        public string business_desc { get; set; }
        public string business_contact_first_name { get; set; }
        public string business_contact_last_name { get; set; }
        public string business_phone_num { get; set; }
        public string business_phone_num2 { get; set; }
        public string business_email { get; set; }
        public string business_hours { get; set; }
        public string business_accepting_hours { get; set; }
        public string business_delivery_hours { get; set; }
        public string business_address { get; set; }
        public string business_unit { get; set; }
        public string business_city { get; set; }
        public string business_state { get; set; }
        public string business_zip { get; set; }
        public string bus_notification_approval { get; set; }
        public int can_cancel { get; set; }
        public int delivery { get; set; }
        public int pick_up { get; set; }
        public int reusable { get; set; }
        public string business_image { get; set; }
        public string business_password { get; set; }
        public string bus_guid_device_id_notification { get; set; }
        public string business_links { get; set; }
        public string business_status { get; set; }
        public string business_facebook_url { get; set; }
        public string business_instagram_url { get; set; }
        public string business_twitter_url { get; set; }
        public string business_website_url { get; set; }
        public string limit_per_person { get; set; }
        public string item_types { get; set; }
        public string business_latitude { get; set; }
        public string business_longitude { get; set; }
    }
}
