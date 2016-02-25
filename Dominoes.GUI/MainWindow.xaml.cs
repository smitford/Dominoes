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
        public MainWindow()
        {
            InitializeComponent();
            _centr = new Point(400,200);
            _tileModelControler = new TileModelControler(Background);
            //_gameLogic.InitGame();
            _gameLogic.PlayerMovesEnent += _gameLogic_PlayerMovesEnent;
            _gameLogic.AImovesEnent += _gameLogic_AImovesEnent;

            //_gameLogic.PlayerMoves(_gameLogic.PlayerTiles[0],_gameLogic.GameMoves.First, Side.Rigt);
            //var moves = _gameLogic.GameMoves;
            //var m0 = moves.FirstMove(new Tile(3,3));
            //var m1 = moves.NewMove(new Tile(3, 2), m0, Side.Rigt);
            //var m2 = moves.NewMove(new Tile(2, 4), m1, Side.Bottom);

            //_tileModelControler.AddNewTile(m0, _centr, Side.Center, Side.Top);
            ////var point = (Point)(_tileModelControler.TileModels.Find(x => x.CurrentNode == m0).Connector(Side.Left));// - _tileModelControler.OffsetCoords(Side.Top));
            ////_centr = (Point)(_tileModelControler.TileModels.Find(x => x.CurrentNode == m0).Center);// - _tileModelControler.OffsetCoords(Side.Top));

            //var t0 = _tileModelControler.TileModels.Find(x=>x.CurrentNode == m0);
            //var tnode1 = m1;
            //var s1 = m0.NeighbourNodes.Find(x=>x.Value== tnode1).Key;
            //var point = (Point)t0.Connector(s1);// - _tileModelControler.OffsetCoords(Side.Top));
            //_tileModelControler.AddNewTile(tnode1, point, s1, Side.Top);

            //var t1 = _tileModelControler.TileModels.Find(x => x.CurrentNode == m1);
            //var t2 = (_tileModelControler.TileModels.Find(x => x.CurrentNode == m2));
            //var s2 = m1.NeighbourNodes.Find(x => x.Value == m2).Key - (int)s1;
            //var point2 = (Point)t1.Connector(s2);// - _tileModelControler.OffsetCoords(Side.Top));
            //_tileModelControler.AddNewTile(m2, point2, s2, Side.Bottom);

            ////point = new Point( t0.Margin.Left, t0.Margin.Top);
            ////Background.Children.Add(new Ellipse { Margin = new Thickness(_centr.X - 4, _centr.Y - 4, 0, 0), Fill = Brushes.Aqua, Width = 8, Height = 8, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left});
           // Background.Children.Add(new Ellipse { Margin = new Thickness(point.X - 4, point.Y - 4, 0, 0), Fill = Brushes.OrangeRed, Width = 8, Height = 8, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left});

            var a = 5;
            DrawGame();
        }

        private void DrawGame()
        {
            var moves = _gameLogic.GameMoves;
            var m0 = moves.FirstMove(new Tile(3, 3));
            var m1 = moves.NewMove(new Tile(3, 2), m0, Side.Rigt);
            var m2 = moves.NewMove(new Tile(2, 4), m1, Side.Bottom);
            var m3 = moves.NewMove(new Tile(4, 4), m2, Side.Bottom);
            var m4 = moves.NewMove(new Tile(4, 6), m3, Side.Rigt);
            var m5 = moves.NewMove(new Tile(6, 1), m4, Side.Bottom);
            var m6 = moves.NewMove(new Tile(4, 2), m3, Side.Top);

            _tileModelControler.AddNewTile(moves.First, _centr, Side.Center ,Side.Top);
            DrawingRecursion(moves.First.NeighbourNodes, moves.First);
        }

        private void DrawingRecursion(List<KeyValuePair<Side, Node>> NeighbourNodes, Node parentNode)
        {
            //var leaves = new List<Node>();

            foreach (var node in NeighbourNodes)
            {
                var childNeighbourNodes = node.Value.NeighbourNodes;
                childNeighbourNodes.Remove(childNeighbourNodes.Find(x => x.Value == parentNode));

                var parentTileModel = _tileModelControler.TileModels.Find(x => x.CurrentNode == parentNode);

                var parentSide = (Side)((-parentTileModel.Angle/90)%4);

                //var parentSide = (Side)(((((180 - parentTileModel.Angle /*- 90*Math.Cos(parentTileModel.Angle)*/) / 360 * 4)) % 4) % 4);
                var connectingSide = (Side)(((int)parentNode.NeighbourNodes.Find(x => x.Value == node.Value).Key + (int)parentSide)%4);
                var childSide = node.Value.NeighbourNodes.Find(x => x.Value == parentNode).Key;
                var point = (Point)parentTileModel.Connector(connectingSide);
                _tileModelControler.AddNewTile(node.Value, point, connectingSide, childSide);
                Background.Children.Add(new Ellipse { Margin = new Thickness(point.X - 4, point.Y - 4, 0, 0), Fill = Brushes.OrangeRed, Width = 8, Height = 8, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left});

                if (childNeighbourNodes.Count >= 0)
                {
                    DrawingRecursion(childNeighbourNodes, node.Value);
                }
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

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var x = Mouse.GetPosition(Application.Current.MainWindow).X;
            var y = Mouse.GetPosition(Application.Current.MainWindow).Y;
        }
    }
}
