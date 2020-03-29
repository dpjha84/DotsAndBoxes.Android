using Xamarin.Forms;

namespace DotsAndBoxesFun.Views
{
    public class AdBanner2 : View
    {
        public enum Sizes { Standardbanner, LargeBanner, MediumRectangle, FullBanner, Leaderboard, SmartBanner, Banner }
        public Sizes Size { get; set; } = Sizes.Banner;
    }
}