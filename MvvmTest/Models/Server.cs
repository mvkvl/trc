using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MvvmTest.Models
{
    internal class Server : BaseModel
    {

        private Random _rnd = new Random();
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


        public Server()
        { 
            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += TimerElapsed;
            timer.Start();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Number = _rnd.Next(0, 100);
        }

    }
}
