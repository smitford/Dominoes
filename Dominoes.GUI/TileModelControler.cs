using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Dominoes.AI;
using System.Windows.Media;

namespace Dominoes.GUI
{
    class TileModelControler
    {
        private Grid _parentGrid;
        double tileWidth;
        double tileHeight;
        public List<KeyValuePair<Node, Point>> Nodes { get; private set; }

        public TileModelControler(Grid grid)
        {
            Nodes = new List<KeyValuePair<Node, Point>>();
            _parentGrid = grid;
            tileWidth = (new TileModel()).Tile.Width;
            tileHeight = (new TileModel()).Tile.Height;
        }

        public void AddNewTile(Node node, Point point, Side parentTileSide, Side childTileSide)
        {
            TileModel tileModel = new TileModel();
            var offset = OffsetCoords(childTileSide);
            double angle = 180 + 90 * (((int)parentTileSide + 2) % 4) % 360;
            var cos = Math.Cos(Math.PI * (angle / 180));
            var sin = Math.Sin(Math.PI * (angle / 180));
            var center = new Point(point.X + (tileWidth * cos - tileHeight * sin)/2, point.Y + (tileHeight * cos + tileWidth * sin)/2);

            point.X = point.X - (offset.X * cos - offset.Y * sin);
            point.Y = point.Y - (offset.Y * cos + offset.X * sin);
            point = (Point)(point - ParentBias(parentTileSide, angle));

            tileModel.RenderTransformOrigin = new Point(0,0);
            tileModel.VerticalAlignment = VerticalAlignment.Top;
            tileModel.HorizontalAlignment = HorizontalAlignment.Left;
            tileModel.RenderTransform = new RotateTransform(angle);//,-35,-20);

            tileModel.Margin = new Thickness(point.X,point.Y, 0,0);
            _parentGrid.Children.Add(tileModel);
            Nodes.Add(new KeyValuePair<Node, Point>(node, center));
        }

        public Point ParentBias(Side tileSide, double angle)
        {
            double x = 0;
            double y = 0;
            var cos = Math.Cos(Math.PI * (angle / 180));
            var sin = Math.Sin(Math.PI * (angle / 180));
            if (tileSide== Side.Top|| tileSide == Side.Bottom)
            {
                y = (tileWidth / 2) * cos + (tileHeight / 2) * sin;

            }
            else if(tileSide == Side.Rigt || tileSide == Side.Left)
            {
                x = (tileWidth / 2) * sin + (tileHeight / 2) * cos;
            }
            else
            {
                x = 0;
                y = 0;
            }

            return new Point(x, y);
        }

        public Point OffsetCoords(Side tileSide)
        {
            double x = 0;
            double y = 0;
            switch (tileSide)
            {
                case Side.Top:
                    x = tileWidth / 2;
                    break;
                case Side.Bottom:
                    x = tileWidth / 2;
                    y = tileHeight; 
                    break;
                case Side.Left:
                    y = tileHeight / 2;
                    break;
                case Side.Rigt:
                    y = tileHeight / 2;
                    x = tileWidth;
                    break;
            }
            
            return new Point(x, y);
        }
    }
}
