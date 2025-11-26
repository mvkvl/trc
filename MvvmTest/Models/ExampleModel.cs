using Base;
using Exchange;
using System;

namespace MvvmTest.Models
{
    public class ExampleModel : BaseModel
    {

        private Server _server;

        private decimal _volume;
        public decimal Volume
        {
            get => _volume;
            set { 
                _volume = value;
                OnPropertyChanged(nameof(Volume));
            }
        }

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }

        private decimal _total;
        public decimal Total
        {
            get => _total;
            set
            {
                _total = value;
                OnPropertyChanged(nameof(Total));
            }
        }

        private DateTime _time;
        public DateTime Time
        {
            get => _time;
            set
            {
                _time = value;
                OnPropertyChanged(nameof(Time));
            }
        }

        private Side _side;
        public Side Side
        {
            get => _side;
            set
            {
                _side = value;
                OnPropertyChanged(nameof(Side));
            }
        }

        public ExampleModel()
        {
            _server = new Server();
            _server.TradeReceivedEvent += TradeReceivedEventHandler;
        }

        private void TradeReceivedEventHandler(Exchange.TradeEvent te)
        {
            Volume = te.Volume;
            Price = te.Price;
            Time = te.Time;
            Side = te.Side;
            Total = te.Price * te.Volume;
        }
    }
}
