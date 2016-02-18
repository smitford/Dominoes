using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes.AI
{
    public class Tile
    {
        public Tile() { }

        public Tile(int leftEnd, int rightEnd)
        {
            LeftEnd = leftEnd;
            RightEnd = rightEnd;
        }

        public int LeftEnd { get; set; }

        public int RightEnd { get; set; }


        public bool IsDouble()
        {
            return LeftEnd == RightEnd;
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

        public static Tile MinimalTile(List<Tile> tiles)
        {
            var minimal = tiles.Min();
            return minimal;
        }

        public static List<Tile> GenerateTiles()
        {
            var tiles = new List<Tile>();
            for (int i = 0; i < 6; i++)
            {
                for (int j = i; j < 6; j++)
                {
                    var tile = new Tile(i, j);
                    tiles.Add(tile);
                }
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

        public static bool operator >(Tile t1, Tile t2)
        {
            if (t1.IsDouble() && t2.IsDouble())
            {
                return t1.LeftEnd > t2.RightEnd;
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
                return t1.LeftEnd < t2.RightEnd;
            }
            else
            {
                throw new Exception("Tile is not double");
            }
        }

    }
}
