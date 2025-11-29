using Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TradesAnalyser.Models;

namespace TradesAnalyser.Trades
{
    internal enum RecordType
    {
        ORIG_ALL,
        UPD_ALL,
        ORIG_FLT,
        UPD_FLT
    }
    internal class StatsCalculator
    {

        public List<StatsRecord> Data;

        public StatsCalculator() {
            Data = new List<StatsRecord>
            {
                new StatsRecord { Title = KindStr(RecordType.ORIG_ALL), Description = "Исходные сделки (все)" },
                new StatsRecord { Title = KindStr(RecordType.UPD_ALL),  Description = "Lot x 3 on Profit (все)" },
                new StatsRecord { Title = KindStr(RecordType.ORIG_FLT), Description = "Исходные сделки (фильтрованные)" },
                new StatsRecord { Title = KindStr(RecordType.UPD_FLT),  Description = "Lot x 3 on Profit (фильтрованные)" }
            };
            Reset();
        }
        public void Reset()
        {
            foreach (var record in Data)
                record.Reset();
        }

        #region - Statistics calculation
        internal void Calculate(DateTime since, DateTime till, List<Trade> trades, TradeHourContainer thc)
        {
            Reset();
            Task.Run( async() => doCalculate(DateTime.MinValue, DateTime.MaxValue, SourceType.ORIGINAL, trades, null, Data[0]) );
            Task.Run(async () => doCalculate(DateTime.MinValue, DateTime.MaxValue, SourceType.UPDATED, trades, null, Data[1]) );
            Task.Run(async () => doCalculate(since, till, SourceType.ORIGINAL, trades, thc, Data[2]) );
            Task.Run(async () => doCalculate(since, till, SourceType.UPDATED, trades, thc, Data[3]) );
        }
        internal void doCalculate(DateTime since, DateTime till, SourceType st, List<Trade> trades, TradeHourContainer thc, StatsRecord record)
        {
            if (trades == null || trades.Count == 0)
            {
                return;
            }

            var tt      = ByHourFilter(thc, ByDateFilter(since, till, trades));
            var ttl     = BySideFilter(Side.Buy, tt);
            var tts     = BySideFilter(Side.Sell, tt);
            record.TR   = tt.ToList().Count();
            record.TK   = CountFiltered(tt, t => t.ExitSignal == ExitSignal.TAKE);
            record.ST   = CountFiltered(tt, t => t.ExitSignal == ExitSignal.STOP);
            record.TKL  = CountFiltered(ttl, t => t.ExitSignal == ExitSignal.TAKE);
            record.STL  = CountFiltered(ttl, t => t.ExitSignal == ExitSignal.STOP);
            record.TKS  = CountFiltered(tts, t => t.ExitSignal == ExitSignal.TAKE);
            record.STS  = CountFiltered(tts, t => t.ExitSignal == ExitSignal.STOP);
            record.RES  = ValueSelector(st, tt).Aggregate((a, b) => a + b);
            record.RESL = ValueSelector(st, ttl).Aggregate((a, b) => a + b);
            record.RESS = ValueSelector(st, tts).Aggregate((a, b) => a + b);
            record.PTP  = Math.Round(tt.Count(t  => t.Pnl >= 0) / record.TR * 100, 2);
            record.PTPL = Math.Round(ttl.Count(t => t.Pnl >= 0) / (decimal)(record.TKL + record.STL) * 100, 2);
            record.PTPS = Math.Round(tts.Count(t => t.Pnl >= 0) / (decimal)(record.TKS + record.STS) * 100, 2);

            calculateDrawDown(tt, st, record);
        }
        internal void calculateDrawDown(IEnumerable<Trade> trades, SourceType st, StatsRecord record)
        {
            decimal currentBalance = 0;
            decimal maxBalance = 0;
            decimal minBalance = decimal.MaxValue;
            decimal maxDrawDown = 0;
            decimal relDrawDown = 0;

            bool profitableDeal = false;

            foreach(Trade t in trades) {
                switch (st)
                {
                    case SourceType.ORIGINAL:
                        currentBalance += t.Pnl;
                        profitableDeal = t.Pnl > 0;
                        break;
                    case SourceType.UPDATED:
                        currentBalance += t.PnlUpdated;
                        profitableDeal = t.PnlUpdated > 0;
                        break;
                }

                if (profitableDeal && currentBalance > maxBalance)
                {
                    maxBalance = currentBalance;
                    minBalance = decimal.MaxValue;
                }
                if (!profitableDeal && currentBalance < minBalance)
                {
                    minBalance = currentBalance;
                    if (maxBalance > 0 && maxBalance > minBalance && maxDrawDown < maxBalance - minBalance)
                    {
                        maxDrawDown = maxBalance - minBalance;
                        var prc = maxDrawDown / maxBalance * 100;
                        if (prc > relDrawDown)
                        {
                            relDrawDown = Math.Round(prc, 2);
                        }
                    }
                }
            }

            record.MDD = maxDrawDown;
            record.MDDP = relDrawDown;
        }
        #endregion
        #region - PnL calculation
        internal decimal Pnl(SourceType st, List<Trade> trades)
        {
            return Pnl(DateTime.MinValue, DateTime.MaxValue, st, trades, null);
        }
        internal decimal Pnl(DateTime since, DateTime till, SourceType st, List<Trade> trades)
        {
            return Pnl(since, till, st, trades, null);
        }
        internal decimal Pnl(DateTime since, DateTime till, SourceType st, List<Trade> trades, TradeHourContainer thc)
        {
            if (trades == null || trades.Count == 0)
            {
                return 0;
            }
            return ValueSelector(st, ByHourFilter(thc, ByDateFilter(since, till, trades))).Aggregate((acc, next) => acc + next);
        }
        #endregion
        #region - Equity calculation
        internal List<decimal> Equity(SourceType st, List<Trade> trades)
        { 
            return Equity(DateTime.MinValue, DateTime.MaxValue, st, trades, null);
        }
        internal List<decimal> Equity(DateTime since, DateTime till, SourceType st, List<Trade> trades)
        {
            return Equity(since, till, st, trades, null);
        }
        internal List<decimal> Equity(DateTime since, DateTime till, SourceType st, List<Trade> trades, TradeHourContainer thc)
        {
            return Aggregate(ValueSelector(st, ByHourFilter(thc, ByDateFilter(since, till, trades))).ToList());
        }

