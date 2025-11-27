using System;
using System.Collections.Generic;
using WpfCommon;

namespace TradesAnalyser.Trades
{
    public class CalculatedData: BaseModel
    {
        internal decimal _result;
        internal decimal Result
        {
            get => _result;
            set
            {
                _result = value;
                OnPropertyChanged(nameof(Result));
            }
        }

        internal decimal _profitTradesPercent;
        internal decimal ProfitTradesPercent
        {
            get => _profitTradesPercent;
            set
            {
                _profitTradesPercent = value;
                OnPropertyChanged(nameof(ProfitTradesPercent));
            }
        }

        internal decimal _maxDrawDown;
        public decimal MaxDrawDown
        {
            get => _maxDrawDown;
            set
            {
                _maxDrawDown = value;
                OnPropertyChanged(nameof(MaxDrawDown));
            }
        }
                
        internal decimal _maxDrawDownPercent;
        public decimal MaxDrawDownPercent
        {
            get => _maxDrawDownPercent;
            set
            {
                _maxDrawDownPercent = value;
                OnPropertyChanged(nameof(MaxDrawDownPercent));
            }
        }
    }

}
