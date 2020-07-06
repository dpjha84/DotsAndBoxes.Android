using DotsAndBoxesFun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DotsAndBoxesFun.Views
{
    public class GameBase : ContentPage
    {
        protected Label textPlayer1Name;
        protected Label textPlayer2Name;
        protected Label textPlayer1Score;
        protected Label textPlayer2Score;
        protected Turn turn = Turn.Player1;
        protected int player1Score = 0;
        protected int player2Score = 0;
        //protected static Color vertexColor = Color.FromHex("#000063");
        //protected static Color edgeDefaultColor = Color.FromHex("#FAEDF0");
        //protected static Color edgeHoverColor = Color.FromHex("#6CCBE6");
        protected static Color vertexColor = Color.FromHex("#000063");//0050EF
        protected static Color edgeDefaultColor = Color.FromHex("#FAEDF0");//FAEDF0
        //protected static Color edgeHoverColor = Color.FromHex("#6CCBE6");
        protected int count = 5;
        protected bool moveFirst = false;
        protected DifficultyLevel level = DifficultyLevel.Medium;
        protected const string player1Sound = "cool1.mp3";
        protected const string player2Sound = "cool2.mp3";
        protected const string houseMadeSound = "cool3.mp3";
        protected Dictionary<string, Edge> shapeDict = new Dictionary<string, Edge>();
        protected Dictionary<int, House> houseDict = new Dictionary<int, House>();
        protected int houseCount = 0;
        protected Random rnd = new Random();
        protected BoxView prevCompRect;
        protected static object lockObject = new object();
        protected int[,] moveArray = new int[,]
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

        public bool IsMute { get; set; }

        protected Player player1;
        protected Player player2;
        protected Player GetPlayer()
        {
            if (GetTurn() == Turn.Player1)
                return player1;
            return player2;
        }

        protected void InitialSetup(Grid grid, ToolbarItem btnSound)
        {
            grid.SizeChanged += (object sender, EventArgs e) =>
            {
                grid.HeightRequest = grid.Width;
            };
            grid.ColumnSpacing = 1;
            grid.RowSpacing = 1;

            player1 = new Player
            {
                BackgroundColor = Constants.playerColor,
                SoundFile = player1Sound,
                Name = "Player1"
            };
            player2 = new Player
            {
                BackgroundColor = Constants.compColor,
                SoundFile = player2Sound,
                Name = "Player2"
            };

            IsMute = GlobalSetting.IsMute;
            btnSound.Icon = IsMute ? "soundmute" : "sound";
        }

        protected void SetupControls(Label textPlayer1Name, Label textPlayer2Name, Label textPlayer1Score, Label textPlayer2Score, BoxView separator1, BoxView separator2)
        {
            textPlayer1Name.Text = GlobalSetting.Player1Name;
            textPlayer1Name.TextColor = Constants.playerColor;
            textPlayer2Name.TextColor = Constants.compColor;
            textPlayer1Score.TextColor = Constants.playerColor;
            textPlayer2Score.TextColor = Constants.compColor;
            separator1.HeightRequest = 1;
            separator1.BackgroundColor = Color.Black;
            separator1.VerticalOptions = LayoutOptions.Start;
            separator2.HeightRequest = 1;
            separator2.BackgroundColor = Color.Black;
            separator2.VerticalOptions = LayoutOptions.Start;
        }

        public void NewGame(Grid grid)
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
                            shape.ClassId = string.Format("house_{0}_{1}", i, j);

                            TouchEffect touchEffect1 = new TouchEffect();
                            touchEffect1.TouchAction += EdgeClicked;
                            shape.Effects.Add(touchEffect1);

                            //var tabGestureOrganiser1 = new TapGestureRecognizer();
                            //tabGestureOrganiser1.Tapped += EdgeClicked;
                            //shape.GestureRecognizers.Add(tabGestureOrganiser1);
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
                    //var tabGestureOrganiser = new TapGestureRecognizer();
                    //tabGestureOrganiser.Tapped += EdgeClicked;

                    TouchEffect touchEffect = new TouchEffect();
                    touchEffect.TouchAction += EdgeClicked;
                    shape.Effects.Add(touchEffect);

                    //shape.GestureRecognizers.Add(tabGestureOrganiser);
                    Xamarin.Forms.Grid.SetRow(shape, i);
                    Xamarin.Forms.Grid.SetColumn(shape, j);
                    grid.Children.Add(shape);
                }
            }
            MapHousesToEdges();
            if (moveFirst && !ClasicGameSetting.Is2Players) CompMove();
        }

        protected void MapHousesToEdges()
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

            for (int houseId = 1; houseId <= count * count; houseId++)
            {
                houseDict[houseId].shapeDict = shapeDict;
            }
        }

        //string tempColorClassId = null;

        //bool Validate(BoxView shape, TouchActionEventArgs e)
        //{
        //    if (e.Type != TouchActionType.Released) return false;
        //    if (e.Type == TouchActionType.Moved || e.Type == TouchActionType.Entered) return false;
        //    if (e.Type == TouchActionType.Pressed)
        //    {
        //        if ((tempColorClassId != null && tempColorClassId != shape.ClassId) ||
        //                shape.ClassId.StartsWith("house")) return false;
        //        shape.BackgroundColor = Color.DarkGreen;
        //        tempColorClassId = shape.ClassId;
        //        return false;
        //    }
        //    else if (e.Type == TouchActionType.Exited || e.Type == TouchActionType.Cancelled ||
        //        (e.Type == TouchActionType.Released && shape.BackgroundColor != Color.DarkGreen))
        //    {
        //        //if (!shape.ClassId.StartsWith("house"))
        //        //{
        //        Edge edge = shapeDict.Values.Where(x => x.Name == tempColorClassId).FirstOrDefault();
        //        edge.Fill(edgeDefaultColor);
        //        tempColorClassId = null;
        //        return false;
        //        //}
        //    }
        //    tempColorClassId = null;
        //    return true;
        //}

        BoxView FindNearestEdgeToProcess(object sender, TouchActionEventArgs e)
        {
            var shape = ((BoxView)sender);
            if (shape.IsFilled())
                return null;
            if (shape.ClassId.StartsWith("house"))
            {
                var pressedLocation = e.Location;
                var hs = shape.ClassId.Split('_');
                int houseId = ((Convert.ToInt32(hs[1]) - 1) / 2) * count + (Convert.ToInt32(hs[2]) + 1) / 2;
                var house = houseDict[houseId];
                if (house.FilledCount < 3)
                {
                    var leftX = pressedLocation.X;
                    var topY = pressedLocation.Y;
                    var rightX = shape.Bounds.Width - leftX;
                    var bottomY = shape.Bounds.Height - topY;

                    var min = Math.Min(leftX, Math.Min(topY, Math.Min(rightX, bottomY)));
                    if (min == leftX)
                        shape = house.LeftEdge.Shape;
                    else if (min == topY)
                        shape = house.TopEdge.Shape;
                    else if (min == rightX)
                        shape = house.RightEdge.Shape;
                    else
                        shape = house.DownEdge.Shape;
                    if (shape.IsFilled()) return null;
                }
            }
            return shape;
        }
        async void EdgeClicked(object sender, TouchActionEventArgs e)
        {
            if (e.Type != TouchActionType.Released) return;
            try
            {
                var shape = FindNearestEdgeToProcess(sender, e);
                if (shape == null) return;
                if (ClasicGameSetting.Is2Players || GetTurn() == Turn.Player1)
                {
                    if (prevCompRect != null)
                        prevCompRect.BackgroundColor = Constants.compColor;
                    bool found = false;
                    bool houseClicked = false;
                    var player = GetPlayer();
                    if (shape.ClassId.StartsWith("house"))
                    {
                        houseClicked = true;
                        var hs = shape.ClassId.Split('_');
                        int houseId = ((Convert.ToInt32(hs[1]) - 1) / 2) * count + (Convert.ToInt32(hs[2]) + 1) / 2;
                        if (houseDict[houseId].FilledCount == 3)
                        {
                            PlaySound(player.SoundFile);
                            shape.BackgroundColor = player.BackgroundColor;
                            var house = houseDict[houseId];
                            var edge = house.Edges.Where(x => x.IsFilled == false).FirstOrDefault();
                            edge.Fill(player.BackgroundColor);
                            edge.House1.FilledCount++;
                            if (edge.House1.FilledCount == 4)
                            {
                                edge.House1.Fill(player.BackgroundColor);
                                Animate(edge.House1.Grid);
                                UpdatePlayerScore();
                            }
                            if (edge.House2 != null)
                            {
                                edge.House2.FilledCount++;
                                if (edge.House2.FilledCount == 4)
                                {
                                    edge.House2.Fill(player.BackgroundColor);
                                    Animate(edge.House2.Grid);
                                    UpdatePlayerScore();
                                }
                            }
                        }
                        else
                        {
                            prevCompRect.BackgroundColor = Constants.compPrevColor;
                        }
                    }
                    else
                    {
                        PlaySound(player.SoundFile);
                        shape.BackgroundColor = player.BackgroundColor;
                        string[] splitted = shape.ClassId.Substring(1).Split('_');

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
                                house.Grid.BackgroundColor = player.BackgroundColor;
                                Animate(house.Grid, textPlayer1Score);
                                found = true;
                                UpdatePlayerScore();
                            }
                            else
                                house.FilledCount++;
                        }
                    }
                    if (level == DifficultyLevel.Hard)
                    {
                        Edge edge = shapeDict.Values.Where(x => x.Name == shape.ClassId).FirstOrDefault();
                        UpdateChains(edge);
                    }
                    if (!found && !houseClicked)
                        SetTurn((GetTurn() == Turn.Player1) ? Turn.Player2 : Turn.Player1);
                    ShowScore();

                    if (!ClasicGameSetting.Is2Players || this is ChallengeGame)
                    {
                        if (GetTurn() == Turn.Player2)
                        {
                            await CompMove();
                        }
                        else
                            CheckIfGameIsOver();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }        

        protected Turn GetTurn()
        {
            lock (lockObject)
            {
                return turn;
            }
        }

        protected void SetTurn(Turn t)
        {
            lock (lockObject)
            {
                turn = t;
            }
        }

        protected async void PlaySound(string fileName)
        {
            if (IsMute) return;
            DependencyService.Get<IAudio>().PlayAudioFile(fileName);
        }

        protected async Task CompMove()
        {
            ShowScore();
            await GetEdge();
            SetTurn(Turn.Player1);
        }

        protected void ShowScore()
        {
            if (GetTurn() == Turn.Player1)
            {
                textPlayer1Score.Text = $"{player1Score}* ";
                textPlayer2Score.Text = $" {player2Score}";
            }
            else
            {
                textPlayer2Score.Text = $" {player2Score}*";
                textPlayer1Score.Text = $"{player1Score} ";
            }
        }

        protected async void CheckIfGameIsOver()
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
                bool choice = false;
                if (this is ClassicGame)
                {
                    if (player1Score == player2Score)
                        choice = await DisplayAlert("Game over!", "Game is tied. Play another game?", "Yes", "No");
                    else
                        choice = await DisplayAlert("Game over!", player1Score > player2Score ? "You won! Play another game?" : "You lost. Play another game?", "Yes", "No");
                    if (choice)
                        new ClassicGameRunner().Start();
                }
                else
                {
                    choice = await DisplayAlert("Game over!", player1Score >= ChallengeGameSetting.TargetScore ?
                        "You won! Play next level?" : "Target score not achieved, You lost! Try again?", "Yes", "No");

                    if (player1Score >= ChallengeGameSetting.TargetScore)
                        ChallengeGameSetting.ChallengeLevel++;
                    if (choice)
                        new ChallengeGameRunner().Start();
                }
            }
        }

        protected List<Chain> GetAvailableList(List<Chain> current)
        {
            var list = new List<Chain>();
            foreach (var chain in current)
            {
                if (chain.BoxList.Any(h => h.FilledCount < 4))
                    list.Add(chain);
            }
            return list;
        }

        async Task GetEdge()
        {
            try
            {
                if (level == DifficultyLevel.Easy)
                {
                    GetEdgeEasy();
                    return;
                }

                await Task.Delay(1200);
                SetTurn(Turn.Player2);
                ShowScore();
                List<Edge> list;
                for (int i = 0; i < moveArray.GetLength(0); i++)
                {
                    if (level == DifficultyLevel.Hard)
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
                            var newList = GetAvailableList(finalList);
                            foreach (var chain in newList)
                            {
                                if (chain.BoxList.Count == 2)
                                {
                                    foreach (var e1 in chain.BoxList[0].Edges)
                                    {
                                        foreach (var e2 in chain.BoxList[1].Edges)
                                        {
                                            if (e1.Name == e2.Name && !e1.IsFilled)
                                            {
                                                UpdateEdge(e1);
                                                return;
                                            }
                                        }
                                    }
                                }
                                foreach (var box in chain.BoxList)
                                {
                                    foreach (var e in GetEmptyEdges(box))
                                    {
                                        UpdateEdge(e);
                                        return;
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

        protected List<Chain> Merge(List<Chain> ll, out bool noOverlap)
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

        protected List<Edge> GetList(int house1FilledCount, int house2FilledCount, int preferenceCount)
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

        protected void UpdateEdge(Edge edge)
        {
            PlaySound(player2Sound);
            if (prevCompRect != null)
                prevCompRect.BackgroundColor = Constants.compColor;
            edge.House1.FilledCount++;
            prevCompRect = edge.Shape;

            edge.Shape.BackgroundColor = Constants.compColor;
            if (edge.House2 != null)
                edge.House2.FilledCount++;
            bool houseMade = false;
            if (edge.House1.FilledCount == 4)
            {
                player2Score++;
                edge.House1.Grid.BackgroundColor = Constants.compColor;
                houseMade = true;
                Animate(edge.House1.Grid);
            }
            if (edge.House2 != null && edge.House2.FilledCount == 4)
            {
                player2Score++;
                edge.House2.Grid.BackgroundColor = Constants.compColor;
                houseMade = true;
                Animate(edge.House2.Grid);
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
                UpdateChains(edge);
                SetTurn(Turn.Player1);
            }
            ShowScore();
            CheckIfGameIsOver();
        }

        protected async void Animate(BoxView house, Label text = null)
        {
            var storyboard = new Animation();
            var rotation = new Animation(callback: d => house.Rotation = d,
                                          start: house.Rotation,
                                          end: house.Rotation + 180,
                                          easing: Easing.Linear);
            storyboard.Add(0, 1, rotation);
            storyboard.Commit(house, "Loop", length: 500);
            PlaySound(houseMadeSound);
        }

        ChainManager chainManager = new ChainManager();

        protected void UpdateChains(Edge edge)
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
                //        box.Grid.BackgroundColor = colors[i];
                //    }
                //    i++;
                //}
            }
            catch (Exception ex)
            {

            }
        }

        protected Chain GetChain(House house)
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

        protected List<Edge> GetEmptyEdges(House house)
        {
            List<Edge> emptyEdges = new List<Edge>();

            foreach (var edge in house.Edges)
            {
                if (!edge.IsFilled)
                    emptyEdges.Add(edge);
            }
            return emptyEdges;
        }


        protected void UpdatePlayerScore()
        {
            if (GetTurn() == Turn.Player1)
                player1Score++;
            else
                player2Score++;
        }

        protected void btnHome_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new HomePage());
        }

        protected void btnNewGame_Clicked(object sender, EventArgs e)
        {
            if (this is ClassicGame)
            {
                new ClassicGameRunner().Start();
            }
            else
            {
                new ChallengeGameRunner().Start();
            }
        }

        protected void btnSound_Clicked(object sender, EventArgs e)
        {
            ToolbarItem btnSound = (ToolbarItem)sender;
            IsMute = !IsMute;
            GlobalSetting.IsMute = IsMute;
            string newValue = IsMute ? "Off" : "On";
            btnSound.IconImageSource = IsMute ? "soundmute" : "sound";
            XFToast.LongMessage($"Sound is {newValue}");
        }

        protected void btnFeedback_Clicked(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://play.google.com/store/apps/details?id=com.game.dotsandboxesplus"));
        }
    }
}
