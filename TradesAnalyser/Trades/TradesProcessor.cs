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
        internal List<StatsRecord> CalculateAll()
        {
            return null;
        }

        internal void ProcessPnl(List<Trade> trades, int multiplier, int maxLot)
        {
            if (trades == null || trades.Count == 0) return;
            
            int lotSize = 1;

            for (int i = 0; i < trades.Count; i++)
            {
                if (i == 0)
                {
                    trades[i].PnlUpdated = trades[i].Pnl;
                    continue;
                }
                if (trades[i - 1].Pnl > 0)
                {
                    lotSize *= multiplier;
                    if (lotSize > maxLot)
                        lotSize = maxLot;
                }
                else
                {
                    lotSize = 1;
                }
                trades[i].PnlUpdated = trades[i - 1].Pnl * lotSize;
            }
        }
        internal void ProcessDailyTrades(DateTime since, DateTime till, List<Trade> trades, decimal maxPnl, TradeHourContainer thc)
        {
            // обнуляем всё
            thc.Reset();
            // считаем, что нужно
            foreach (Trade t in trades)
            {
                if (t.Time < since || t.Time > till) continue;
                thc.TryAdd(t, maxPnl);
            }
        }
    }
}
