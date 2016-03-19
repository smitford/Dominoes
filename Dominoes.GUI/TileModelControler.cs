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
    /// <summary>
    /// Control tiles in interface
    /// </summary>
    class TileModelControler
    {
        private Grid _parentGrid; 
        double tileWidth;
        double tileHeight;

        /// <summary>
        /// List of tiles on the table
        /// </summary>
        public List<TileModel> GameTableTileModels { get; private set; }

        /// <summary>
        /// List of user's tiles
        /// </summary>
        public List<TileModel> UserBaseTileModels { get; private set; }
        
        public delegate void TilePickHandler(Node node);

        /// <summary>
        /// Tile picked from user base
        /// </summary>
        public event TilePickHandler TilePicked;

        public TileModelControler(Grid grid)
        {
            GameTableTileModels = new List<TileModel>();
            UserBaseTileModels = new List<TileModel>();

            _parentGrid = grid;
            tileWidth = (new TileModel()).Tile.Width;
            tileHeight = (new TileModel()).Tile.Height;
        }

        /// <summary>
        /// Set tilemodel centr coords and its rotating angle
        /// </summary>
        /// <param name="tileModel">Tilemodel which is applied parametrs to</param>
        /// <param name="node">Node parameter</param>
        /// <param name="point">Point of parent side</param>
        /// <param name="parentTileSide">Parent sile side</param>
        /// <param name="childTileSide">Child tile side</param>
        /// <returns></returns>
        private TileModel SetTileParameters(TileModel tileModel, Node node, Point point, Side parentTileSide, Side childTileSide)
        {
            tileModel.CurrentNode = node;
            int angle = 0;
            if (parentTileSide != Side.Center)
            {
                angle = 2 - ((int)parentTileSide) + ((int)childTileSide);
            }
            tileModel.Angle = angle%4;

            var offset = tileModel.OffsetVector(childTileSide);
            point.X = point.X - offset.X;
            point.Y = point.Y - offset.Y;
            tileModel.Center = point;
            
            return tileModel;
        }

        /// <summary>
        /// Add tile to game table
        /// </summary>
        /// <param name="node">New node</param>
        /// <param name="point">Point of parent side</param>
        /// <param name="parentTileSide">Parent sile side</param>
        /// <param name="childTileSide">Child tile side</param>
        /// <returns>Added tile model</returns>
        public TileModel AddTileToGame(Node node, Point point, Side parentTileSide, Side childTileSide)
        {
            TileModel tileModel = new TileModel();
            SetTileParameters(tileModel,node, point, parentTileSide, childTileSide);

            _parentGrid.Children.Add(tileModel);
            GameTableTileModels.Add(tileModel);
            return tileModel;
        }

        /// <summary>
        /// Add tile to user interface on exact coords
        /// </summary>
        /// <param name="tile">New tile</param>
        /// <param name="point">Point on the table</param>
        public void AddTileToUserBase(Tile tile, Point point)
        {
            TileModel tileModel = new TileModel();
            var node = new Node { CurrentTile = tile };
            tileModel.CurrentNode = node;

            tileModel.Center = point;
            tileModel.MouseDown += TileModel_MouseDown;
            UserBaseTileModels.Add(tileModel);
            _parentGrid.Children.Add(tileModel);
        }

        /// <summary>
        /// Executes when clicked on tile from user base
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
