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
    public partial class Setting : ContentPage
    {
        public Setting()
        {
            InitializeComponent();
            ePlayer1Name.Text = GlobalSetting.Player1Name;
            ePlayer2Name.Text = GlobalSetting.Player2Name;
        }

        private void btnSave_Clicked(object sender, EventArgs e)
        {
            GlobalSetting.Player1Name = ePlayer1Name.Text;
            GlobalSetting.Player2Name = ePlayer2Name.Text;
            XFToast.LongMessage("Setting saved.");
        }

        private void btnResetChallange_Clicked(object sender, EventArgs e)
        {
            ChallengeGameSetting.ChallengeLevel = 1;
            ChallengeGameSetting.StarCount = 0;
            XFToast.LongMessage("Challenge mode is reset to Level 1.");
        }

        private void btnResetClassic_Clicked(object sender, EventArgs e)
        {
            ClasicGameSetting.BoardSize = 6;
            ClasicGameSetting.DifficultyLevel = DifficultyLevel.Medium;
            ClasicGameSetting.FirstMove = false;
            XFToast.LongMessage("Classic mode is reset.");
        }
    }
}