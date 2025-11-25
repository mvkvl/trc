namespace Exchange
{
    public class MockConnector
    {
        private Timer? _timer;
        public event TradeHandlerDelegate TradeEvent;
        private readonly Random _rnd = new();

        public void Connect()
        {
            this.SubscribeTrades();
        }
        public void Disconnect()
        {
            this.UnsubscribeTrades();
        }

        private void SubscribeTrades()
        {
            TimerCallback tmcb = new(GenerateTrade);
            _timer = new Timer(tmcb, null, 0, 1300);
        }
        private void UnsubscribeTrades()
        {
            if (_timer != null)
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private void GenerateTrade(object? obj)
        {
            var t = new TradeEvent
            {
                SecCode = "MOCK_SEC",
                Volume = _rnd.Next(10),
                Price = Convert.ToDecimal(_rnd.NextDouble()) * _rnd.Next(7),
                Time = DateTime.Now
            };
            TradeEvent?.Invoke(t);
        }

    }
}
