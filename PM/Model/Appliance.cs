using System;
using System.ComponentModel;

namespace PM.Model
{
    public class Appliance : INotifyPropertyChanged
    {
        string _checkboxImg { get; set; }
        public string appName { get; set; }
        bool _isChecked { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public string checkboxImg
        {
            set
            {
                if (_checkboxImg != value)
                {
                    _checkboxImg = value;
                    OnPropertyChanged("checkboxImg");
                }
            }
            get
            {
                return _checkboxImg;
            }

        }

        public bool isChecked
        {
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged("isChecked");
                }
            }
            get
            {
                return _isChecked;
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
}
