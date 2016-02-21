using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes.AI
{
    public class GameLogic
    {
        public delegate void NewMoveHandler (Node node);
        public event NewMoveHandler AImovesEnent;
        public event NewMoveHandler PlayerMovesEnent;

        public List<Tile> TileBase { get; private set; }
        public List<Tile> PlayerTiles { get; private set; }
        public List<Tile> AiTiles { get; private set; }

        public List<Tile> PlayerTile { get { return PlayerTiles; } }
        public bool PlayerTurn { get; private set; }
        public Moves GameMoves { get; private set; }
        private int movesCount = 0;

        public void PlayerMoves(Tile tile, Node node, Side tileSide)
        {
            if (PlayerTurn)
            {
                Go(tile, node, tileSide, PlayerTiles);
            }
            else { throw new Exception("Is not player's turn");}
        }

        private void Go(Tile tile, Node node, Side tileSide, List<Tile> tiles)
        {
            var newMove = GameMoves.NewMove(tile, node, tileSide);
            tiles.Remove(tile);
            if (!PlayerTurn && AImovesEnent != null)
            {
                AImovesEnent(newMove);
            }
            else if (PlayerTurn && PlayerMovesEnent != null)
            {
                PlayerMovesEnent(newMove);
            }
            PlayerTurn = true;// !PlayerTurn;
            movesCount++;
        }

        private void Go(Tile tile, List<Tile> tiles)
        {
            var newMove = GameMoves.FirstMove(tile);
            tiles.Remove(tile);
            if (!PlayerTurn && AImovesEnent != null)
            {
                AImovesEnent(newMove);
            }
            else if (PlayerTurn && PlayerMovesEnent != null)
            {
                PlayerMovesEnent(newMove);
            }
            PlayerTurn = true; //!PlayerTurn;
            movesCount++;
        }

        public GameLogic()
        {
            TileBase = new List<Tile>();
            PlayerTiles = new List<Tile>();
            AiTiles = new List<Tile>();

            GameMoves = new Moves();
        }

        public void InitGame()
        {
            TileBase.AddRange(Tile.GenerateTiles());
            PlayerTiles.AddRange(Tile.DealTiles(TileBase));
            AiTiles.AddRange(Tile.DealTiles(TileBase));

            PlayerTurn = true;// Tile.MinimalTile(AiTiles) > Tile.MinimalTile(PlayerTiles);
            if (PlayerTurn)
            {
                Go(Tile.MinimalTile(PlayerTiles), PlayerTiles);
            }
            else
            {
                Go(Tile.MinimalTile(AiTiles), AiTiles);
            }
        }
    }
}
