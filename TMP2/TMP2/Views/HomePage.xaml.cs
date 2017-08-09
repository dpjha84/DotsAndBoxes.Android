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
        int currentBoardSize = 1;
        bool currentFirstMove = false;
        DifficulyLevel currentDifficulyLevel = DifficulyLevel.Medium;
        bool IsMute = false;
        Test1 currentGame;

        public HomePage()
        {
            InitializeComponent();
        }

        private void BtnClassic_Clicked(object sender, EventArgs e)
        {
            try
            {
                Application.Current.MainPage = new NavigationPage(new Test1(1, DifficulyLevel.Medium, false, false, this));
            }
            catch (Exception ex)
            {

            }
            
            //while (Constants.restartGame)
            //{
            //    Constants.restartGame = false;
            //    Navigation.PopAsync();
            //    Navigation.PushAsync(new Test1(1, DifficulyLevel.Medium, false, false, this), true);
            //}
            //MainPage =  new MainPage();
        }

        public void LaunchGame()
        {
            try
            {
                Application.Current.MainPage = new NavigationPage(StartGame());
            }
            catch (Exception ex)
            {

            }
        }

        private Test1 StartGame()
        {
            DependencyService.Get<IUserPreferences>().SetString("BoardSize", currentBoardSize.ToString());
            DependencyService.Get<IUserPreferences>().SetString("DifficulyLevel", currentDifficulyLevel.ToString());
            DependencyService.Get<IUserPreferences>().SetString("FirstMove", currentFirstMove.ToString());
            DependencyService.Get<IUserPreferences>().SetString("IsMute", IsMute.ToString());
            currentGame = new Test1(currentBoardSize, currentDifficulyLevel, currentFirstMove, IsMute, this);
            return currentGame;
        }
    }
}