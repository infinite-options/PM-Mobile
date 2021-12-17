using System;
using System.Collections.Generic;

namespace PM.Model
{
    public class GetItems
    {
        public List<string> types { get; set; }
        public List<string> ids { get; set; }
    }

    public class GetItemsResponse
    {
        public string message { get; set; }
        //public int code { get; set; }
        public BusinessInfo[] result { get; set; }
    }

    public class BusinessInfo
    {
        public string supply_uid { get; set; }
        public string sup_created_at { get; set; }
        public string sup_brand_uid { get; set; }
        public string sup_item_uid { get; set; }
        public string sup_desc { get; set; }
        public string sup_type { get; set; }
        public string sup_num { get; set; }
        public string sup_measure { get; set; }
        public string sup_unit { get; set; }
        public string detailed_num { get; set; }
        public string detailed_measure { get; set; }
        public string detailed_unit { get; set; }
        public string item_photo { get; set; }
        public string package_upc { get; set; }
        public string brand_uid { get; set; }
        public string brand_name { get; set; }
        public string brand_address { get; set; }
        public string brand_unit { get; set; }
        public string brand_city { get; set; }
        public string brand_state { get; set; }
        public string brand_zip { get; set; }
        public string brand_contact_first_name { get; set; }
        public string brand_contact_last_name { get; set; }
        public string brand_phone_num1 { get; set; }
        public string brand_phone_num2 { get; set; }
        public string item_uid { get; set; }
        public string item_name { get; set; }
        public string item_desc { get; set; }
        public string item_type { get; set; }
        public string item_tags { get; set; }
        public string receive_uid { get; set; }
        public string receive_supply_uid { get; set; }
        public string receive_business_uid { get; set; }
        public string donation_type { get; set; }
        public string qty_received { get; set; }
        public string receive_date { get; set; }
        public string available_date { get; set; }
        public string exp_date { get; set; }

    }
}
