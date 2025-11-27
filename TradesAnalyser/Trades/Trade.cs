using Base;
using System;

namespace TradesAnalyser
{
    internal class Trade
    {
        public DateTime Time;
        public Side    Side;
        public int     Lot;
        public decimal Pnl;
        public decimal PnlUpdated;
    }
}
