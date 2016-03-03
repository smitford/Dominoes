using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Dominoes.AI;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Dominoes.GUI
{
    class TileModelControler
    {
        private Grid _parentGrid;
        double tileWidth;
        double tileHeight;
        public List<TileModel> GameTableTileModels { get; private set; }
        public List<TileModel> UserBaseTileModels { get; private set; }
        public delegate void TilePickHandler(Node node);
        public event TilePickHandler TilePicked;

        public TileModelControler(Grid grid)
        {
            GameTableTileModels = new List<TileModel>();
            UserBaseTileModels = new List<TileModel>();

            _parentGrid = grid;
            tileWidth = (new TileModel()).Tile.Width;
            tileHeight = (new TileModel()).Tile.Height;
        }

        public TileModel SetTileParameters(TileModel tileModel, Node node, Point point, Side parentTileSide, Side childTileSide)
        {
            tileModel.CurrentNode = node;
            int angle = 0;
            if (parentTileSide != Side.Center)
            {
                angle = 2 - ((int)parentTileSide) + ((int)childTileSide);
            }
            tileModel.Angle = angle%4;

            var offset = tileModel.ConnectorOffset(childTileSide);
            point.X = point.X + offset.X;
            point.Y = point.Y + offset.Y;
            tileModel.Center = point;
            
            return tileModel;
        }

        public TileModel AddTileToGame(Node node, Point point, Side parentTileSide, Side childTileSide)
        {
            
            TileModel tileModel = new TileModel();
            SetTileParameters(tileModel,node, point, parentTileSide, childTileSide);

            _parentGrid.Children.Add(tileModel);
            GameTableTileModels.Add(tileModel);
            //_parentGrid.Children.Add(new Ellipse { Margin = new Thickness(point.X - 4, point.Y - 4, 0, 0), Fill = Brushes.Aqua, Width = 8, Height = 8, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left });
            return tileModel;
        }

        public void AddTileToUserBase(Tile tile, Point point)
        {
            TileModel tileModel = new TileModel();
            var node = new Node { CurrentTile = tile };
            tileModel.CurrentNode = node;
            tileModel.VerticalAlignment = VerticalAlignment.Top;
            tileModel.HorizontalAlignment = HorizontalAlignment.Left;
            var offset = tileModel.SideCoords(Side.Center);
            point = (Point)(point - offset);
            tileModel.Margin = new Thickness(point.X, point.Y, 0, 0);
            tileModel.MouseDown += TileModel_MouseDown;
            UserBaseTileModels.Add(tileModel);
            _parentGrid.Children.Add(tileModel);

        }

        private void TileModel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (var tileModel in UserBaseTileModels)
            {
                tileModel.Rect.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff3399ff"));
            }
            ((TileModel)sender).Rect.Fill = Brushes.Red;
            if (TilePicked != null)
            {
                TilePicked(((TileModel)sender).CurrentNode);
            }
        }

    }
}
