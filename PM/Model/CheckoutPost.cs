using System;
namespace PM.Model
{
    public class CheckoutPost
    {
        public string pur_customer_uid { get; set; }
        public string pur_business_uid { get; set; }
        public Item[] items { get; set; }
        public string order_instructions { get; set; }
        public string delivery_instructions { get; set; }
        public string order_type { get; set; }
        public string delivery_first_name { get; set; }
        public string delivery_last_name { get; set; }
        public string delivery_phone_num { get; set; }
        public string delivery_email { get; set; }
        public string delivery_address { get; set; }
        public string delivery_unit { get; set; }
        public string delivery_city { get; set; }
        public string delivery_state { get; set; }
        public string delivery_zip { get; set; }
        public string delivery_latitude { get; set; }
        public string delivery_longitude { get; set; }
        public string purchase_notes { get; set; }
        public string start_delivery_date { get; set; }
        public string pay_coupon_id { get; set; }
        public string amount_due { get; set; }
        public string amount_discount { get; set; }
        public string amount_paid { get; set; }
        public string info_is_Addon { get; set; }
        public string cc_num { get; set; }
        public string cc_exp_date { get; set; }
        public string cc_cvv { get; set; }
        public string cc_zip { get; set; }
        public string charge_id { get; set; }
        public string payment_type { get; set; }
        public string delivery_status { get; set; }
        public string subtotal { get; set; }
        public string service_fee { get; set; }
        public string delivery_fee { get; set; }
        public string driver_tip { get; set; }
        public string taxes { get; set; }
        public string ambassador_code { get; set; }
    }

    public class Item
    {
        public string img { get; set; }
        public int qty { get; set; }
        public string name { get; set; }
        public string unit { get; set; }
        public int price { get; set; }
        public string item_uid { get; set; }
        public string itm_business_uid { get; set; }
    }
}