        private List<decimal> Aggregate(List<decimal> values)
        {
            List<decimal> result = new List<decimal>();
            if (values == null || values.Count == 0)
            {
                return result;
            }
            result.Add(values[0]);
            for (int i = 1; i < values.Count; i++)
            {
                result.Add(result[i - 1] + values[i]);
            }
            return result;
        }
        #endregion

        #region - Tools
        public string KindStr(RecordType rt)
        {
            switch (rt)
            {
                case RecordType.ORIG_ALL:
                    return "Исходные (все)";
                case RecordType.UPD_ALL:
                    return "Изменённые (все)";
                case RecordType.ORIG_FLT:
                    return "Исходные (фильтр по часам и датам)";
                case RecordType.UPD_FLT:
                    return "Изменённые (фильтр по часам и датам)";
            }
            return "";
        }
        #endregion

        #region - Filters & Selectors

        internal IEnumerable<Trade> ByDateFilter(DateTime since, DateTime till, IEnumerable<Trade> trades)
        {
            return trades.Where(t => t.Time >= since && t.Time <= till);
        }
        internal IEnumerable<Trade> ByHourFilter(TradeHourContainer thc, IEnumerable<Trade> trades)
        {
            if (thc == null)
            {
                return trades;
            }
            return trades.Where(t => thc.Accepts(t));
        }
        internal IEnumerable<Trade> BySideFilter(Side side, IEnumerable<Trade> trades)
        {
            return trades.Where(t => t.Side == side);
        }
        internal IEnumerable<decimal> ValueSelector(SourceType st, IEnumerable<Trade> trades)
        {
            if (st == SourceType.ORIGINAL)
                return trades.Select(t => t.Pnl);
            else
                return trades.Select(t => t.PnlUpdated);
        }

        internal int CountFiltered(IEnumerable<Trade> trades, Func<Trade, bool> predicate)
        {
            if (!trades.Any())
            {
                return 0;
            }
            return trades.Count(t => predicate(t));
        }

        #endregion

    }
}
