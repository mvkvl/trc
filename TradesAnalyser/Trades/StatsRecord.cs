using System;
using System.Collections.Generic;
using WpfCommon;

namespace TradesAnalyser.Trades
{
    public class StatsRecord: BaseModel
    {

        internal string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        internal decimal _trades;
        public decimal TR
        {
            get => _trades;
            set
            {
                _trades = value;
                OnPropertyChanged(nameof(TR));
            }
        }


        internal decimal _result;
        public decimal RES
        {
            get => _result;
            set
            {
                _result = value;
                OnPropertyChanged(nameof(RES));
            }
        }

        internal decimal _resultLong;
        public decimal RESL
        {
            get => _resultLong;
            set
            {
                _resultLong = value;
                OnPropertyChanged(nameof(RESL));
            }
        }

        internal decimal _resultShort;
        public decimal RESS
        {
            get => _resultShort;
            set
            {
                _resultShort = value;
                OnPropertyChanged(nameof(RESS));
            }
        }

        internal decimal _profitTradesPercent;
        public decimal PTP
        {
            get => _profitTradesPercent;
            set
            {
                _profitTradesPercent = value;
                OnPropertyChanged(nameof(PTP));
            }
        }

        internal decimal _profitTradesPercentL;
        public decimal PTPL
        {
            get => _profitTradesPercentL;
            set
            {
                _profitTradesPercentL = value;
                OnPropertyChanged(nameof(PTPL));
            }
        }

        internal decimal _profitTradesPercentS;
        public decimal PTPS
        {
            get => _profitTradesPercentS;
            set
            {
                _profitTradesPercentS = value;
                OnPropertyChanged(nameof(PTPS));
            }
        }

        internal int _takes;
        public int TK
        {
            get => _takes;
            set
            {
                _takes = value;
                OnPropertyChanged(nameof(TK));
            }
        }

        internal int _stops;
        public int ST
        {
            get => _stops;
            set
            {
                _stops = value;
                OnPropertyChanged(nameof(ST));
            }
        }

        internal int _longTakes;
        public int TKL
        {
            get => _longTakes;
            set
            {
                _longTakes = value;
                OnPropertyChanged(nameof(TKL));
            }
        }

        internal int _longStops;
        public int STL
        {
            get => _longStops;
            set
            {
                _longStops = value;
                OnPropertyChanged(nameof(STL));
            }
        }

        internal int _shortTakes;
        public int TKS
        {
            get => _shortTakes;
            set
            {
                _shortTakes = value;
                OnPropertyChanged(nameof(TKS));
            }
        }

        internal int _shortStops;
        public int STS
        {
            get => _shortStops;
            set
            {
                _shortStops = value;
                OnPropertyChanged(nameof(STS));
            }
        }

        internal decimal _maxDrawDown;
        public decimal MDD
        {
            get => _maxDrawDown;
            set
            {
                _maxDrawDown = value;
                OnPropertyChanged(nameof(MDD));
            }
        }

        internal decimal _maxDrawDownPercent;
        public decimal MDDP
        {
            get => _maxDrawDownPercent;
            set
            {
                _maxDrawDownPercent = value;
                OnPropertyChanged(nameof(MDDP));
            }
        }

        internal string _description;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public void Reset()
        {
            //Title = "";
            TR = 0;
            RES = 0;
            RESL = 0;
            RESS = 0;
            PTP = 0;
            PTPL = 0;
            PTPS = 0;
            TK = 0;
            ST = 0;
            TKL = 0;
            STL = 0;
            TKS = 0;
            STS = 0;
            MDD = 0;
            MDDP = 0;
            //Description = "";
        }
    }

}
