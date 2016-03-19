using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes.AI
{
    [DebuggerDisplay("{TopEnd} || {BottomEnd}")]
    public class Tile : IComparable
    {
        public Tile() { }

        public Tile(int topEnd, int bottomEnd) 
        {
            TopEnd = topEnd;
            BottomEnd = bottomEnd;
        }

        /// <summary>
        /// Number of dots on top end
        /// </summary>
        public int TopEnd { get; set; }

        /// <summary>
        /// Number of dots on bottom end
        /// </summary>
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

        /// <summary>
        /// If bottom == top, then true
        /// </summary>
        /// <returns></returns>
        public bool IsDouble()
        {
            return TopEnd == BottomEnd;
        }

        /// <summary>
        /// Return minimal double from tile list
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
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
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Generate and shuffle tiles
        /// </summary>
        /// <returns></returns>
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
        
        /// <summary>
        /// Picks tile from base and remove it from base list
        /// </summary>
        /// <param name="tileBase"></param>
        /// <returns></returns>
        public static Tile PickTileFromBase(List<Tile> tileBase)
        {
            var tile = tileBase[0];
            tileBase.Remove(tile);
            return tile;
        }

        /// <summary>
        /// Deals a list of tiles to user/ai 
        /// </summary>
        /// <param name="tileBase"></param>
        /// <returns></returns>
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
