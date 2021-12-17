using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace PM.Model
{
    public class Date1 : INotifyPropertyChanged
    {
        string img;
        Color textcolor;

        public event PropertyChangedEventHandler PropertyChanged;
        public string dotw { get; set; }
        public string day { get; set; }
        public string month { get; set; }
        public DateTime dateObj { get; set; }
        //public string fullDateTime { get; set; }
        public string BackgroundImg
        {
            set
            {
                if (img != value)
                {
                    img = value;
                    OnPropertyChanged("BackgroundImg");
                }
            }
            get
            {
                return img;
            }

        }

        public Color TextColor
        {
            set
            {
                if (textcolor != value)
                {
                    textcolor = value;
                    OnPropertyChanged("TextColor");
                }
            }
            get
            {
                return textcolor;
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

    public class Date : INotifyPropertyChanged
    {
        Color Fill;
        Color Outline;

        public event PropertyChangedEventHandler PropertyChanged;
        public string dotw { get; set; }
        //public string month { get; set; }
        public string date { get; set; }
        public string Status { get; set; }
        public string fullDateTime { get; set; }
        //public Color fillColor { get; set; }
        public int index { get; set; }
        public Color fillColor
        {
            set
            {
                if (Fill != value)
                {
                    Fill = value;
                    OnPropertyChanged("fillColor");
                }
            }
            get
            {
                return Fill;
            }

        }

        public Color outlineColor
        {
            set
            {
                if (Outline != value)
                {
                    Outline = value;
                    OnPropertyChanged("outlineColor");
                }
            }
            get
            {
                return Outline;
            }

        }

        public string status
        {
            set
            {
                if (Status != value)
                {
                    Status = value;
                    OnPropertyChanged("status");
                }
            }
            get
            {
                return Status;
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
