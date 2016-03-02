using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes.AI
{
    public class Scoring
    {
        public delegate void GameOverHandler(bool playerWin, int scores);
        public event GameOverHandler GameOver;

        public void CheckGameState(Moves moves, List<Tile> tileBase, List<Tile> playerTiles, List<Tile> aiTiles)
        {
            if (playerTiles.Count == 0)
            {
                var scores = CountScores(aiTiles);
                if (GameOver != null)
                {
                    GameOver(true, scores);
                }
            }
            else if (aiTiles.Count == 0)
            {
                var scores = CountScores(playerTiles);
                if (GameOver != null)
                {
                    GameOver(false, scores);
                }
            }
            else if ((tileBase.Count == 0 && (moves.Leaves.Select(x => x.CurrentTile).Where(x => playerTiles.Select(y => y.TopEnd).Contains(x.TopEnd)).Count() == 0 ||
                     moves.Leaves.Select(x => x.CurrentTile).Where(x => playerTiles.Select(y => y.TopEnd).Contains(x.BottomEnd)).Count() == 0 ||
                     moves.Leaves.Select(x => x.CurrentTile).Where(x => playerTiles.Select(y => y.BottomEnd).Contains(x.TopEnd)).Count() == 0 ||
                     moves.Leaves.Select(x => x.CurrentTile).Where(x => playerTiles.Select(y => y.BottomEnd).Contains(x.BottomEnd)).Count() == 0)) 
                     ||
                     (tileBase.Count == 0 && (moves.Leaves.Select(x => x.CurrentTile).Where(x => aiTiles.Select(y => y.TopEnd).Contains(x.TopEnd)).Count() == 0 ||
                     moves.Leaves.Select(x => x.CurrentTile).Where(x => aiTiles.Select(y => y.TopEnd).Contains(x.BottomEnd)).Count() == 0 ||
                     moves.Leaves.Select(x => x.CurrentTile).Where(x => aiTiles.Select(y => y.BottomEnd).Contains(x.TopEnd)).Count() == 0 ||
                     moves.Leaves.Select(x => x.CurrentTile).Where(x => aiTiles.Select(y => y.BottomEnd).Contains(x.BottomEnd)).Count() == 0))
                     )
            {
                var playerScores = CountScores(playerTiles);
                var aiScores = CountScores(aiTiles);
                var scores = playerScores - aiScores;
                if (GameOver != null)
                {
                    GameOver(scores > 0, scores);
                }
            }
        }

        public int CountScores(List<Tile> tiles)
        {
            int summ = 0;
            foreach (var tile in tiles)
            {
                if (tile.IsDouble())
                {
                    if (tile.TopEnd == 0)
                    {
                        summ += 25;
                    }
                    if (tile.TopEnd == 6)
                    {
                        summ += 50;
                    }
                    summ += tile.TopEnd + tile.BottomEnd;

                }
                else
                {
                    summ += tile.TopEnd + tile.BottomEnd;
                }
            }
            return summ;
        }
    }
}
