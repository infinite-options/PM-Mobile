using System;
using System.ComponentModel;

namespace PM.Model
{
    public class FoodBanks : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string name { get; set; } //done
        public string business_uid { get; set; } //done
        public string bankImg { get; set; } //done
        public string desc { get; set; } 
        public bool openNow { get; set; }
        public bool delivery { get; set; } //done
        public bool pickup { get; set; }
        public int itemLimit { get; set; } //done
        public bool fruits { get; set; } //done
        public bool vegetables { get; set; } //done
        public bool meals { get; set; } //done
        public bool desserts { get; set; } //done
        public bool beverages { get; set; } //done
        public bool dairy { get; set; } //done
        public bool snacks { get; set; } //done
        public bool cannedFoods { get; set; } //done
        public string mondayHours { get; set; } //done
        public string tuesdayHours { get; set; } //done
        public string wednesdayHours { get; set; } //done
        public string thursdayHours { get; set; } //done
        public string fridayHours { get; set; } //done
        public string saturdayHours { get; set; } //done
        public string sundayHours { get; set; } //done
        public string distance { get; set; }
        public string address { get; set; }
        public string unit { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        bool hoursVisible;
        bool totalVisible;
        int filterCount;
        double height;

        //added from mapped food banks
        public double latitude { get; set; }
        public double longitude { get; set; }
        //added

        public double Height
        {
            set
            {
                if (height != value)
                {
                    height = value;
                    OnPropertyChanged("Height");
                }
            }
            get
            {
                return height;
            }
        }

        public int FilterCount
        {
            set
            {
                if (filterCount != value)
                {
                    filterCount = value;
                    OnPropertyChanged("FilterCount");
                }
            }
            get
            {
                return filterCount;
            }

        }

        public bool HoursVisible
        {
            set
            {
                if (hoursVisible != value)
                {
                    hoursVisible = value;
                    OnPropertyChanged("HoursVisible");
                }
            }
            get
            {
                return hoursVisible;
            }

        }

        public bool TotalVisible
        {
            set
            {
                if (totalVisible != value)
                {
                    totalVisible = value;
                    OnPropertyChanged("TotalVisible");
                }
            }
            get
            {
                return totalVisible;
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class MappedFoodBanks
    {
        public string name { get; set; } //done
        public string business_uid { get; set; } //done
        public string bankImg { get; set; } 
        public string desc { get; set; }
        public bool openNow { get; set; }
        public bool delivery { get; set; } 
        public bool pickup { get; set; }
        public int itemLimit { get; set; } 
        public bool fruits { get; set; }
        public bool vegetables { get; set; } 
        public bool meals { get; set; } 
        public bool desserts { get; set; } 
        public bool beverages { get; set; } 
        public bool dairy { get; set; } 
        public bool snacks { get; set; } 
        public bool cannedFoods { get; set; } 
        public string mondayHours { get; set; } 
        public string tuesdayHours { get; set; } 
        public string wednesdayHours { get; set; } 
        public string thursdayHours { get; set; } 
        public string fridayHours { get; set; } 
        public string saturdayHours { get; set; } 
        public string sundayHours { get; set; } 
        public string distance { get; set; }
        bool hoursVisible;
        bool totalVisible;
        int filterCount;
        double height;
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string bus_img { get; set; }
        public string item_limit { get; set; }
    }
}
