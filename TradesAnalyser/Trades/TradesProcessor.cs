using System;
using System.Collections.Generic;
using System.Linq;
using TradesAnalyser.Models;

namespace TradesAnalyser.Trades
{
    internal enum SourceType {
        ORIGINAL, // исходные данные
        UPDATED   // изменённые данные (с увеличением лота при успешных сделках)
    }
    internal class TradesProcessor
    {
        internal decimal Pnl(SourceType st, List<Trade> trades)
        {
            if (trades == null || trades.Count == 0)
            {
                return 0;
            }
            switch (st)
            {
                case SourceType.ORIGINAL:
                    return trades.Select(t => t.Pnl).Aggregate((acc, next) => acc + next);
                case SourceType.UPDATED:
                    return trades.Select(t => t.PnlUpdated).Aggregate((acc, next) => acc + next);
            }
            return 0;
        }
        internal decimal Pnl(SourceType st, List<Trade> trades, TradeHourContainer thc)
        {
            if (trades == null || trades.Count == 0)
            {
                return 0;
            }
            switch (st)
            {
                case SourceType.ORIGINAL:
                    return trades.Where(t => thc.Accepts(t)).Select(t => t.Pnl).Aggregate((acc, next) => acc + next);
                case SourceType.UPDATED:
                    return trades.Where(t => thc.Accepts(t)).Select(t => t.PnlUpdated).Aggregate((acc, next) => acc + next);
            }
            return 0;
        }

        internal List<decimal> Equity(SourceType st, List<Trade> trades)
        {
            switch (st)
            {
                case SourceType.ORIGINAL:
                    return Aggregate(trades.Select(t => t.Pnl).ToList());
                case SourceType.UPDATED:
                    return Aggregate(trades.Select(t => t.PnlUpdated).ToList());
            }
            return new List<decimal>();
        }
        internal List<decimal> Equity(SourceType st, List<Trade> trades, TradeHourContainer thc)
        {
            switch (st)
            {
                case SourceType.ORIGINAL:
                    return Aggregate(trades.Where(t => thc.Accepts(t)).Select(t => t.Pnl).ToList());
                case SourceType.UPDATED:
                    return Aggregate(trades.Where(t => thc.Accepts(t)).Select(t => t.PnlUpdated).ToList());
            }
            return new List<decimal>();
        }

        private List<decimal> Aggregate(List<decimal> values)
        {
            List<decimal> result = new List<decimal>();
            if (values == null || values.Count == 0)
            {
                return result;
            }
            result.Add(values[0]);
            for(int i = 1; i < values.Count; i++)
            {
                result.Add(values[i - 1] + values[i]);
            }
            return result;
        }

        internal void ProcessPnl(List<Trade> trades, int multiplier)
        {
            if (trades == null || trades.Count == 0) return;
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
        internal void ProcessDailyTrades(DateTime since, DateTime till, List<Trade> trades, TradeHourContainer thc)
        {
            // обнуляем всё
            thc.Reset();
            // считаем, что нужно
            foreach (Trade t in trades)
            {
                if (t.Time < since || t.Time > till) continue;
                thc.TryAdd(t);
            }
        }
    }
}
