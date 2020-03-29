 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DotsAndBoxesFun.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        //IAdInterstitial adInterstitial;

        public HomePage()
        {
            InitializeComponent();
            //adInterstitial = DependencyService.Get<IAdInterstitial>();
            //adInterstitial.ShowAd();
            //Setting.Reset();
        }

        private void BtnClassic_Clicked(object sender, EventArgs e)
        {
            new ClassicGameRunner().Start();
        }        

        private void BtnChallange_Clicked(object sender, EventArgs e)
        {
            new ChallengeGameRunner().Start();
        }

        private void ButtonSetting_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Setting());
            
        }

        private void ButtonHowToPlay_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new HowToPlay());
        }

        //void Show_Interstitial(object sender, EventArgs e)
        //{
        //    adInterstitial.ShowAd();
        //}
    }
}