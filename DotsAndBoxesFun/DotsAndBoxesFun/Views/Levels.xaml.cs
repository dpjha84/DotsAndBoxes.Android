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
    public partial class Levels : ContentPage
    {
        public Levels()
        {
            InitializeComponent();
            int level = 1;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Button button;
                    if (level <= ChallengeGameSetting.ChallengeLevel)
                        button = GetEnabledButton(level.ToString());
                    else
                        button = GetDisabledButton(level.ToString());
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                    grid.Children.Add(button);
                    level++;
                }
            }            
        }

        public Button GetEnabledButton(string text)
        {
            var button = new Button
            {
                Text = text,
                BackgroundColor = ChallengeGameSetting.ChallengeLevel.ToString() == text ? Color.FromHex("#9BF700") : Color.FromHex("#E8AD00"),
                FontSize = 24,
                TextColor = Color.Black,
            };
            button.Clicked += Button_Clicked;
            return button;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            int level = int.Parse(((Button)sender).Text);
            new ChallengeGameRunner(level).Start();
        }

        public Button GetDisabledButton(string text)
        {
            var button = new Button
            {
                Text = text,
                BackgroundColor = Color.FromHex("#ddd"),
                FontSize = 24,
                TextColor = Color.Gray,
                IsEnabled = false
            };
            return button;
        }
    }
}