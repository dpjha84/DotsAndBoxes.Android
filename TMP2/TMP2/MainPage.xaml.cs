using DotsAndBoxesFun.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DotsAndBoxesFun
{
    public enum DifficulyLevel
    {
        Easy, Medium, Hard
    };

    public partial class MainPage : MasterDetailPage
    {
        public ObservableCollection<MasterPageItem> allMenuList { get; set; }
        public ObservableCollection<MasterPageItem> expandedMenuList { get; set; }
        Entry entry = new Entry();

        int currentBoardSize = 6;
        bool currentFirstMove = false;
        DifficulyLevel currentDifficulyLevel = DifficulyLevel.Medium;
        bool IsMute = false;
        Test1 currentGame;
        public MainPage()
        {
            InitializeComponent();

            entry.Placeholder = "Settings updated.";
            entry.HorizontalOptions = LayoutOptions.Center;

            allMenuList = new ObservableCollection<MasterPageItem>();
            expandedMenuList = new ObservableCollection<MasterPageItem>();

            // Creating our pages for menu navigation
            // Here you can define title for item, 
            // icon on the left side, and page that you want to open after selection
            //var page1 = new MasterPageItem() { Title = "Item 1", Icon = "itemIcon1.png", TargetType = typeof(Test1) };
            BuildMenu();
            UpdateListContent();
            // Setting our list to be ItemSource for ListView in MainPage.xaml
            //navigationDrawerList.ItemsSource = allMenuList;
            //gameMode.ItemsSource = gameModeList;
            //navigationDrawerList1.ItemsSource = menuList;
            LaunchGame();
        }

        private void UpdateListContent()
        {
            expandedMenuList.Clear();
            foreach (var item in allMenuList)
            {
                var newGroup = new MasterPageItem(item.Title, item.ShortName, item.Expanded);
                newGroup.ItemCount = item.ItemCount;
                if (item.Expanded)
                {
                    foreach (var group in item)
                    {
                        newGroup.Add(group);
                    }
                }
                expandedMenuList.Add(newGroup);
            }
            navigationDrawerList.ItemsSource = expandedMenuList;
        }

        public void LaunchGame()
        {
            Detail = new NavigationPage(StartGame());
        }

        private void BuildMenu()
        {
            ReadSetting();
            var page1 = new SubItem() { Id = 1, Title = "New Game", Icon = "itemIcon5.png", CurrentValue="Test"};
            var page2 = new SubItem() { Id = 2, Title = "Board size", Icon = "itemIcon2.png", CurrentValue = $"{currentBoardSize}x{currentBoardSize}" };
            var page3 = new SubItem() { Id = 3, Title = "Difficulty level", Icon = "itemIcon3.png", CurrentValue = currentDifficulyLevel.ToString() };
            var page4 = new SubItem() { Id = 4, Title = "First move", Icon = "itemIcon4.png", CurrentValue = currentFirstMove ? "CPU" : "You" };

            var page5 = new SubItem() { Id = 5, Title = "Sound", Icon = "sound.png", CurrentValue = IsMute ? "Off" : "On" };
            var page6 = new SubItem() { Id = 6, Title = "Give Feedback", Icon = "feedback.png", CurrentValue = "ss" };

            var page7 = new SubItem() { Id = 7, Title = "Game mode", Icon = "feedback.png", CurrentValue = "Classic" };
            var page8 = new SubItem() { Id = 8, Title = "Current Level", Icon = "feedback.png", CurrentValue = "1" };

            allMenuList.Add(new MasterPageItem("Game Mode", "") { page7 });
            allMenuList.Add(new MasterPageItem("Modern Game", "") { page8 });
            allMenuList.Add(new MasterPageItem("Classic Game", "test2") { page1, page2, page3, page4});
            allMenuList.Add(new MasterPageItem("", "test4") { page5 });
            allMenuList.Add(new MasterPageItem("", "test4") { page6 });
        }

        private void ReadSetting()
        {
            var data = DependencyService.Get<IUserPreferences>().GetString("BoardSize");
            if (!string.IsNullOrWhiteSpace(data))
                currentBoardSize = int.Parse(data);

            data = DependencyService.Get<IUserPreferences>().GetString("DifficulyLevel");
            if (!string.IsNullOrWhiteSpace(data))
                currentDifficulyLevel = (DifficulyLevel)Enum.Parse(typeof(DifficulyLevel), data);

            data = DependencyService.Get<IUserPreferences>().GetString("FirstMove");
            if (!string.IsNullOrWhiteSpace(data))
                currentFirstMove = bool.Parse(data);

            data = DependencyService.Get<IUserPreferences>().GetString("IsMute");
            if (!string.IsNullOrWhiteSpace(data))
                IsMute = bool.Parse(data);
        }

        private void RebuildMenu(int id, string newValue)
        {
            SubItem page1, page2, page3, page4, page5, page6, page7, page8;
            page1 = new SubItem() { Id = 1, Title = "New Game", Icon = "itemIcon5.png", CurrentValue = "Test" };
            page2 = new SubItem() { Id = 2, Title = "Board size", Icon = "itemIcon2.png", CurrentValue = allMenuList[0][1].CurrentValue };
            page3 = new SubItem() { Id = 3, Title = "Difficulty level", Icon = "itemIcon3.png", CurrentValue = allMenuList[0][2].CurrentValue };
            page4 = new SubItem() { Id = 4, Title = "First move", Icon = "itemIcon4.png", CurrentValue = allMenuList[0][3].CurrentValue };

            page5 = new SubItem() { Id = 5, Title = "Sound", Icon = "sound.png", CurrentValue = allMenuList[1][0].CurrentValue };
            page6 = new SubItem() { Id = 6, Title = "Give Feedback", Icon = "feedback.png", CurrentValue = "ss" };

            page7 = new SubItem() { Id = 7, Title = "Game mode", Icon = "feedback.png", CurrentValue = allMenuList[1][2].CurrentValue };
            page8 = new SubItem() { Id = 8, Title = "Current Level", Icon = "feedback.png", CurrentValue = "1" };

            if (id == 2)
                page2.CurrentValue = newValue;
            else if (id == 3)
                page3.CurrentValue = newValue;
            else if (id == 4)
                page4.CurrentValue = newValue;
            else if (id == 5)
                page5.CurrentValue = newValue;

            allMenuList.Clear();
            allMenuList.Add(new MasterPageItem("Game Mode", "") { page7 });
            allMenuList.Add(new MasterPageItem("Modern Game", "") { page8 });
            allMenuList.Add(new MasterPageItem("Classic Game", "test2") { page1, page2, page3, page4 });
            allMenuList.Add(new MasterPageItem("", "test4") { page5 });
            allMenuList.Add(new MasterPageItem("", "test4") { page6 });
        }

        private string[] GetButtons(int menuId)
        {
            if (menuId == 1)
            {
                string[] buttons = new string[5];
                int b = 0;
                for (int i = 5; i <= 9; i++)
                {
                    buttons[b++] = (currentBoardSize == i) ? $"» {i}x{i}" : $"  {i}x{i}";
                }
                return buttons;
            }
            else if (menuId == 2)
            {
                string[] buttons = new string[3];
                int b = 0;
                foreach (var level in Enum.GetValues(typeof(DifficulyLevel)))
                {
                    buttons[b++] = (currentDifficulyLevel == (DifficulyLevel)level) ? $"» {level}" : $"  {level}";
                }
                return buttons;
            }
            else
            {
                string[] buttons1 = new string[2];
                for (int i = 0; i <= 1; i++)
                {
                    buttons1[i++] = !currentFirstMove == true ? "» You" : $"  CPU";
                }
                if (!currentFirstMove)
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

        private async void OnMenuItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                if (e.SelectedItem == null) return;
                navigationDrawerList.SelectedItem = null;
                if ((e.SelectedItem as SubItem).Id == 1)
                    Detail = new NavigationPage(StartGame());
                else if ((e.SelectedItem as SubItem).Id == 2)
                {
                    var action = await DisplayActionSheet("Choose Board size", "Cancel", null, GetButtons(1));
                    if (action == null || action == "Cancel") return;
                    action = action.Replace("» ", "").Trim();
                    int newBoardSize = Convert.ToInt32(action.Split('x')[0]);
                    if (newBoardSize == currentBoardSize) return;
                    currentBoardSize = newBoardSize;
                    RebuildMenu(2, action);
                    Detail = new NavigationPage(StartGame());
                    XFToast.LongMessage($"Board size is set to {action}");
                }
                else if ((e.SelectedItem as SubItem).Id == 3)
                {
                    var action = await DisplayActionSheet("Choose Difficulty level", "Cancel", null, GetButtons(2));

                    if (action == null || action == "Cancel") return;
                    var newDifficulyLevel = (DifficulyLevel)Enum.Parse(typeof(DifficulyLevel), action.Replace("» ", "").Trim());
                    if (newDifficulyLevel == currentDifficulyLevel) return;

                    currentDifficulyLevel = newDifficulyLevel;
                    RebuildMenu(3, currentDifficulyLevel.ToString());
                    Detail = new NavigationPage(StartGame());
                    XFToast.LongMessage($"Difficulty level is set to {currentDifficulyLevel.ToString()}");
                }
                else if ((e.SelectedItem as SubItem).Id == 4)
                {
                    var action = await DisplayActionSheet("Choose who should move first", "Cancel", null, GetButtons(3));

                    if (action == null || action == "Cancel") return;
                    action = action.Replace("» ", "").Trim();
                    bool newFirstMove = (action == "CPU");
                    if (newFirstMove == currentFirstMove) return;

                    currentFirstMove = newFirstMove;
                    RebuildMenu(4, action);
                    Detail = new NavigationPage(StartGame());
                    XFToast.LongMessage($"First move is set to {action}");
                }
                else if ((e.SelectedItem as SubItem).Id == 5)
                {
                    IsMute = currentGame.IsMute = !currentGame.IsMute;
                    string newValue = currentGame.IsMute ? "Off" : "On";
                    RebuildMenu(5, newValue);
                    XFToast.LongMessage($"Sound is {newValue}");
                }
                else if ((e.SelectedItem as SubItem).Id == 6)
                {
                    Device.OpenUri(new Uri("https://play.google.com/store/apps/details?id=com.game.dotsandboxesfun"));
                }
                    //Detail = new NavigationPage(new Test1(currentBoardSize, currentDifficulyLevel, currentFirstMove));
                IsPresented = false;
                UpdateListContent();
            }
            catch(Exception ex)
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

        private void gameMode_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            int selectedIndex = expandedMenuList.IndexOf(((MasterPageItem)((Button)sender).CommandParameter));
            allMenuList[selectedIndex].Expanded = !allMenuList[selectedIndex].Expanded;
            UpdateListContent();
        }
    }

    public class MasterPageItem : ObservableCollection<SubItem>, INotifyPropertyChanged
    {
        public MasterPageItem(string title, string shortName, bool expanded = true)
        {
            Title = title;
            ShortName = shortName;
            Expanded = expanded;
        }
        bool _expanded;
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortName { get; set; }
        public int ItemCount { get; set; }
        public string TitleWithItemCount { get { return $"{Title} {ItemCount}";} }
        public string StateIcon { get { return Expanded ? "feedback.png" : "sound.png"; } }
        public bool Expanded
        {
            get
            {
                return _expanded;
            }
            set
            {
                if (_expanded != value)
                {
                    _expanded = value;
                    //RaisePropertyChanged("Expanded");
                    //RaisePropertyChanged("StateIcon");
                }
            }
        }
        public string Icon { get; set; }

        protected void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            //var evt = PropertyChanged;   // create local copy in case the reference is replaced
            //if (evt != null)             // check if there are any subscribers
               // PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SubItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CurrentValue { get; set; }
        public string Icon { get; set; }
    }

    public interface IMessage
    {
        void LongAlert(string message);
        void ShortAlert(string message);
    }

    public static class XFToast
    {
        public static void LongMessage(string message)
        {
            try
            {
            DependencyService.Get<IMessage>().LongAlert(message);
            }
            catch (Exception ex)
            {
            }
        }
    }

    public interface IAudio
    {
        void PlayAudioFile(string fileName);
    }

    public interface IUserPreferences
    {
        void SetString(string key, string value);
        string GetString(string key);
    }
}
