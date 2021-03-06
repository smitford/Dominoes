﻿using System;
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
using Dominoes.DB;
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
        String nick;
        DbConnector _dbconnector;
    
        bool _gameStarted = false;
        public class Leaf
        {
            public Leaf(TileModel connectorTileModel, Side connectorSide, Point connectorPoint)
            {
                ConnectorSide = connectorSide;
                ConnectorPoint = connectorPoint;
                ParentTileModel = connectorTileModel;
            }

            public Side ConnectorSide { get; set; }
            public Point ConnectorPoint { get; set; }
            public TileModel ParentTileModel { get; set; }
        }
        
        public MainWindow()
        {
            InitializeComponent();
            pickButton.IsEnabled = false;
            _dbconnector = new DbConnector();
            _centr = new Point(Width / 2, Height / 2 - Background.Margin.Top - 100);
        }

        /// <summary>
        /// Executes at game over.
        /// Count scores.
        /// </summary>
        /// <param name="playerScores">Player scores</param>
        /// <param name="aiScores">AI scores</param>
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
            pickButton.IsEnabled = false;

            Thread thread = new Thread(new ParameterizedThreadStart(GameOverMessage));
            thread.Start(message);

            _dbconnector.Add(nick, playerWin);
            var gameHistoryWindow = new GameHistoryWindow();
            gameHistoryWindow.dataGrid.ItemsSource = _dbconnector.GetHistory();
            gameHistoryWindow.Show();
        }

        /// <summary>
        /// Executes at game over
        /// </summary>
        /// <param name="message"></param>
        void GameOverMessage(object message)
        {
            MessageBox.Show((string)message);
        }

        /// <summary>
        /// Initialize the game
        /// </summary>
        private void InitGame()
        {
            var loginDialog = new LoginDialog();
            if (loginDialog.ShowDialog() == true)
            {
                nick = loginDialog.ResponseText;
                helloLabel.Content = "Hello, " + nick + "!";
                _gameLogic.ResetGame();
                _gameStarted = true;
                this.MouseMove += Window_MouseMove;
                Background.MouseDown += Background_MouseDown;
                
                _tileModelControler = new TileModelControler(Background);
                _tileModelControler.TilePicked += _tileModelControler_TilePicked;
                _gameLogic.AImovesEnent += _gameLogic_AImovesEnent;
                _gameLogic.PlayerMovesEnent += _gameLogic_PlayerMovesEnent; 
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
        }

        /// <summary>
        /// Executes when game logic goes instead of user
        /// </summary>
        /// <param name="parentNode">Parent node</param>
        /// <param name="childNode">Child node</param>
        /// <param name="parentSide">Parent tile side</param>
        /// <param name="childSide">Child tile side</param>
        private void _gameLogic_PlayerMovesEnent(Node parentNode, Node childNode, Side parentSide, Side childSide)
        {
            var b = _tileModelControler.GameTableTileModels.Find(x => x.CurrentNode == parentNode).Angle;
            parentSide = (Side)((4 - b + (int)parentSide) % 4);
            AddNewMove(childNode, parentNode, parentSide, childSide);
        }

        /// <summary>
        /// Executes when game logic goes instead of PC
        /// </summary>
        /// <param name="parentNode">Parent node</param>
        /// <param name="childNode">Child node</param>
        /// <param name="parentSide">Parent tile side</param>
        /// <param name="childSide">Child tile side</param>
        private void _gameLogic_AImovesEnent(Node parentNode, Node childNode, Side parentSide, Side childSide)
        {
            Thread.Sleep(100);  //delay before move
            var b = _tileModelControler.GameTableTileModels.Find(x => x.CurrentNode == parentNode).Angle;
            parentSide = (Side)((4 - b + (int)parentSide) % 4);
            AddNewMove(childNode, parentNode, parentSide , childSide);
        }

        /// <summary>
        /// Executes when start button clicked.
        /// Game starts or restarts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            startButton.Content = "Restart game";
            InitGame();
        }

        /// <summary>
        /// Executes when start pick clicked.
        /// Picks to new tile to user base.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pickButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _tileModelControler.UserBaseTileModels)
            {
                Background.Children.Remove(item);
            }
            _gameLogic.PlayerTiles.Add(Tile.PickTileFromBase(_gameLogic.TileBase));
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

        /// <summary>
        /// Processes new move. Adds it to game logic and UI
        /// </summary>
        /// <param name="currNode">New node</param>
        /// <param name="parentNode">Parent node</param>
        /// <param name="parentTileSide">Parent tile side</param>
        /// <param name="childTileSide">Child tile side</param>
        private void AddNewMove(Node currNode, Node parentNode, Side parentTileSide, Side childTileSide)
        {
            try {
                var point = _tileModelControler.GameTableTileModels.Find(x => x.CurrentNode == parentNode).Connector(parentTileSide);

                var t = _tileModelControler.AddTileToGame(currNode, point, parentTileSide, childTileSide);
                if (_gameLogic.IsPlayerTurn) // if not player's turn
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

        /// <summary>
        /// Executes at click on tile from base
        /// </summary>
        /// <param name="node"></param>
        private void _tileModelControler_TilePicked(Node node)
        {
            _newTile.CurrentNode = node;
        }

        /// <summary>
        /// Shows user tile in UI
        /// </summary>
        /// <param name="playerTiles"></param>
        private void ShowUserTilles(List<Tile> playerTiles)
        {
            for (int i = 0; i < playerTiles.Count; i++)
            {
                var p = new Point((Width/2 - (playerTiles.Count-1)*50/2) + i * 50, Height-180);
                _tileModelControler.AddTileToUserBase(playerTiles[i], p);
            }
        }

        /// <summary>
        /// Draws dominoes at start
        /// </summary>
        private void DrawGame()
        {
            var moves = _gameLogic.GameMoves;
            var m0 = moves.First;

            _tileModelControler.AddTileToGame(moves.First, _centr, Side.Center, Side.Top);
        }

        /// <summary>
        /// Executes at mouse click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Background_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_gameLogic.IsPlayerTurn)
            {
                var model = e.Source;

                if (!_tileModelControler.UserBaseTileModels.Contains(model) && _nearConnector != null && _newTile.CurrentNode != null)
                {
                    var pn = _nearConnector.ParentTileModel.CurrentNode;

                    var moves = _gameLogic.GameMoves;
                    var childTileSide = _gameLogic.GameMoves.GetMatchSide(_nearConnector.ParentTileModel.CurrentNode, _newTile.CurrentNode, _nearConnector.ConnectorSide);

                    var b = _nearConnector.ParentTileModel.Angle;
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

        /// <summary>
        /// Executes at mouse move
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var mouse = new Point(Mouse.GetPosition(Application.Current.MainWindow).X - Background.Margin.Left, Mouse.GetPosition(Application.Current.MainWindow).Y - Background.Margin.Top);

            if (_newTile.CurrentNode != null)
            {
                HelpToConnect(mouse, _newTile.CurrentNode.CurrentTile);
            }
        }

        /// <summary>
        /// Returns leaves with free sides.
        /// </summary>
        /// <param name="leaves">Nodes with free sides</param>
        /// <returns>List with information about available connecting points</returns>
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
                        leavesConnectors.Add(new Leaf(tileModel, (Side)((int)side.Key % 4), point));
                    }
                }
            }
            return leavesConnectors;
        }

        /// <summary>
        /// Initialization of helper tile binded to mouse
        /// </summary>
        private void HelperInit()
        {
            _newTile = new TileModel { VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left };
            _newTile.Visibility = Visibility.Hidden;
            _newTile.Rect.Fill = Brushes.Red;
            Background.Children.Add(_newTile);
        }

        /// <summary>
        /// Finds available points to connect new tile
        /// </summary>
        /// <param name="mouse">Current mouse coordinates</param>
        /// <param name="tile">New tile</param>
        private void HelpToConnect(Point mouse, Tile tile)
        {
            if (_gameLogic.IsPlayerTurn)
            {
                //get match free connectors
                var connectors = GetLeavesConnectors(_gameLogic.GameMoves.Leaves).Where(x =>
                x.ParentTileModel.CurrentNode.CurrentTile.GetSideValue(x.ConnectorSide) == tile.TopEnd ||
                x.ParentTileModel.CurrentNode.CurrentTile.GetSideValue(x.ConnectorSide) == tile.BottomEnd).ToList();

                foreach (var item in connectors)
                {
                    var point = item.ConnectorPoint;
                    Background.Children.Add(new Ellipse { Margin = new Thickness(point.X - 4, point.Y - 4, 0, 0), Fill = Brushes.OrangeRed, Width = 8, Height = 8, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left });
                }

                if (connectors.Count > 0)
                {
                    var points = connectors.Select(x => x.ConnectorPoint).ToList();
                    var vectors = points.Select(x => (x - mouse)).ToList(); //Vector from mouse to connector

                    var minLength = points.Select(x => (x - mouse).Length).Min();
                    var minVectors = vectors.Find(x => x.Length == minLength);

                    if (minLength <= 70)
                    {
                        _nearConnector = connectors.Find(x => x.ConnectorPoint == (mouse + minVectors));
                        var point = _nearConnector.ConnectorPoint;
                        var childNode = new Node { CurrentTile = tile };
                        var parentTileSide = _nearConnector.ConnectorSide;
                        var childTileSide = _gameLogic.GameMoves.GetMatchSide(_nearConnector.ParentTileModel.CurrentNode, childNode, parentTileSide);
                        int angle = (2 + _nearConnector.ParentTileModel.Angle - ((int)parentTileSide) + ((int)childTileSide)) % 4;
                        var offset = _newTile.OffsetVector(childTileSide);
                        point.X = point.X - offset.X;
                        point.Y = point.Y - offset.Y;
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
