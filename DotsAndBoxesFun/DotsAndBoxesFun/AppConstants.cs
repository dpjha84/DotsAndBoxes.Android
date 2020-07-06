using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DotsAndBoxesFun
{
    public class AppConstants
    {
        public static string AppId
        {
            get
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        return "";
                    default:
                        return "";
                }
            }
        }

        /// <summary>
        /// These Ids are test Ids from https://developers.google.com/admob/android/test-ads
        /// </summary>
        /// <value>The banner identifier.</value>
        public static string BannerId
        {

            get
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        return "ca-app-pub-9351754143985661/4193954438";//"ca-app-pub-3940256099942544/6300978111";//"ca-app-pub-9351754143985661/2277234543";
                    default:
                        return "ca-app-pub-9351754143985661/4193954438";//"ca-app-pub-9351754143985661/2277234543";
                }
            }
        }

        /// <summary>
        /// These Ids are test Ids from https://developers.google.com/admob/android/test-ads
        /// </summary>
        /// <value>The Interstitial Ad identifier.</value>
        public static string InterstitialAdId
        {
            get
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        return "ca-app-pub-3940256099942544/1033173712";
                    default:
                        return "ca-app-pub-3940256099942544/1033173712";
                }
            }
        }

        public static bool ShowAds
        {
            get
            {
                _adCounter++;
                if (_adCounter % 5 == 0)
                {
                    return true;
                }
                return false;
            }
        }

        private static int _adCounter;

    }
}
