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
using System.Threading;

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
        TileModel _newTile;
        Leaf _nearConnector;
        bool _gameStarted = false;
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
            pickButton.IsEnabled = false;
            saveButton.IsEnabled = false;
        }

        private void _scoring_GameOver(int playerScores, int aiScores)
        {
            _gameStarted = false;
            //Thread.Sleep(500);
            var message = "";
            var playerWin = playerScores > aiScores;
            if (playerWin)
            {
                message += "You win :)";
            }
            else
            {
                message += "You lose :(";
            }
            message += "\n";
            message += String.Format("{0} : {1}",playerScores, aiScores);

            Thread thread = new Thread(new ParameterizedThreadStart(GameOverMessage));
            thread.Start(message);
            //InitGame();
        }

        void GameOverMessage(object message)
        {
            MessageBox.Show((string)message);
        }

        private void InitGame()
        {
            _gameLogic.ResetGame();

            _gameStarted = true;
            this.MouseMove += Window_MouseMove;
            Background.MouseDown += Background_MouseDown;
            _centr = new Point(Width / 2, Height / 2 - Background.Margin.Top-100);
            _tileModelControler = new TileModelControler(Background);
            _tileModelControler.TilePicked += _tileModelControler_TilePicked;
            _gameLogic.AImovesEnent += _gameLogic_AImovesEnent;
            _gameLogic.PlayerMovesEnent += _gameLogic_PlayerMovesEnent; ;
            _gameLogic.InitGame();

            Background.Children.Clear();
            _tileModelControler.GameTableTileModels.Clear();
            _tileModelControler.UserBaseTileModels.Clear();

            DrawGame();
            HelperInit();
            ShowUserTilles(_gameLogic.PlayerTiles);
            if (_gameLogic.TileBase.Count > 0)
            {
                pickButton.IsEnabled = true;
            }
            if (!_gameLogic.IsPlayerTurn)
            {
                _gameLogic.AIMoves();
            }
            _gameLogic.Scoring.GameOver += _scoring_GameOver;
            AiTilesCountLabel.Content = _gameLogic.AiTiles.Count;
            StockTileCountLable.Content = _gameLogic.TileBase.Count;
        }

        private void _gameLogic_PlayerMovesEnent(Node parentNode, Node childNode, Side parentSide, Side childSide)
        {
            var b = _tileModelControler.GameTableTileModels.Find(x => x.CurrentNode == parentNode).Angle;
            parentSide = (Side)((4 - b + (int)parentSide) % 4);
            AddNewMove(childNode, parentNode, parentSide, childSide);
        }

        private void _gameLogic_AImovesEnent(Node parentNode, Node childNode, Side parentSide, Side childSide)
        {
            Thread.Sleep(100);  //delay before move
            var b = _tileModelControler.GameTableTileModels.Find(x => x.CurrentNode == parentNode).Angle;
            parentSide = (Side)((4 - b + (int)parentSide) % 4);
            AddNewMove(childNode, parentNode, parentSide , childSide);
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            startButton.Content = "Restart game";
            InitGame();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void pickButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _tileModelControler.UserBaseTileModels)
            {
                Background.Children.Remove(item);
            }
            _gameLogic.PlayerTile.Add(Tile.PickTileFromBase(_gameLogic.TileBase));
            ShowUserTilles(_gameLogic.PlayerTiles);
            if (_gameLogic.TileBase.Count > 0)
            {
                pickButton.IsEnabled = true;
            }
            else
            {
                pickButton.IsEnabled = false;
            }
            AiTilesCountLabel.Content = _gameLogic.AiTiles.Count;
            StockTileCountLable.Content = _gameLogic.TileBase.Count;
        }

        private void AddNewMove(Node currNode, Node parentNode, Side parentTileSide, Side childTileSide)
        {
            try {
                var point = _tileModelControler.GameTableTileModels.Find(x => x.CurrentNode == parentNode).Connector(parentTileSide);

                var t = _tileModelControler.AddTileToGame(currNode, point, parentTileSide, childTileSide);
                if (_gameLogic.IsPlayerTurn)
                {
                    t.Rect.Fill = Brushes.DarkGreen;
                }

                var selectedTile = _tileModelControler.UserBaseTileModels.Find(x => x.CurrentNode == _newTile.CurrentNode);
                Background.Children.Remove(selectedTile);
                _tileModelControler.UserBaseTileModels.Remove(selectedTile);
                _newTile.CurrentNode = null;
                AiTilesCountLabel.Content = _gameLogic.AiTiles.Count;
                StockTileCountLable.Content = _gameLogic.TileBase.Count;
            }
            catch { }
        }

        private void _tileModelControler_TilePicked(Node node)
        {
            _newTile.CurrentNode = node;
        }

        private void ShowUserTilles(List<Tile> playerTiles)
        {
            for (int i = 0; i < playerTiles.Count; i++)
            {
                var p = new Point((Width/2- (playerTiles.Count-1)*50/2) + i * 50, Height-180);
                _tileModelControler.AddTileToUserBase(playerTiles[i], p);

            }
        }

        private void DrawGame()
        {
            var moves = _gameLogic.GameMoves;
            var m0 = moves.First;

            //var m0  =  moves.FirstMove(new Tile(0, 0));
            //var m1  =  moves.NewMove(new Tile(1, 0), m0, Side.Top);
            //var m1_1 = moves.NewMove(new Tile(1, 1), m1, Side.Top);
            //var m2 = moves.NewMove(new Tile(2, 0), m0, Side.Right);
            //var m2_2 = moves.NewMove(new Tile(2, 2), m2, Side.Top);
            //var m2_3 = moves.NewMove(new Tile(2, 2), m2_2, Side.Bottom);
            //var m2_4 = moves.NewMove(new Tile(2, 3), m2_3, Side.Top);
            //var m2_5 = moves.NewMove(new Tile(4, 2), m2_3, Side.Right);

            //var m3 = moves.NewMove(new Tile(3, 0), m0, Side.Left);
            //var m3_3 = moves.NewMove(new Tile(3, 3), m3, Side.Top);
            //var m4 = moves.NewMove(new Tile(4, 0), m0, Side.Bottom);
            //var m4_4 = moves.NewMove(new Tile(4, 4), m4, Side.Bottom);

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
            if (_gameLogic.IsPlayerTurn)
            {
                var model = e.Source;

                if (!_tileModelControler.UserBaseTileModels.Contains(model) && _nearConnector != null && _newTile.CurrentNode != null)
                {
                    var pn = _nearConnector.ConnectorTileModel.CurrentNode;

                    var moves = _gameLogic.GameMoves;
                    var childTileSide = _gameLogic.GameMoves.GetMatchSide(_nearConnector.ConnectorTileModel.CurrentNode, _newTile.CurrentNode, _nearConnector.ConnectorSide);

                    var b = _nearConnector.ConnectorTileModel.Angle;
                    var parentSide = (4 - b + (int)_nearConnector.ConnectorSide) % 4;
                    var t = _newTile.CurrentNode.CurrentTile;
                    var currNode = _gameLogic.PlayerMoves(t, pn, (Side)_nearConnector.ConnectorSide);

                    AddNewMove(currNode, pn, (Side)parentSide, childTileSide);
                    if (_gameLogic.TileBase.Count > 0)
                    {
                        pickButton.IsEnabled = true;
                    }
                    if (_gameStarted&& !_gameLogic.IsPlayerTurn)
                    {
                        _gameLogic.AIMoves();
                    }
                    var uiPoints = Background.Children.OfType<Ellipse>().ToList();
                    foreach (var point in uiPoints)
                    {
                        Background.Children.Remove(point);
                    }
                }
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var mouse = new Point(Mouse.GetPosition(Application.Current.MainWindow).X - Background.Margin.Left, Mouse.GetPosition(Application.Current.MainWindow).Y - Background.Margin.Top);

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
                if (tileModel!=null)
                {
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
            }
            return leavesConnectors;
        }

        private void HelperInit()
        {
            _newTile = new TileModel { VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left };
            //_newTile.CurrentNode = new Node { CurrentTile = new Tile(2, 1) };
            _newTile.Visibility = Visibility.Hidden;
            _newTile.Rect.Fill = Brushes.Red;
            Background.Children.Add(_newTile);
        }

        private void HelpToConnect(Point mouse, Tile tile)
        {
            if (_gameLogic.IsPlayerTurn)
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
                        int angle = (2 + _nearConnector.ConnectorTileModel.Angle - ((int)parentTileSide) + ((int)childTileSide)) % 4;
                        var offset = _newTile.ConnectorOffset(childTileSide);
                        point.X = point.X + offset.X;
                        point.Y = point.Y + offset.Y;
                        _newTile.Angle = angle;
                        _newTile.Center = point;
                        _newTile.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _nearConnector = null;
                        _newTile.Visibility = Visibility.Hidden;
                    }
                }
                else
                {
                    _nearConnector = null;
                    _newTile.Visibility = Visibility.Hidden;
                }
            }
        }
    }
}
