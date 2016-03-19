using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes.AI
{
    public class GameLogic
    {
        public delegate void NewMoveHandler(Node parentNode, Node childNode, Side parentSide, Side childSide);


        public event NewMoveHandler AImovesEnent;
        public event NewMoveHandler PlayerMovesEnent;

        /// <summary>
        /// List of tiles in the stock
        /// </summary>
        public List<Tile> TileBase { get; private set; }

        /// <summary>
        /// List of player's tiles
        /// </summary>
        public List<Tile> PlayerTiles { get; private set; }

        /// <summary>
        /// List of player's tiles
        /// </summary>
        public List<Tile> AiTiles { get; private set; }

        public bool IsPlayerTurn { get; private set; }
        public Moves GameMoves { get; private set; }
        private int movesCount = 0;
        public Scoring Scoring { get; private set; }

        public Node PlayerMoves(Tile tile, Node node, Side tileSide)
        {
            if (IsPlayerTurn)
            {
                var n = Go(tile, node, tileSide, PlayerTiles);
                Scoring.CheckGameState(GameMoves, TileBase, PlayerTiles, AiTiles);
                return n;
            }
            else { throw new Exception("Is not player's turn"); }
        }

        public void AIMoves()
        {
            if (!IsPlayerTurn)
            {
                var leaves = GameMoves.Leaves;
                var freeSides = new List<KeyValuePair<Side, Node>>();
                foreach (var leaf in leaves)
                {
                    freeSides.AddRange(leaf.AvailableNeighbourNodes.Where(x =>
                    (x.Key == Side.Top || x.Key == Side.Bottom) ||
                    (x.Key == Side.Right && leaf.CurrentTile.IsDouble())
                    ));
                }
                foreach (var aiTile in AiTiles)
                {

                    var parentNode = leaves.Find(l =>
                                     ((l.CurrentTile.TopEnd == aiTile.TopEnd ||
                                     l.CurrentTile.TopEnd == aiTile.BottomEnd ) && l.TopNode == null ) ||
                                     ((l.CurrentTile.BottomEnd == aiTile.TopEnd ||
                                     l.CurrentTile.BottomEnd == aiTile.BottomEnd) && l.BottomNode == null)
                                     );
                    if (parentNode != null)
                    {
                        var mathcTile = aiTile;
                        Side parentSide = Side.Top; //Just for init
                        if (parentNode.CurrentTile.IsDouble())
                        {
                            if (parentNode.NeighbourNodes.Find(x => x.Key == Side.Right).Value == null)
                            {
                                parentSide = Side.Right;
                            }
                            else if (parentNode.NeighbourNodes.Find(x => x.Key == Side.Left).Value == null)
                            {
                                parentSide = Side.Left;
                            }
                        }
                        else if (!parentNode.CurrentTile.IsDouble())
                        {
                            if (parentNode.AvailableNeighbourNodes.Find(x => x.Key == Side.Top).Value == null)
                            {
                                parentSide = Side.Top;
                            }
                            if (parentNode.AvailableNeighbourNodes.Find(x => x.Key == Side.Bottom).Value == null)
                            {
                                parentSide = Side.Bottom;
                            }
                        }

                        var freeNodesNOTAvailable =
                            (GameMoves.Leaves.Select(x => x.CurrentTile).Where(x => AiTiles.Select(y => y.TopEnd).Contains(x.TopEnd)).Count() == 0 &&
                             GameMoves.Leaves.Select(x => x.CurrentTile).Where(x => AiTiles.Select(y => y.TopEnd).Contains(x.BottomEnd)).Count() == 0 &&
                             GameMoves.Leaves.Select(x => x.CurrentTile).Where(x => AiTiles.Select(y => y.BottomEnd).Contains(x.TopEnd)).Count() == 0 &&
                             GameMoves.Leaves.Select(x => x.CurrentTile).Where(x => AiTiles.Select(y => y.BottomEnd).Contains(x.BottomEnd)).Count() == 0);

                        while (TileBase.Count > 0 && freeNodesNOTAvailable)
                        {
                            AiTiles.Add(Tile.PickTileFromBase(TileBase));
                            freeNodesNOTAvailable =
                                                     (GameMoves.Leaves.Select(x => x.CurrentTile).Where(x => AiTiles.Select(y => y.TopEnd).Contains(x.TopEnd)).Count() == 0 &&
                                                      GameMoves.Leaves.Select(x => x.CurrentTile).Where(x => AiTiles.Select(y => y.TopEnd).Contains(x.BottomEnd)).Count() == 0 &&
                                                      GameMoves.Leaves.Select(x => x.CurrentTile).Where(x => AiTiles.Select(y => y.BottomEnd).Contains(x.TopEnd)).Count() == 0 &&
                                                      GameMoves.Leaves.Select(x => x.CurrentTile).Where(x => AiTiles.Select(y => y.BottomEnd).Contains(x.BottomEnd)).Count() == 0);
                        }
                        Scoring.CheckGameState(GameMoves, TileBase, PlayerTiles, AiTiles);

                        var childNode = Go(mathcTile, parentNode, parentSide, AiTiles);
                        var childSide = GameMoves.GetMatchSide(parentNode, childNode, parentSide);

                        if (Scoring.CheckGameState(GameMoves, TileBase, PlayerTiles, AiTiles))
                        {
                            return;
                        }

                        if (AImovesEnent != null)
                        {
                            AImovesEnent(parentNode, childNode, parentSide, childSide);
                        }
                        return;
                    }
                }
                AiTiles.Add(Tile.PickTileFromBase(TileBase));
                AIMoves();
            }
            else { throw new Exception("Is player's turn"); }
        }

        private Node Go(Tile tile, Node parentNode, Side parentTileSide, List<Tile> tiles)
        {
            var newMove = GameMoves.NewMove(tile, parentNode, parentTileSide);
            tiles.Remove(tile);

            IsPlayerTurn = !IsPlayerTurn;
            movesCount++;
            return newMove;
        }

        private void FirstGo(Tile tile, List<Tile> tiles)
        {
            var newMove = GameMoves.FirstMove(tile);
            tiles.Remove(tile);
            IsPlayerTurn = !IsPlayerTurn;
            movesCount++;
        }

        public GameLogic()
        {
            TileBase = new List<Tile>();
            PlayerTiles = new List<Tile>();
            AiTiles = new List<Tile>();
            Scoring = new Scoring();
            GameMoves = new Moves();
        }

        public void InitGame()
        {
            TileBase.AddRange(Tile.GenerateTiles());
            PlayerTiles.AddRange(Tile.DealTiles(TileBase));
            AiTiles.AddRange(Tile.DealTiles(TileBase));

            while (Tile.MinimalTile(AiTiles) == null && Tile.MinimalTile(PlayerTiles) == null)
            {
                AiTiles.Add(Tile.PickTileFromBase(TileBase));
            }
            if (Tile.MinimalTile(AiTiles) == null)
            {
                IsPlayerTurn = true;
            }
            else if (Tile.MinimalTile(PlayerTiles) == null)
            {
                IsPlayerTurn = false;
            }
            else
            {
                IsPlayerTurn = Tile.MinimalTile(AiTiles) > Tile.MinimalTile(PlayerTiles);
            }

            if (IsPlayerTurn)
            {

                FirstGo(Tile.MinimalTile(PlayerTiles), PlayerTiles);
            }
            else
            {
                FirstGo(Tile.MinimalTile(AiTiles), AiTiles);
            }
        }

        public void ResetGame()
        {
            TileBase.Clear();
            PlayerTiles.Clear();
            AiTiles.Clear();
            movesCount = 0;
            GameMoves = new Moves();
            Scoring = new Scoring();
        }
    }
}