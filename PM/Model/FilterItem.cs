using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace PM.Model
{
    public class FilterItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public string filterName { get; set; }
        public Color filterColor { get; set; }
        public Color filterTextColor { get; set; }

        public Color filterColorUpdate
        {

            set { filterColor = value; PropertyChanged(this, new PropertyChangedEventArgs("filterColor")); }
        }
        public Color filterTextColorUpdate
        {

            set { filterTextColor = value; PropertyChanged(this, new PropertyChangedEventArgs("filterTextColor")); }
        }
    }
}
