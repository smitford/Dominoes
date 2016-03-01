using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes.AI
{
    public class GameLogic
    {
        public delegate void NewMoveHandler (Node parentNode, Node childNode);
        public event NewMoveHandler AImovesEnent;
        public event NewMoveHandler PlayerMovesEnent;

        public List<Tile> TileBase { get; private set; }
        public List<Tile> PlayerTiles { get; private set; }
        public List<Tile> AiTiles { get; private set; }

        public List<Tile> PlayerTile { get { return PlayerTiles; } }
        public bool IsPlayerTurn { get; private set; }
        public Moves GameMoves { get; private set; }
        private int movesCount = 0;
        private Scoring _scoring;

        public Node PlayerMoves(Tile tile, Node node, Side tileSide)
        {
            if (IsPlayerTurn)
            {
                return Go(tile, node, tileSide, PlayerTiles);
            }
            else { throw new Exception("Is not player's turn");}
        }

        private Node Go(Tile tile, Node parentNode, Side tileSide, List<Tile> tiles)
        {
            var newMove = GameMoves.NewMove(tile, parentNode, tileSide);
            tiles.Remove(tile);
            if (!IsPlayerTurn && AImovesEnent != null)
            {
                AImovesEnent(parentNode, newMove);
            }
            else if (IsPlayerTurn && PlayerMovesEnent != null)
            {
                PlayerMovesEnent(parentNode, newMove);
            }
            IsPlayerTurn = !IsPlayerTurn;
            movesCount++;
            return newMove;
        }

        private void Go(Tile tile, List<Tile> tiles)
        {
            var newMove = GameMoves.FirstMove(tile);
            tiles.Remove(tile);
            if (!IsPlayerTurn && AImovesEnent != null)
            {
                AImovesEnent(null, newMove);
            }
            else if (IsPlayerTurn && PlayerMovesEnent != null)
            {
                PlayerMovesEnent(null, newMove);
            }
            IsPlayerTurn = !IsPlayerTurn;
            movesCount++;

            _scoring.CheckGameState(GameMoves, PlayerTiles, AiTiles);
        }

        public GameLogic()
        {
            TileBase = new List<Tile>();
            PlayerTiles = new List<Tile>();
            AiTiles = new List<Tile>();
            _scoring = new Scoring();
            GameMoves = new Moves();
        }

        public void InitGame()
        {
            TileBase.AddRange(Tile.GenerateTiles());
            PlayerTiles.AddRange(Tile.DealTiles(TileBase));
            AiTiles.AddRange(Tile.DealTiles(TileBase));

            if (Tile.MinimalTile(AiTiles) == null && Tile.MinimalTile(PlayerTiles) == null)
            {
                ResetGame();
                InitGame();
                return;
            }
            else if (Tile.MinimalTile(AiTiles) == null)
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
                Go(Tile.MinimalTile(PlayerTiles), PlayerTiles);
            }
            else
            {
                Go(Tile.MinimalTile(AiTiles), AiTiles);
            }
        }

        public void ResetGame()
        {
            TileBase.Clear();
            PlayerTiles.Clear();
            AiTiles.Clear();
            movesCount = 0;
            GameMoves = new Moves();
        }
    }
}
