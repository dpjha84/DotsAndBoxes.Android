using DotsAndBoxesFun.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public HomePage()
        {
            InitializeComponent();
            btnChallenge.Text += $" - Level {ChallengeGameSetting.ChallengeLevel}";
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

        async void ShowInterstitialAdsButton_Clicked(object sender, EventArgs e)
        {
            if (AppConstants.ShowAds)
            {
                await DependencyService.Get<IAdmobInterstitialAds>().Display(AppConstants.InterstitialAdId);
            }

            Debug.WriteLine("Continue button click implementation");
        }

        async void NavigateToPage2_Clicked(object sender, EventArgs e)
        {
            //await this.Navigation.PushAsync(new Page2());
        }
    }
}