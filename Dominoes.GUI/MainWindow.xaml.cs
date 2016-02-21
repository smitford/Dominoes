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
            _centr = new Point(450,305);
            _tileModelControler = new TileModelControler(Background);
            //_gameLogic.InitGame();
            _gameLogic.PlayerMovesEnent += _gameLogic_PlayerMovesEnent;
            _gameLogic.AImovesEnent += _gameLogic_AImovesEnent;

            //_gameLogic.PlayerMoves(_gameLogic.PlayerTiles[0],_gameLogic.GameMoves.First, Side.Rigt);
            var moves = _gameLogic.GameMoves;
            var m0 = moves.FirstMove(new Tile(3, 2));
            var m1 = moves.NewMove(new Tile(3, 2), moves.First, Side.Rigt);
            //var m2 = moves.NewMove(new Tile(3, 2), m1, Side.Left);

            _tileModelControler.AddNewTile(m0, _centr, Side.Center, Side.Top);

            var point = (Point)(_tileModelControler.Nodes.Find(x => x.Key == m0).Value - _tileModelControler.OffsetCoords(Side.Top));
            //_tileModelControler.AddNewTile(m1, point, m0.NeighbourNodes.Find(x => x.Value == m0).Key, Side.Top);

            Background.Children.Add(new Ellipse { Width = 8, Height = 8, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(point.X-4, point.Y-4, 0, 0), Fill = Brushes.OrangeRed });
            var a = 5;
            //DrawGame();
        }

        private void DrawGame()
        {
            var moves = _gameLogic.GameMoves;
            var leaves = moves.Leaves;
            _tileModelControler.AddNewTile(moves.First, _centr, Side.Top ,Side.Top);
            DrawingRecursion(moves.First.NeighbourNodes, moves.First);
        }

        private void DrawingRecursion(List<KeyValuePair<Side, Node>> NeighbourNodes, Node parentNode)
        {
            var leaves = new List<Node>();

            foreach (var node in NeighbourNodes)
            {
                var childNeighbourNodes = node.Value.NeighbourNodes;
                childNeighbourNodes.Remove(childNeighbourNodes.Find(x => x.Value == parentNode));
                var point = (Point)(_tileModelControler.Nodes.Find(x => x.Key == parentNode).Value );// - _tileModelControler.OffsetCoords(node.Key));
                _tileModelControler.AddNewTile(node.Value, point, node.Key,Side.Top);

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
