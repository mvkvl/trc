using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradesAnalyser.Trades
{
    internal class TradeAnalyser
    {
        internal decimal OriginalPnl(List<Trade> trades)
        {
            return trades.Select(t => t.Pnl).Aggregate((acc, next) => acc + next);
        }
        internal decimal UpdatedPnl(List<Trade> trades)
        {
            return trades.Select(t => t.PnlUpdated).Aggregate((acc, next) => acc + next);
        }

        internal void Process(List<Trade> trades, int multiplier)
        {
            if (trades.Count == 0) return;
            for (int i = 0; i < trades.Count; i++)
            {
                if (i == 0)
                {
                    trades[i].PnlUpdated = trades[i].Pnl;
                    continue;
                }
                if (trades[i - 1].Pnl > 0)
                {
                    trades[i].PnlUpdated = trades[i - 1].Pnl * multiplier;
                }
                else
                {
                    trades[i].PnlUpdated = trades[i - 1].Pnl;
                }
            }
        }
    }
}
