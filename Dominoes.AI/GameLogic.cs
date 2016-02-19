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
        event NewMoveHandler AImovesEnent;
        event NewMoveHandler PlayerMovesEnent;

        private List<Tile> _tileBase = new List<Tile>();
        private List<Tile> _playerTiles = new List<Tile>();
        private List<Tile> _aiTiles = new List<Tile>();

        public List<Tile> PlayerTile { get { return _playerTiles; } }
        public bool PlayerTurn { get; private set; }
        public Moves GameMoves { get; private set; }
        private int movesCount = 0;

        public void PlayerMoves(Tile tile, Node node, TileSide tileSide)
        {
            if (PlayerTurn)
            {
                Go(tile, node, tileSide, _playerTiles);
            }
            else { throw new Exception("Is not player's turn");}
        }

        private void Go(Tile tile, Node node, TileSide tileSide, List<Tile> tiles)
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
            PlayerTurn = !PlayerTurn;
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
            PlayerTurn = !PlayerTurn;
            movesCount++;
        }

        public void InitGame()
        {
            GameMoves = new Moves();
            _tileBase.AddRange(Tile.GenerateTiles());
            _playerTiles.AddRange(Tile.DealTiles(_tileBase));
            _aiTiles.AddRange(Tile.DealTiles(_tileBase));

            PlayerTurn = Tile.MinimalTile(_aiTiles) > Tile.MinimalTile(_playerTiles);
            if (PlayerTurn)
            {
                Go(Tile.MinimalTile(_playerTiles), _playerTiles);
            }
            else
            {
                Go(Tile.MinimalTile(_aiTiles), _aiTiles);
            }
        }
    }
}
