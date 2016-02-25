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
        public List<TileModel> TileModels { get; private set; }

        public TileModelControler(Grid grid)
        {
            TileModels = new List<TileModel>();
            _parentGrid = grid;
            tileWidth = (new TileModel()).Tile.Width;
            tileHeight = (new TileModel()).Tile.Height;
        }

        public void AddNewTile(Node node, Point point, Side parentTileSide, Side childTileSide)
        {
            TileModel tileModel = new TileModel();
            tileModel.CurrentNode = node;
            double angle;
            if (parentTileSide == Side.Center)
            {
                angle = 0;
            }
            else
            {
                angle = ((180 - 90 * (int)parentTileSide) + 90 * (int)childTileSide) % 360;
            }
            tileModel.RenderTransform = new RotateTransform(angle);
            //tileModel.RenderTransform = new RotateTransform(180, tileWidth / 2, tileHeight / 2);
            tileModel.Angle = angle;

            var offset = tileModel.SideCoords(childTileSide);
            point = (Point)(point - offset);
            tileModel.Margin = new Thickness(point.X, point.Y, 0, 0);

            _parentGrid.Children.Add(tileModel);
            TileModels.Add(tileModel);
        }
    }
}
