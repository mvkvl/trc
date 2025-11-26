using Exchange;
using System;
using System.Timers;

namespace MvvmTest.Models
{
    public class Server
    {
        public delegate void TradeEventDelegate(TradeEvent te);
        public event TradeEventDelegate TradeReceivedEvent;
        public Server()
        {
            var cn = new MockConnector();
            cn.TradeEvent += (te) => TradeReceivedEvent?.Invoke(te);
            cn.Connect();
        }
    }
}
