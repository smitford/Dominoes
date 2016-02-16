using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes.AI
{
    public class GameLogic
    {
        public delegate void AImovesHandler (Move move);
        event AImovesHandler AImoves;

        public void PlayerMoves(Move move)
        {

        }


    }
}
