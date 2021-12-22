using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace PM.Model
{ 
    public class PropListDetails
    {
        public double titleFontSize { get; set; }
        public string titleText { get; set; }
        public Color monthlyColor { get; set; }
        public bool monthlyVisible { get; set; }
        public string monthlyText { get; set; }
        public Color ytdColor { get; set; }
        public bool ytdVisible { get; set; }
        public string ytdText { get; set; }
        public Thickness gridMargin { get; set; }
        public bool newVisible { get; set; }

    }
}
