using Base;
using System;

namespace TradesAnalyser
{
    internal enum ExitSignal
    {
        NONE, TAKE, STOP
    }
    internal class Trade
    {
        public DateTime   Time;
        public ExitSignal ExitSignal;
        public Side       Side;
        public int        Lot;
        public decimal    Pnl;
        public decimal    PnlUpdated;
    }
}
