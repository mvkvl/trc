using Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exchange
{
    public class TradeEvent
    {
        public string   SecCode;
        public decimal  Volume;
        public decimal  Price;
        public DateTime Time;
        public Side     Side;
    }

    public delegate void TradeHandlerDelegate(TradeEvent evt);
}
