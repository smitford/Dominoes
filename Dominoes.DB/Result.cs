using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes.DB
{
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
        public int Id { get; private set; }
        public string Nick { get; private set; }
        public bool Win { get; private set; }

    }
}
