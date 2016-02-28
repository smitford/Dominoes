using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes.AI
{
    public class Tile : IComparable
    {
        public Tile() { }

        public Tile(int topEnd, int bottomEnd)
        {
            TopEnd = topEnd;
            BottomEnd = bottomEnd;
        }

        public int TopEnd { get; set; }

        public int BottomEnd { get; set; }

        public int GetSideValue(Side side)
        {
            if (side == Side.Top)
            {
                return TopEnd;
            }
            else if (side == Side.Bottom)
            {
                return BottomEnd;
            }
            else if (this.IsDouble() && (side == Side.Left || side == Side.Right))
            {
                return BottomEnd;
            }
            throw new Exception("Not existing end");
        }

        public bool IsDouble()
        {
            return TopEnd == BottomEnd;
        }

        public static List<Tile> DoubleTiles(List<Tile> tiles)
        {
            var doubleTiles = new List<Tile>();
            foreach (var tile in tiles)
            {
                if (tile.IsDouble())
                {
                    doubleTiles.Add(tile);
                }
            }
            return doubleTiles;
        }

        public static Tile MinimalTile(List<Tile> t)
        {
            List<Tile> tiles = new List<Tile>();
            tiles.AddRange(t);
            tiles.RemoveAll(x => !x.IsDouble());
            if (tiles.Count > 0)
            {
                var minimal = tiles.Min();
                return minimal;
            }
            else {
                return null;
            }
        }

        public static List<Tile> GenerateTiles()
        {
            var tiles = new List<Tile>();
            for (int i = 0; i <= 6; i++)
            {
                for (int j = i; j <= 6; j++)
                {
                    var tile = new Tile(i, j);
                    tiles.Add(tile);
                }
            }
            // Shuffle tiles
            var rand = new Random();
            int n = tiles.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                var value = tiles[k];
                tiles[k] = tiles[n];
                tiles[n] = value;
            }
            return tiles;
        }
        
        public static Tile PickTileFromBase(List<Tile> tileBase)
        {
            var tile = tileBase[new Random().Next(tileBase.Count)];
            tileBase.Remove(tile);
            return tile;
        }

        public static List<Tile> DealTiles(List<Tile> tileBase)
        {
            var dealTiles = new List<Tile>();
            for (int i = 0; i < 7; i++)
            {
                dealTiles.Add(PickTileFromBase(tileBase));
            }
            return dealTiles;
        }

        public int CompareTo(object obj)
        {
            var t1 = this;
            var t2 = (Tile)obj;

            if (t1 > t2)
            {
                return 1;
            }
            else if (t1 < t2)
            {
                return -1;
            }
            else 
            {
                return 0;
            }
        }

        public static bool operator >(Tile t1, Tile t2)
        {
            if (t1.IsDouble() && t2.IsDouble())
            {
                return t1.TopEnd > t2.BottomEnd;
            }
            else
            {
                throw new Exception("Tile is not double");
            }
        }

        public static bool operator <(Tile t1, Tile t2)
        {

            if (t1.IsDouble() && t2.IsDouble())
            {
                return t1.TopEnd < t2.BottomEnd;
            }
            else
            {
                throw new Exception("Tile is not double");
            }
        }

    }
}
