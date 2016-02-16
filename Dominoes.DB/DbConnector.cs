using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes.DB
{
    public class DbConnector
    {
        HistoryDb _history;

        public DbConnector()
        {
            _history = new HistoryDb();
        }

        public Result Add(String nick, bool win)
        {
            var result =_history.Results.Add(new Result(nick, win));
            _history.SaveChanges();
            return result;
        }
        public bool Remove(Result result)
        {
            try
            {
                _history.Results.Remove(result);
                _history.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public List<Result> GetHistory()
        {
            return _history.Results.ToList<Result>();
        }
    }
}
