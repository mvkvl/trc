using System;
using System.Timers;

namespace MvvmTest.Models
{
    public class ExampleModel : BaseModel
    {

        private int _number;
        public int Number
        {
            get => _number;
            set
            {
                _number = value;
                OnPropertyChanged(nameof(Number));
            }
        }

        private Random _rnd = new Random();

        public ExampleModel() {
            System.Timers.Timer tm = new System.Timers.Timer
            {
                Interval = 1000
            };
            tm.Elapsed += TimerElapsed;
            tm.Start();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Number = _rnd.Next(0, 100);
        }

    }
}
