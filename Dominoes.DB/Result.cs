using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes.DB
{
    /// <summary>
    /// Game hisory results table 
    /// </summary>
    public class Result 
    {
        public Result(string nick, bool win)
        {
            Nick = nick;
            Win = win;
        }
        public Result()
        {

        }

        public int Id { get; private set; } // ID
        public string Nick { get; private set; } // Nick name
        public bool Win { get; private set; } // If player won

    }
}
