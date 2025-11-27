using System;
using System.Collections.Generic;
using static TradesAnalyser.Models.TradeHour;

namespace TradesAnalyser.Models
{
    internal class TradeHourContainer
    {

        List<List<TradeHour>> Trades;

        private SelectionUpdate processorDelegate;

        public TradeHourContainer(SelectionUpdate processorDelegate) {
            this.processorDelegate = processorDelegate;
            Init();
        }

        public List<List<TradeHour>> Get()
        {
            return Trades;
        }

        private void Init()
        {
            var th = new List<List<TradeHour>>();
            for (int i = 0; i <= 6; i++)
            {
                var dh = new List<TradeHour>();
                for (int j = 0; j <= 23; j++)
                {
                    var trhr = new TradeHour() { Hour = j };
                    trhr.SelectionUpdated += processorDelegate;
                    dh.Add(trhr);
                }
                th.Add(dh);
            }
            Trades = th;
        }

        public void Reset()
        {
            Reset(false);
        }
        public void Reset(bool resetActive)
        {
            foreach (List<TradeHour> lth in Trades)
            {
                foreach (TradeHour th in lth)
                {
                    th.SelectionUpdated -= processorDelegate;
                    if (resetActive)
                        th.IsActive = true;
                    th.Pnl = 0;
                    th.Max = 0;
                    th.SelectionUpdated += processorDelegate;
                }
            }
        }
        public bool Accepts(Trade t, out int day, out int hour)
        {
            day = ((int)t.Time.DayOfWeek);
            day -= 1;
            if (day < 0)
                day = 6;
            hour = t.Time.Hour;
            if (!Trades[day][hour].IsActive)
                return false;
            return true;
        }
        public bool Accepts(Trade t)
        {
            return Accepts(t, out int _, out int _);
        }
        public bool TryAdd(Trade t, decimal maxPnl)
        {
            if (Accepts(t, out int d, out int h))
            {
                Trades[d][h].Pnl += t.Pnl;
                Trades[d][h].Max  = maxPnl;
                return true;
            }
            return false;
        }
    }
}