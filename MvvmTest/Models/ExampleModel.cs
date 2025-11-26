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
        }
    }
}
