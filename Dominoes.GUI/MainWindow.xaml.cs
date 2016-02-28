using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dominoes.AI;

namespace Dominoes.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TileModelControler _tileModelControler;
        GameLogic _gameLogic = new GameLogic();
        Point _centr;
        AimModel _helperPoint;
        TileModel _newTile;
        Leaf _nearConnector;

        public class Leaf
        {
            public Leaf(TileModel connectorTileModel, Side connectorSide, Point connectorPoint)
            {
                ConnectorSide = connectorSide;
                ConnectorPoint = connectorPoint;
                ConnectorTileModel = connectorTileModel;
            }
            public Side ConnectorSide { get; set; }
            public Point ConnectorPoint { get; set; }
            public TileModel ConnectorTileModel { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();

            _centr = new Point(400, 200);
            _tileModelControler = new TileModelControler(Background);
            _tileModelControler.TilePicked += _tileModelControler_TilePicked;
            _gameLogic.InitGame();
            _gameLogic.PlayerMovesEnent += _gameLogic_PlayerMovesEnent;
            _gameLogic.AImovesEnent += _gameLogic_AImovesEnent;

            DrawGame();
            HelperInit();
            ShowUserTilles(_gameLogic.PlayerTiles);
        }

        private void _tileModelControler_TilePicked(Node node)
        {
            _newTile.CurrentNode = node;
        }

        private void ShowUserTilles(List<Tile> playerTiles)
        {
            for (int i = 0; i < playerTiles.Count; i++)
            {
                var p = new Point((Width/2- (playerTiles.Count-1)*50/2) + i * 50, Height-100);
                _tileModelControler.AddTileToUserBase(playerTiles[i], p);

            }
        }

        private void DrawGame()
        {
            var moves = _gameLogic.GameMoves;
            //var m0 = moves.First;

            var m0  =  moves.FirstMove(new Tile(0, 0));
            var m1  =  moves.NewMove(new Tile(1, 0), m0, Side.Top);
            var m1_1 = moves.NewMove(new Tile(1, 1), m1, Side.Top);
            var m2 = moves.NewMove(new Tile(2, 0), m0, Side.Right);
            var m2_2 = moves.NewMove(new Tile(2, 2), m2, Side.Top);
            var m2_3 = moves.NewMove(new Tile(2, 2), m2_2, Side.Bottom);
            var m2_4 = moves.NewMove(new Tile(2, 3), m2_3, Side.Top);
            var m2_5 = moves.NewMove(new Tile(4, 2), m2_3, Side.Right);

            var m3 = moves.NewMove(new Tile(3, 0), m0, Side.Left);
            var m3_3 = moves.NewMove(new Tile(3, 3), m3, Side.Top);
            var m4 = moves.NewMove(new Tile(4, 0), m0, Side.Bottom);
            var m4_4 = moves.NewMove(new Tile(4, 4), m4, Side.Bottom);

            _tileModelControler.AddTileToGame(moves.First, _centr, Side.Center, Side.Top);
            DrawingRecursion(moves.First.AvailableNeighbourNodes, moves.First);

        }

        private void DrawingRecursion(List<KeyValuePair<Side, Node>> neighbourNodes, Node parentNode)
        {

            foreach (var node in neighbourNodes)  
            {
                
                var parentTileModel = _tileModelControler.GameTableTileModels.Find(x => x.CurrentNode == parentNode);

                var parentSide = 4-parentTileModel.Angle + (int)parentNode.AvailableNeighbourNodes.Find(x => x.Value == node.Value).Key;
                var childSide = node.Value.AvailableNeighbourNodes.Find(x => x.Value == parentNode).Key;
                var point = (Point)parentTileModel.Connector((Side)(parentSide%4));

                _tileModelControler.AddTileToGame(node.Value, point, (Side)(parentSide % 4), childSide);
                //Background.Children.Add(new Ellipse { Margin = new Thickness(point.X - 4, point.Y - 4, 0, 0), Fill = Brushes.OrangeRed, Width = 8, Height = 8, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left});

                var childNeighbourNodes = node.Value.AvailableNeighbourNodes.Where(x => x.Value != parentNode).ToList();
                if (childNeighbourNodes.Count >0)
                {
                    DrawingRecursion(childNeighbourNodes, node.Value);
                }
            }
        }

        

        private void Background_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var model = e.Source;

            if (!_tileModelControler.UserBaseTileModels.Contains(model) && _nearConnector != null )
            {
                var pn = _nearConnector.ConnectorTileModel.CurrentNode;

                var moves = _gameLogic.GameMoves;
                var childTileSide = _gameLogic.GameMoves.GetMatchSide(_nearConnector.ConnectorTileModel.CurrentNode, _newTile.CurrentNode, _nearConnector.ConnectorSide);

                var b = _nearConnector.ConnectorTileModel.Angle;
                var parentSide = (4-b + (int)_nearConnector.ConnectorSide) % 4;
                var t = _newTile.CurrentNode.CurrentTile;
                var point = _nearConnector.ConnectorPoint;

                var currNode = moves.NewMove(t, pn,(Side)_nearConnector.ConnectorSide);
                _tileModelControler.AddTileToGame(currNode, point, (Side)parentSide, childTileSide);
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var mouse = new Point(Mouse.GetPosition(Application.Current.MainWindow).X, Mouse.GetPosition(Application.Current.MainWindow).Y);

            if (_newTile.CurrentNode != null)
            {
                HelpToConnect(mouse, _newTile.CurrentNode.CurrentTile);
            }
        }

        private List<Leaf> GetLeavesConnectors(List<Node> leaves)
        {
            var leavesConnectors = new List<Leaf>();

            foreach (var node in leaves)
            {
                var sides = node.NeighbourNodes.Where(x => x.Value == null).ToList();
                var tileModel = _tileModelControler.GameTableTileModels.Find(x => x.CurrentNode == node);
                if (!node.CurrentTile.IsDouble())
                {
                    sides.RemoveAll(x => x.Key == Side.Left || x.Key == Side.Right);
                }
                foreach (var side in sides)
                {
                    var point = tileModel.SideCoords(side.Key);
                    leavesConnectors.Add(new Leaf(tileModel, side.Key, point));
                }
            }
            return leavesConnectors;
        }

        private void HelperInit()
        {
            _newTile = new TileModel { VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left };
            //_newTile.CurrentNode = new Node { CurrentTile = new Tile(2, 1) };
            _newTile.Visibility = Visibility.Hidden;
            _helperPoint = new AimModel { VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left };
            _helperPoint.Visibility = Visibility.Hidden;
            _newTile.Rect.Fill = Brushes.Red;
            Background.Children.Add(_helperPoint);
            Background.Children.Add(_newTile);
        }

        private void HelpToConnect(Point mouse, Tile tile)
        {
            var connectors = GetLeavesConnectors(_gameLogic.GameMoves.Leaves).Where(x =>
            x.ConnectorTileModel.CurrentNode.CurrentTile.GetSideValue(x.ConnectorSide) == tile.TopEnd ||
            x.ConnectorTileModel.CurrentNode.CurrentTile.GetSideValue(x.ConnectorSide) == tile.BottomEnd).ToList();

            foreach (var item in connectors)
            {
                var point = item.ConnectorPoint;
                Background.Children.Add(new Ellipse { Margin = new Thickness(point.X - 4, point.Y - 4, 0, 0), Fill = Brushes.OrangeRed, Width = 8, Height = 8, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left });
            }

            if (connectors.Count > 0)
            {
                var points = connectors.Select(x => x.ConnectorPoint).ToList();
                var vectors = points.Select(x => (x - mouse)).ToList();
                var minLength = points.Select(x => (x - mouse).Length).Min();
                var minVectors = vectors.Find(x => x.Length == minLength);

                if (minLength <= 70)
                {
                    _nearConnector = connectors.Find(x => x.ConnectorPoint == (mouse + minVectors));
                    var point = _nearConnector.ConnectorPoint;
                    var childNodde = new Node { CurrentTile = tile };
                    _nearConnector.ConnectorSide = (Side)(((int)_nearConnector.ConnectorSide) % 4);
                    var parentTileSide = _nearConnector.ConnectorSide;
                    var childTileSide = _gameLogic.GameMoves.GetMatchSide(_nearConnector.ConnectorTileModel.CurrentNode, childNodde, parentTileSide);
                    int angle = (2+_nearConnector.ConnectorTileModel.Angle - ((int)parentTileSide) + ((int)childTileSide) ) % 4;

                    Background.Children.Add(new AimModel { Margin = new Thickness(point.X - 5, point.Y - 5, 0, 0),  VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left });

                    _newTile.Angle = angle;
                    var cos = Math.Cos(Math.PI * (angle * 2));
                    var sin = Math.Sin(Math.PI * (angle * 2));
                    //Point b = new Point(0, 0);
                    //var angle = 2 - ((int)parentTileSide) + ((int)childTileSide)
                    var offset = _newTile.ConnectorOffset(childTileSide);
                    point.X = point.X + offset.X;
                    point.Y = point.Y + offset.Y;
                    
                    _helperPoint.Margin = new Thickness(point.X - 5, point.Y - 5, 0, 0);
                    _newTile.Center = point;

                    _helperPoint.Visibility = Visibility.Visible;
                    _newTile.Visibility = Visibility.Visible;
                }
                else
                {
                    _nearConnector = null;
                    _newTile.Visibility = Visibility.Hidden;
                    _helperPoint.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                _nearConnector = null;
                _newTile.Visibility = Visibility.Hidden;
                _helperPoint.Visibility = Visibility.Hidden;
            }
        }

        private void _gameLogic_AImovesEnent(Node node)
        {
            //throw new NotImplementedException();
        }

        private void _gameLogic_PlayerMovesEnent(Node node)
        {
            //throw new NotImplementedException();
        }

    }
}
