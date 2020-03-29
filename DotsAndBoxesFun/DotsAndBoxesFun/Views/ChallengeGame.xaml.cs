using DotsAndBoxesFun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DotsAndBoxesFun.Views
{
    public partial class ChallengeGame : GameBase
    {
        public ChallengeGame()
        {
            InitializeComponent();
            InitialSetup(grid, btnSound);
            textPlayer2Name.Text = "CPU";
            SetupControls(textPlayer1Name, textPlayer2Name, textPlayer1Score, textPlayer2Score, separator1, separator2);

            count = ChallengeGameSetting.BoardSize;
            btnLevel.Text = $"Level {ChallengeGameSetting.RequestedChallengeLevel}";
            this.level = ChallengeGameSetting.DifficultyLevel;
            btnTarget.Text = $"Target {ChallengeGameSetting.TargetScore.ToString()}";            
            IsMute = GlobalSetting.IsMute;
            btnSound.Icon = IsMute ? "soundmute" : "sound";
            btnCoin.Text = ChallengeGameSetting.StarCount.ToString();
            base.textPlayer1Name = textPlayer1Name;
            base.textPlayer2Name = textPlayer2Name;
            base.textPlayer1Score = textPlayer1Score;
            base.textPlayer2Score = textPlayer2Score;
            NewGame(grid);
        }

        private void btnLevel_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Levels());
        }
    }
}
