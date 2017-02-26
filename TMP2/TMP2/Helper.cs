using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DotsAndBoxesFun
{
    public static class Constants
    {
        public static Color compColor = Color.FromHex("#D13A1B");
        public static Color playerColor = Color.FromHex("#12B504");
        public static Color compPrevColor = Color.FromHex("#DBB312");

        public static bool IsFilled(this BoxView shape)
        {
            return shape.BackgroundColor == compColor || shape.BackgroundColor == playerColor || shape.BackgroundColor == compPrevColor;
        }
    }

    public enum Turn
    {
        Player1,
        Player2
    };

    public class House
    {
        public Xamarin.Forms.BoxView Grid { get; set; }
        public string Text { get; set; }
        public int FilledCount { get; set; }
    }

    public class Edge
    {
        public string Name { get; set; }
        public House House1 { get; set; }
        public House House2 { get; set; }
        public BoxView Shape { get; set; }

        public bool IsFilled
        {
            get
            {
                return Shape.BackgroundColor == Constants.playerColor || Shape.BackgroundColor == Constants.compColor || Shape.BackgroundColor == Constants.compPrevColor;
            }
        }
    }
}
