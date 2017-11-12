using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;

namespace DotsAndBoxesFun
{
    public class Player
    {
        public int Score { get; set; }
        public Color BackgroundColor { get; set; }
        public string SoundFile { get; set; }
        public string Name { get; set; }
    }

    public static class Constants
    {
        public static bool restartGame = false;
        public static Color compColor = Color.FromHex("#D13A1B");
        public static Color playerColor = Color.FromHex("#12B504");
        public static Color compPrevColor = Color.FromHex("#DBB312");
        public static int StarRequired = 5;
        public static bool IsFilled(this BoxView shape)
        {
            return shape.BackgroundColor == compColor || shape.BackgroundColor == playerColor || shape.BackgroundColor == compPrevColor;
        }
    }

    public enum Turn
    {
        Player1,
        Player2
    };

    public class House
    {
        public Dictionary<string, Edge> shapeDict { get; set; } = new Dictionary<string, Edge>();
        private int count = 0;

        public House(int boardSize)
        {
            count = boardSize;
        }

        public int Id { get; set; }
        public Xamarin.Forms.BoxView Grid { get; set; }
        public int FilledCount { get; set; }        

        public Edge TopEdge
        {
            get
            {
                if (4 * (Id - count) - 1 < 0)
                    return GetValue($"_{4 * Id - 3}");
                return GetValue($"_{4*(Id - count) - 1}_{4*Id - 3}");
            }
        }

        private Edge GetValue(string id)
        {
            return shapeDict.Values.Where(x => x.Name == id).FirstOrDefault();
        }
        public Edge DownEdge
        {
            get
            {
                if (4 * (Id + count) - 3 > (4*count*count))
                    return GetValue($"_{4 * Id - 1}");
                return GetValue($"_{4 * Id - 1}_{4 * (Id + count) - 3}");
            }
        }
        public Edge LeftEdge
        {
            get
            {
                if (Id % count == 1)
                    return GetValue($"_{4 * Id}");
                return GetValue($"_{4 * Id - 6}_{4 * Id}");
            }
        }
        public Edge RightEdge
        {
            get
            {
                if (Id % count == 0)
                    return GetValue($"_{4 * Id - 2}");
                return GetValue($"_{4 * Id - 2}_{4 * (Id + 1)}");
            }
        }

        public List<Edge> Edges
        {
            get
            {
                return new List<Edge>
                {
                    TopEdge, RightEdge, DownEdge, LeftEdge
                };
            }
        }

        public void Fill(Color color)
        {
            Grid.BackgroundColor = color;
        }
    }

    public class Edge
    {
        public string Name { get; set; }
        public House House1 { get; set; }
        public House House2 { get; set; }
        public BoxView Shape { get; set; }

        public bool IsFilled
        {
            get
            {
                return Shape.BackgroundColor == Constants.playerColor || Shape.BackgroundColor == Constants.compColor || Shape.BackgroundColor == Constants.compPrevColor;
            }
        }

        public void Fill(Color color)
        {
            Shape.BackgroundColor = color;
        }
    }

    public class ChainManager
    {
        public List<Chain> ChainList { get; set; } = new List<Chain>();
    }

    public class Chain
    {
        public Chain()
        {

        }
        public Chain(List<House> houses)
        {
            BoxList.AddRange(houses);
        }
        public List<House> BoxList { get; set; } = new List<House>();

        public int Count { get { return BoxList.Count; } }
    }
}
