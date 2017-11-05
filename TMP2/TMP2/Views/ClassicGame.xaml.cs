using System;

namespace DotsAndBoxesFun.Views
{
    public partial class ClassicGame : GameBase
    {
        public ClassicGame()
        {
            this.InitializeComponent();
            InitialSetup(grid, btnSound);

            gridOption.Children.Clear();
            gridOption.Children.Add(btnPlayerMode);
            gridOption.Children.Add(btnBoardSize);
            if (!ClasicGameSetting.Is2Players)
            {
                gridOption.Children.Add(btnDiffLevel);
                gridOption.Children.Add(btnFirstMove);
            }

            textPlayer2Name.Text = ClasicGameSetting.Is2Players ? GlobalSetting.Player2Name : "CPU";
            SetupControls(textPlayer1Name, textPlayer2Name, textPlayer1Score, textPlayer2Score, separator1, separator2);

            count = ClasicGameSetting.BoardSize;
            btnBoardSize.Text = $"{count}x{count}";
            this.moveFirst = ClasicGameSetting.FirstMove;
            btnFirstMove.Text = moveFirst ? "CPU" : "You";
            btnPlayerMode.Text = ClasicGameSetting.Is2Players ? "2P" : "1P";

            this.level = ClasicGameSetting.DifficultyLevel;
            btnDiffLevel.Text = level.ToString();            
            
            base.textPlayer1Name = textPlayer1Name;
            base.textPlayer2Name = textPlayer2Name;
            base.textPlayer1Score = textPlayer1Score;
            base.textPlayer2Score = textPlayer2Score;
            NewGame(grid);
        }        

        private string[] GetButtons(int menuId)
        {
            if (menuId == 1)
            {
                string[] buttons1 = new string[2];
                for (int i = 0; i <= 1; i++)
                {
                    buttons1[i++] = !ClasicGameSetting.Is2Players == true ? "» 1 Player" : $"  2 Players";
                }
                if (!ClasicGameSetting.Is2Players)
                {
                    buttons1[0] = "» 1 Player";
                    buttons1[1] = "  2 Players";
                }
                else
                {
                    buttons1[0] = "  1 Player";
                    buttons1[1] = "» 2 Players";
                }
                return buttons1;
            }
            else if (menuId == 2)
            {
                string[] buttons = new string[5];
                int b = 0;
                for (int i = 5; i <= 9; i++)
                {
                    buttons[b++] = (count == i) ? $"» {i}x{i}" : $"  {i}x{i}";
                }
                return buttons;
            }
            else if (menuId == 3)
            {
                string[] buttons = new string[3];
                int b = 0;
                foreach (var level1 in Enum.GetValues(typeof(DifficultyLevel)))
                {
                    if ((DifficultyLevel)level1 == DifficultyLevel.None) continue;
                    buttons[b++] = (level == (DifficultyLevel)level1) ? $"» {level1}" : $"  {level1}";
                }
                return buttons;
            }
            else
            {
                string[] buttons1 = new string[2];
                for (int i = 0; i <= 1; i++)
                {
                    buttons1[i++] = !moveFirst == true ? "» You" : $"  CPU";
                }
                if (!moveFirst)
                {
                    buttons1[0] = "» You";
                    buttons1[1] = "  CPU";
                }
                else
                {
                    buttons1[0] = "  You";
                    buttons1[1] = "» CPU";
                }
                return buttons1;
            }
        }

        private async void btnPlayerMode_Clicked(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("Choose Player mode", "Cancel", null, GetButtons(1));
            if (action == null || action == "Cancel") return;
            action = action.Replace("» ", "").Trim();
            bool is2Players = (action == "2 Players");
            if (is2Players == ClasicGameSetting.Is2Players) return;

            ClasicGameSetting.Is2Players = is2Players;
            new ClassicGameRunner().Start();
            XFToast.LongMessage($"Game is set to {action} mode");
        }

        private async void btnBoardSize_Clicked(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("Choose Board size", "Cancel", null, GetButtons(2));
            if (action == null || action == "Cancel") return;
            action = action.Replace("» ", "").Trim();
            int newBoardSize = Convert.ToInt32(action.Split('x')[0]);
            if (newBoardSize == count) return;
            //count = newBoardSize;
            ClasicGameSetting.BoardSize = newBoardSize;
            new ClassicGameRunner().Start();
            //Parent.LaunchGame(newBoardSize);
            btnBoardSize.Text = action;// $"{newBoardSize}x{newBoardSize}";
            //RebuildMenu(2, action);
            //Detail = new NavigationPage(StartGame());
            XFToast.LongMessage($"Board size is set to {action}");
        }

        private async void btnDiffLevel_Clicked(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("Choose Difficulty level", "Cancel", null, GetButtons(3));

            if (action == null || action == "Cancel") return;
            var newDifficulyLevel = (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), action.Replace("» ", "").Trim());
            if (newDifficulyLevel == level) return;
            ClasicGameSetting.DifficultyLevel = newDifficulyLevel;
            new ClassicGameRunner().Start();
            btnDiffLevel.Text = action;
            XFToast.LongMessage($"Difficulty level is set to {action}");
        }

        private async void btnFirstMove_Clicked(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("Choose who should move first", "Cancel", null, GetButtons(4));

            if (action == null || action == "Cancel") return;
            action = action.Replace("» ", "").Trim();
            bool newFirstMove = (action == "CPU");
            if (newFirstMove == moveFirst) return;

            ClasicGameSetting.FirstMove = newFirstMove;
            new ClassicGameRunner().Start();
            btnFirstMove.Text = action;
            XFToast.LongMessage($"First move is set to {action}");
        }        
    }
}
