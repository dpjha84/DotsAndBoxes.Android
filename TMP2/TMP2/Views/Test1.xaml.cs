using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace DotsAndBoxesFun.Views
{
    public partial class Test1 : ContentPage
    {
        Turn turn = Turn.Player1;
        int player1Score = 0;
        int player2Score = 0;
        private static Color vertexColor = Color.FromHex("#0A4008");
        private static Color edgeDefaultColor = Color.FromHex("#FAEDF0");
        private static Color edgeHoverColor = Color.FromHex("#6CCBE6");
        int count = 5;
        bool moveFirst = false;
        DifficulyLevel level = DifficulyLevel.Medium;
        const string player1Sound = "cool1.mp3";
        const string player2Sound = "cool2.mp3";
        Dictionary<string, Edge> shapeDict = new Dictionary<string, Edge>();
        Dictionary<int, House> houseDict = new Dictionary<int, House>();
        int houseCount = 0;
        Random rnd = new Random();
        BoxView prevCompRect;
        static object lockObject = new object();
        int[,] moveArray = new int[,]
        {
            {3, 3, 3},
            {3, 2, 3},
            {3, 0, 3},
            {3, 1, 3},
            {0, 0, 0},
            {0, 1, 0},
            {1, 1, 1},
            {2, 0, 0},
            {2, 1, 1},
            {2, 2, 2}
        };

        public Test1(int boardSize, DifficulyLevel level, bool moveFirst)
        {
            this.InitializeComponent();
            grid.SizeChanged += (object sender, EventArgs e) =>
            {
                grid.HeightRequest = grid.Width;
            };
            grid.ColumnSpacing = 3;
            grid.RowSpacing = 3;
            textPlayer1Score.TextColor = Constants.playerColor;
            textPlayer2Score.TextColor = Constants.compColor;
            separator1.HeightRequest = 1;
            separator1.BackgroundColor = Color.Black;
            separator1.VerticalOptions = LayoutOptions.Start;
            separator2.HeightRequest = 1;
            separator2.BackgroundColor = Color.Black;
            separator2.VerticalOptions = LayoutOptions.Start;
            count = boardSize;
            this.moveFirst = moveFirst;
            this.level = level;
            NewGame();
        }

        public Test1() : this(6, DifficulyLevel.Medium, false)
        {
        }

        Turn GetTurn()
        {
            lock (lockObject)
            {
                return turn;
            }
        }

        void SetTurn(Turn t)
        {
            lock (lockObject)
            {
                turn = t;
            }
        }

        public void NewGame()
        {
            ShowScore();
            for (int i = 0; i < count * 2 + 1; i++)
            {
                int c = i % 2 == 0 ? 1 : 3;
                var rd = new RowDefinition() { Height = new GridLength(c, GridUnitType.Star) };
                var cd = new ColumnDefinition() { Width = new GridLength(c, GridUnitType.Star) };
                grid.RowDefinitions.Add(rd);
                grid.ColumnDefinitions.Add(cd);
            }

            Xamarin.Forms.BoxView shape;
            string id = "";
            int last = count * 2 + 1;
            for (int i = 0; i < last; i++)
            {
                bool start = true;
                for (int j = 0; j < last; j++)
                {
                    if ((i + j) % 2 == 0)
                    {
                        shape = new Xamarin.Forms.BoxView() { BackgroundColor = vertexColor };
                        Xamarin.Forms.Grid.SetRow(shape, i);
                        Xamarin.Forms.Grid.SetColumn(shape, j);
                        grid.Children.Add(shape);
                        if (j % 2 != 0)
                        {
                            shape.BackgroundColor = edgeDefaultColor;
                            houseDict.Add(++houseCount, new House(count) { Id = houseCount, Grid = shape });
                        }
                        else
                            shape.ClassId = string.Format("vertex{0}{1}", i, j);
                        continue;
                    }
                    if (start)
                    {
                        if (i < last - 1)
                        {
                            if (i % 2 == 0)
                                id = ((i / 2) * (4 * count) + j).ToString();
                            else
                                id = ((i / 2) * (4 * count) + 4).ToString();
                        }
                        else
                            id = id = (((i - 1) / 2) * (4 * count) + 3).ToString();
                        start = false;
                    }
                    else
                    {
                        var splitted = id.Split('_');
                        id = (Convert.ToInt32(splitted[splitted.Length - 1]) + 4).ToString();
                    }
                    if (i > 0 && j > 0 && i < last - 1)
                    {
                        if (j < last - 1)
                        {
                            if (i % 2 != 0)
                                id = (Convert.ToInt32(id) - 6).ToString() + "_" + id;
                            else
                                id = (Convert.ToInt32(id) - (4 * count - 2)).ToString() + "_" + id;
                        }
                        else
                            id = (Convert.ToInt32(id) - 6).ToString();
                    }
                    shape = new Xamarin.Forms.BoxView()
                    {
                        BackgroundColor = edgeDefaultColor
                    };
                    shape.ClassId = "_" + id;
                    var ids = id.Split('_');
                    foreach (var item in ids)
                    {
                        var edge = new Edge() { Shape = shape, Name = shape.ClassId };
                        shapeDict.Add(item, edge);
                    }
                    var tabGestureOrganiser = new TapGestureRecognizer();
                    tabGestureOrganiser.Tapped += EdgeClicked;
                    shape.GestureRecognizers.Add(tabGestureOrganiser);
                    Xamarin.Forms.Grid.SetRow(shape, i);
                    Xamarin.Forms.Grid.SetColumn(shape, j);
                    grid.Children.Add(shape);
                }
            }
            MapHousesToEdges();
            if (moveFirst) CompMove();
        }

        private void MapHousesToEdges()
        {
            foreach (var edge in shapeDict)
            {
                string[] splitted = edge.Value.Name.Substring(1).Split('_');
                int houseId = (int)Math.Ceiling(Convert.ToDouble(splitted[0]) / 4);
                edge.Value.House1 = houseDict[houseId];
                if (splitted.Length > 1)
                {
                    houseId = (int)Math.Ceiling(Convert.ToDouble(splitted[1]) / 4);
                    edge.Value.House2 = houseDict[houseId];
                }
            }

            for (int houseId = 1; houseId <= count*count; houseId++)
            {
                houseDict[houseId].shapeDict = shapeDict;
            }
        }

        async void PlaySound(string fileName)
        {
            //MediaElement mysong = new MediaElement();
            //Windows.Storage.StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            //Windows.Storage.StorageFile file = await folder.GetFileAsync(fileName);
            //var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            //mysong.SetSource(stream, file.ContentType);
            //mysong.Play();
        }

        async Task CompMove()
        {
            ShowScore();
            await GetEdge();
            SetTurn(Turn.Player1);
        }

        private void ShowScore()
        {
            if (GetTurn() == Turn.Player1)
            {
                textPlayer1Score.Text = player1Score.ToString() + "*";
                textPlayer2Score.Text = player2Score.ToString();
            }
            else
            {
                textPlayer2Score.Text = player2Score.ToString() + "*";
                textPlayer1Score.Text = player1Score.ToString();
            }
        }

        private async void CheckIfGameIsOver()
        {
            bool end = true;
            foreach (var item in shapeDict)
            {
                if (item.Value.Shape.BackgroundColor == edgeDefaultColor)
                {
                    end = false;
                    break;
                }
            }
            if (end)
            {
                prevCompRect.BackgroundColor = Constants.compColor;
                if (player1Score == player2Score)
                    DisplayAlert("Game over!", "Game is tied. Try again!", "OK");
                else
                    DisplayAlert("Game over!", player1Score > player2Score ? "You won!" : "You lost. Try again!", "OK");
            }
        }

        async Task GetEdge()
        {
            try
            {            
                if (level == DifficulyLevel.Easy)
                {
                    GetEdgeEasy();
                    return;
                }

                await Task.Delay(1200);
                PlaySound(player2Sound);
                SetTurn(Turn.Player2);
                ShowScore();
                List<Edge> list;
                for (int i = 0; i < moveArray.GetLength(0); i++)
                {
                    if (level == DifficulyLevel.Hard)
                    {
                        if ((moveArray[i, 0] == 2 && moveArray[i, 1] == 0) ||
                            (moveArray[i, 0] == 2 && moveArray[i, 1] == 1) ||
                            (moveArray[i, 0] == 2 && moveArray[i, 1] == 2))
                        {
                            bool noOverlap = false;
                            var final = Merge(chainManager.ChainList, out noOverlap);
                            while (noOverlap)
                            {
                                noOverlap = false;
                                final = Merge(final, out noOverlap);
                            }
                            var finalList = final.OrderBy(c => c.Count).Where(c => c.Count > 0).ToList();
                            foreach (var chain in finalList)
                            {
                                foreach (var box in chain.BoxList)
                                {
                                    foreach (var e in GetEmptyEdges(box))
                                    {
                                        if (!e.IsFilled)
                                        {
                                            UpdateEdge(e);
                                            return;
                                        }
                                    }
                                }
                            }                        
                        }
                    }
                    list = GetList(moveArray[i, 0], moveArray[i, 1], moveArray[i, 2]);
                    if (list.Count > 0)
                    {
                        UpdateEdge(list[rnd.Next(0, list.Count - 1)]);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private List<Chain> Merge(List<Chain> ll, out bool noOverlap)
        {
            noOverlap = false;
            bool found = false;
            int k = 0;
            List<Chain> ll2 = new List<Chain>();
            for (var a = 0; a < chainManager.ChainList.Count; a++)
            {
                found = false;
                int j = a + 1;
                for (j = a + 1; j < chainManager.ChainList.Count; j++)
                {
                    if (chainManager.ChainList[a].BoxList.Intersect(chainManager.ChainList[j].BoxList).ToList().Count > 0)
                    {
                        noOverlap = true;
                        found = true;
                        k = j;
                        ll2.Add(new Chain(chainManager.ChainList[a].BoxList.Union(chainManager.ChainList[j].BoxList).ToList()));
                        chainManager.ChainList[j].BoxList.Clear();
                    }
                }
                if (!found)
                {
                    if (a != k || (a == 0 && k == 0))
                    {
                        k = j;
                        ll2.Add(new Chain(chainManager.ChainList[a].BoxList.ToList()));
                    }
                }
            }
            return ll2;
        }

        async Task GetEdgeEasy()
        {
            await Task.Delay(1200);
            PlaySound(player2Sound);
            SetTurn(Turn.Player2);
            ShowScore();
            List<Edge> list;
            for (int i = 0; i < 4; i++)
            {
                list = GetList(moveArray[i, 0], moveArray[i, 1], moveArray[i, 2]);
                if (list.Count > 0)
                {
                    UpdateEdge(list[rnd.Next(0, list.Count - 1)]);
                    return;
                }
            }
            for (int i = 4; i < moveArray.GetLength(0) - 1; i++)
            {
                int j = rnd.Next(i, moveArray.GetLength(0));
                var temp0 = moveArray[i, 0];
                var temp1 = moveArray[i, 1];
                var temp2 = moveArray[i, 2];
                moveArray[i, 0] = moveArray[j, 0];
                moveArray[i, 1] = moveArray[j, 1];
                moveArray[i, 2] = moveArray[j, 2];
                moveArray[j, 0] = temp0;
                moveArray[j, 1] = temp1;
                moveArray[j, 2] = temp2;
            }
            for (int i = 4; i < moveArray.GetLength(0); i++)
            {
                list = GetList(moveArray[i, 0], moveArray[i, 1], moveArray[i, 2]);
                if (list.Count > 0)
                {
                    UpdateEdge(list[rnd.Next(0, list.Count - 1)]);
                    return;
                }
            }
        }

        private List<Edge> GetList(int house1FilledCount, int house2FilledCount, int preferenceCount)
        {
            List<Edge> list = new List<Edge>();
            foreach (var item in shapeDict)
            {
                if (item.Value.IsFilled)
                    continue;
                if (item.Value.House2 == null)
                {
                    if (item.Value.House1.FilledCount == preferenceCount)
                        list.Add(item.Value);
                }
                else
                {
                    if ((item.Value.House1.FilledCount == house1FilledCount && item.Value.House2.FilledCount == house2FilledCount) ||
                        (item.Value.House1.FilledCount == house2FilledCount && item.Value.House2.FilledCount == house1FilledCount))
                        list.Add(item.Value);
                }
            }
            return list;
        }

        private void UpdateEdge(Edge edge)
        {
            if (prevCompRect != null)
                prevCompRect.BackgroundColor = Constants.compColor;
            edge.House1.FilledCount++;
            prevCompRect = edge.Shape;

            edge.Shape.BackgroundColor = Constants.compColor;
            //edge.Shape = 5;
            if (edge.House2 != null)
                edge.House2.FilledCount++;
            bool houseMade = false;
            if (edge.House1.FilledCount == 4)
            {
                player2Score++;
                edge.House1.Grid.BackgroundColor = Constants.compColor;
                houseMade = true;
            }
            if (edge.House2 != null && edge.House2.FilledCount == 4)
            {
                player2Score++;
                edge.House2.Grid.BackgroundColor = Constants.compColor;
                houseMade = true;
            }
            if (houseMade)
            {
                edge.Shape.BackgroundColor = Constants.compColor;
                UpdateChains(edge);
                GetEdge();
            }
            else
            {
                edge.Shape.BackgroundColor = Constants.compPrevColor;
                turn = Turn.Player1;
                UpdateChains(edge);
            }            
            ShowScore();
            CheckIfGameIsOver();
        }

        ChainManager chainManager = new ChainManager();

        private void UpdateChains(Edge edge)
        {
            try
            {
                var house1 = edge.House1;
                var house2 = edge.House2;
                bool found = false;
                if (house1.FilledCount == 2)
                {
                    var emptyEdges = GetEmptyEdges(house1);
                    foreach (var emptyEdge in emptyEdges)
                    {
                        if (emptyEdge.House1.FilledCount == 2)
                        {
                            var chain = GetChain(emptyEdge.House1);
                            if (chain != null)
                            {
                                found = true;
                                //var dupChain = chainManager.ChainList.Where(c => c.BoxList.Contains(house1)).FirstOrDefault();
                                //if (dupChain != null)
                                //{
                                //    dupChain.BoxList.Concat(chain.BoxList);
                                //    chainManager.ChainList.Remove(chain);
                                //}
                                if (!chain.BoxList.Contains(house1))
                                {
                                    chain.BoxList.Add(house1);
                                }
                            }
                        }
                        if (emptyEdge.House2 != null && emptyEdge.House2.Id != house1.Id && emptyEdge.House2.FilledCount == 2)
                        {
                            var chain = GetChain(emptyEdge.House2);
                            if (chain != null)
                            {
                                found = true;
                                //var dupChain = chainManager.ChainList.Where(c => c.BoxList.Contains(house1)).FirstOrDefault();
                                //if (dupChain != null)
                                //{
                                //    dupChain.BoxList.Concat(chain.BoxList);
                                //    chainManager.ChainList.Remove(chain);
                                //}
                                if (!chain.BoxList.Contains(house1))
                                {
                                    chain.BoxList.Add(house1);
                                }
                            }
                        }
                    }
                    if (!found)
                    {
                        var chain = new Chain();
                        chain.BoxList.Add(house1);
                        chainManager.ChainList.Add(chain);
                    }
                }
                found = false;
                if (house2 != null && house2.FilledCount == 2)
                {
                    var emptyEdges = GetEmptyEdges(house2);
                    foreach (var emptyEdge in emptyEdges)
                    {
                        if (emptyEdge.House1.Id != house2.Id && emptyEdge.House1.FilledCount == 2)
                        {
                            var chain = GetChain(emptyEdge.House1);
                            if (chain != null)
                            {
                                found = true;
                                //var dupChain = chainManager.ChainList.Where(c => c.BoxList.Contains(house2)).FirstOrDefault();
                                //if (dupChain != null)
                                //{
                                //    dupChain.BoxList.Concat(chain.BoxList);
                                //    chainManager.ChainList.Remove(chain);
                                //}
                                if (!chain.BoxList.Contains(house2))
                                {
                                    chain.BoxList.Add(house2);
                                }
                            }
                        }
                        if (emptyEdge.House2 != null && emptyEdge.House2.Id != house2.Id && emptyEdge.House2.FilledCount == 2)
                        {
                            var chain = GetChain(emptyEdge.House2);
                            if (chain != null)
                            {
                                found = true;
                                //var dupChain = chainManager.ChainList.Where(c => c.BoxList.Contains(house2)).FirstOrDefault();
                                //if (dupChain != null)
                                //{
                                //    dupChain.BoxList.Concat(chain.BoxList);
                                //    chainManager.ChainList.Remove(chain);
                                //}
                                if (!chain.BoxList.Contains(house2))
                                {
                                    chain.BoxList.Add(house2);
                                }
                            }
                        }
                    }
                    if (!found)
                    {
                        var chain = new Chain();
                        chain.BoxList.Add(house2);
                        chainManager.ChainList.Add(chain);
                    }
                }

                bool noOverlap = false;
                chainManager.ChainList = Merge(chainManager.ChainList, out noOverlap);
                while (noOverlap)
                {
                    noOverlap = false;
                    chainManager.ChainList = Merge(chainManager.ChainList, out noOverlap);
                }

                //List<Color> colors = new List<Color> { Color.Fuchsia, Color.Gray, Color.Green, Color.Lime, Color.Maroon, Color.Navy };
                //int i = 0;
                //foreach (var item in chainManager.ChainList)
                //{
                //    foreach (var box in item.BoxList)
                //    {
                //        //box.Grid.BackgroundColor = colors[i];
                //    }
                //    i++;
                //}
            }
            catch (Exception ex)
            {

            }
        }

        private Chain GetChain(House house)
        {
            foreach (var chain in chainManager.ChainList)
            {
                foreach (var box in chain.BoxList)
                {
                    if (box.Id == house.Id)
                        return chain;
                }
            }
            return null;
        }

        private List<Edge> GetEmptyEdges(House house)
        {
            List<Edge> emptyEdges = new List<Edge>();
            foreach (var edge in house.Edges)
            {
                if (!edge.IsFilled)
                    emptyEdges.Add(edge);
            }
            return emptyEdges;
        }

        void EdgeClicked(object sender, EventArgs e)
        {
            try
            {
                if (GetTurn() == Turn.Player2 || ((BoxView)sender).IsFilled()) return;
                var shape = ((BoxView)sender);
                PlaySound(player1Sound);
                if (prevCompRect != null)
                    prevCompRect.BackgroundColor = Constants.compColor;
                shape.BackgroundColor = Constants.playerColor;
                string[] splitted = shape.ClassId.Substring(1).Split('_');
                bool found = false;
                foreach (var id in splitted)
                {
                    int num = Convert.ToInt32(id);
                    int houseId = (num % 4 == 0) ? num / 4 : num / 4 + 1;
                    var house = houseDict[houseId];
                    if (shapeDict[(4 * houseId).ToString()].IsFilled &&
                        shapeDict[(4 * houseId - 1).ToString()].IsFilled &&
                        shapeDict[(4 * houseId - 2).ToString()].IsFilled &&
                        shapeDict[(4 * houseId - 3).ToString()].IsFilled)
                    {
                        house.FilledCount = 4;
                        house.Grid.BackgroundColor = Constants.playerColor;
                        found = true;
                        if (GetTurn() == Turn.Player1)
                            player1Score++;
                        else
                            player2Score++;
                    }
                    else
                        house.FilledCount++;
                }
                if (level == DifficulyLevel.Hard)
                {
                    Edge edge = shapeDict.Values.Where(x => x.Name == shape.ClassId).FirstOrDefault();
                    UpdateChains(edge);
                }
                    
                if (!found)
                    SetTurn((GetTurn() == Turn.Player1) ? Turn.Player2 : Turn.Player1);
                ShowScore();
                if (GetTurn() == Turn.Player2)
                    CompMove();
                CheckIfGameIsOver();
            }
            catch (Exception ex)
            {

            }
        }        
    }
}
