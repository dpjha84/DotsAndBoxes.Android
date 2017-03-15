using DotsAndBoxesFun.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<MasterPageItem> menuList { get; set; }
        Entry entry = new Entry();

        int currentBoardSize = 6;
        bool currentFirstMove = false;
        DifficulyLevel currentDifficulyLevel = DifficulyLevel.Hard;
        public MainPage()
        {
            InitializeComponent();

            entry.Placeholder = "Settings saved. Hit New Game to play with new settings.";
            entry.HorizontalOptions = LayoutOptions.Center;

            menuList = new ObservableCollection<MasterPageItem>();

            // Creating our pages for menu navigation
            // Here you can define title for item, 
            // icon on the left side, and page that you want to open after selection
            //var page1 = new MasterPageItem() { Title = "Item 1", Icon = "itemIcon1.png", TargetType = typeof(Test1) };
            BuildMenu();

            // Setting our list to be ItemSource for ListView in MainPage.xaml
            navigationDrawerList.ItemsSource = menuList;

            // Initial navigation, this can be used for our home page
            Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(Test1)));
        }

        private void BuildMenu()
        {
            var page1 = new MasterPageItem() { Id = 1, Title = "New Game", Icon = "itemIcon5.png" };
            var page2 = new MasterPageItem() { Id = 2, Title = "Board size", Icon = "itemIcon2.png", CurrentValue = "6x6" };
            var page3 = new MasterPageItem() { Id = 3, Title = "Difficulty level", Icon = "itemIcon3.png", CurrentValue = "Medium" };
            var page4= new MasterPageItem() { Id = 4, Title = "First move", Icon = "itemIcon4.png", CurrentValue = "You" };

            menuList.Add(page1);
            menuList.Add(page2);
            menuList.Add(page3);
            menuList.Add(page4);
        }

        private void RebuildMenu(int id, string newValue)
        {
            MasterPageItem page1, page2, page3, page4;
            page1 = new MasterPageItem() { Id = 1, Title = "New Game", Icon = "itemIcon5.png" };
            page2 = new MasterPageItem() { Id = 2, Title = "Board size", Icon = "itemIcon2.png", CurrentValue = menuList[1].CurrentValue };
            page3 = new MasterPageItem() { Id = 3, Title = "Difficulty level", Icon = "itemIcon3.png", CurrentValue = menuList[2].CurrentValue };
            page4 = new MasterPageItem() { Id = 4, Title = "First move", Icon = "itemIcon4.png",CurrentValue = menuList[3].CurrentValue };

            if (id == 2)
                page2.CurrentValue = newValue;
            else if (id == 3)
                page3.CurrentValue = newValue;
            else if (id == 4)
                page4.CurrentValue = newValue;

            menuList.Clear();

            menuList.Add(page1);
            menuList.Add(page2);
            menuList.Add(page3);
            menuList.Add(page4);
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

                if ((e.SelectedItem as MasterPageItem).Id == 1)
                {
                    Detail = new NavigationPage(new Test1(currentBoardSize, currentDifficulyLevel, currentFirstMove));
                    IsPresented = false;
                }
                else if ((e.SelectedItem as MasterPageItem).Id == 2)
                {
                    var action = await DisplayActionSheet("Choose Board size", "Cancel", null, GetButtons(1));
                    if (action == null || action == "Cancel") return;
                    action = action.Replace("» ", "").Trim();
                    int newBoardSize = Convert.ToInt32(action.Split('x')[0]);
                    if (newBoardSize == currentBoardSize) return;
                    currentBoardSize = newBoardSize;
                    RebuildMenu(2, action);
                    //Detail = new NavigationPage(new Test1(newBoardSize, currentFirstMove));
                    XFToast.LongMessage(GetMessage());
                }
                else if ((e.SelectedItem as MasterPageItem).Id == 3)
                {
                    var action = await DisplayActionSheet("Choose Difficulty level", "Cancel", null, GetButtons(2));

                    if (action == null || action == "Cancel") return;
                    var newDifficulyLevel = (DifficulyLevel)Enum.Parse(typeof(DifficulyLevel), action.Replace("» ", "").Trim());
                    if (newDifficulyLevel == currentDifficulyLevel) return;

                    currentDifficulyLevel = newDifficulyLevel;
                    RebuildMenu(3, currentDifficulyLevel.ToString());
                    //Detail = new NavigationPage(new Test1(currentBoardSize, currentFirstMove));
                    XFToast.LongMessage(GetMessage());
                }
                else if ((e.SelectedItem as MasterPageItem).Id == 4)
                {
                    var action = await DisplayActionSheet("Choose who should move first", "Cancel", null, GetButtons(3));

                    if (action == null || action == "Cancel") return;
                    action = action.Replace("» ", "").Trim();
                    bool newFirstMove = (action == "CPU");
                    if (newFirstMove == currentFirstMove) return;

                    currentFirstMove = newFirstMove;
                    RebuildMenu(4, action);
                    //Detail = new NavigationPage(new Test1(currentBoardSize, firstMove));
                    XFToast.LongMessage(GetMessage());
                }
            }
            catch(Exception ex)
            {

            }            
        }

        string GetMessage()
        {
            var message = entry.Text;
            if (string.IsNullOrEmpty(message))
            {
                message = entry.Placeholder;
            }
            return message;
        }
    }

    public class MasterPageItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CurrentValue { get; set; }
        public string Icon { get; set; }
        public Type TargetType { get; set; }
    }

    public interface IMessage
    {
        void LongAlert(string message);
        void ShortAlert(string message);
    }

    public static class XFToast
    {
        public static void ShortMessage(string message)
        {
            DependencyService.Get<IMessage>().ShortAlert(message);
        }

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
}
