using System;
using System.ComponentModel;

namespace PM.Model
{
    public class StoreItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public string image { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }
        public int availableAmt { get; set; }
        public string type { get; set; }
        public string unit { get; set; }
        public int price { get; set; }
        public string item_uid { get; set; }
        public string itm_business_uid { get; set; }
        public string measure { get; set; }

        public int quantityUpdate
        {
            set
            {
                quantity = value;
                PropertyChanged(this, new PropertyChangedEventArgs("quantity"));
            }
        }
    }
}
