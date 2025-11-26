using System;
using System.Collections.Generic;

namespace Calculator
{

    public class CalcConfig
    {
        internal decimal _depo;
        internal int     _lot;
        internal decimal _take;
        internal decimal _stop;
        internal decimal _fee;
        internal int     _deals;
        internal decimal _profit;
        internal decimal _go;
        internal decimal _minDepoPrc;

        public CalcConfig WithDepo(string input)
        {
            decimal.TryParse(input, out this._depo);
            return this;
        }
        public CalcConfig WithLot(string input)
        {
            int.TryParse(input, out this._lot);
            return this;
        }
        public CalcConfig WithTake(string input)
        {
            decimal.TryParse(input, out this._take);
            return this;
        }
        public CalcConfig WithStop(string input)
        {
            decimal.TryParse(input, out this._stop);
            return this;
        }
        public CalcConfig WithFee(string input)
        {
            decimal.TryParse(input, out this._fee);
            return this;
        }
        public CalcConfig WithDealsAmount(string input)
        {
            int.TryParse(input, out this._deals);
            return this;
        }
        public CalcConfig WithProfit(string input)
        {
            decimal.TryParse(input, out this._profit);
            return this;
        }
        public CalcConfig WithGo(string input)
        {
            decimal.TryParse(input, out this._go);
            return this;
        }
        public CalcConfig WithMinDepoPrc(string input)
        {
            decimal.TryParse(input, out this._minDepoPrc);
            return this;
        }
    }

    /// <summary>
    /// Calculated Data Type
    /// </summary>
    public class CalculatedData
    {

        public Strategy Strategy { get; set; }

        internal decimal _initialDepo;
        public string InitialDepo
        {
            get
            {
                return _initialDepo.ToString("N0");
            }
        }

        internal decimal _resultingDepo;
        internal decimal ResultingDepoValue
        {
            get => _resultingDepo;
            set
            {
                _resultingDepo = value;
                _profit = value - _initialDepo;
                _profitPercent = Math.Round(_profit / _initialDepo * 100, 2);
            }
        }
        public string ResultingDepo
        {
            get
            {
                return _resultingDepo.ToString("N0");
            }
        }

        internal decimal _profit;
        /// <summary>
        /// П/У
        /// </summary>
        public string Profit
        {
            get
            {
                return _profit.ToString("N0");
            }
        }

        internal decimal _profitPercent;
        /// <summary>
        /// П/У %
        /// </summary>
        public string ProfitPercent
        {
            get
            {
                return _profitPercent.ToString("N2");
            }
        }

        internal decimal _maxDrawDown;
        /// <summary>
        /// Максимальная просадка абсолютная
        /// </summary>
        public string MaxDrawDown
        {
            get
            {
                return _maxDrawDown.ToString("N0");
            }
        }

        internal decimal _maxDrawDownPercent;
        /// <summary>
        /// Максимальная просадка в процентах
        /// </summary>
        public string MaxDrawDownPercent { get
            {
                return _maxDrawDownPercent.ToString("N2");
            }
        }

        internal List<decimal> Equity { get; set; }

        public CalculatedData(decimal initialDepo, Strategy strategy)
        {
            _initialDepo = initialDepo;
            Strategy = strategy;
        }

    }

    internal class Calc
    {

        private readonly Random _rnd = new Random();

        public List<CalculatedData> Calculate(CalcConfig cfg)
        {
            List<CalculatedData> calculatedData = new List<CalculatedData>
            {
                new CalculatedData(cfg._depo, Strategy.FIXED),
                new CalculatedData(cfg._depo, Strategy.CPT),
                new CalculatedData(cfg._depo, Strategy.UG),
                new CalculatedData(cfg._depo, Strategy.DG),
            };
            var deals = RandomDeals(cfg);
            foreach (CalculatedData data in calculatedData)
            {
                Simulate(cfg, data, deals);
            }
            return calculatedData;
        }

        private List<bool> RandomDeals(CalcConfig cfg)
        {
            List<bool> result = new List<bool>(); // true - profit, false - loss
            for (int i = 0; i < cfg._deals; i++)
            {
                var v = Convert.ToDecimal(_rnd.Next(0, 100));
                result.Add(v < cfg._profit);
            }
            return result;
        }
        private void Simulate(CalcConfig cfg, CalculatedData data, List<bool> deals)
        {
            var stc = StrategyCalculatorFactory.Get(data.Strategy, cfg);
            foreach (bool v in deals)
            {
                if (v) stc.Profit();
                else   stc.Loss();
            }
            data.ResultingDepoValue = stc.Balance();
            data._maxDrawDown = stc.MaxDrawDown();
            data._maxDrawDownPercent = stc.RelativeDrawDown();
            data.Equity = stc.Equity();
        }

    }

    #region ==== Strategy Calculators 

    internal interface IStrategyCalculator
    {
        void Profit();
        void Loss();
        decimal Balance();
        decimal MaxDrawDown();
        decimal RelativeDrawDown();
        List<decimal> Equity();
    }

    internal abstract class AbstractStrategyCalculator : IStrategyCalculator
    {
        private decimal maxBalance;
        private decimal minBalance = decimal.MaxValue;
        private decimal maxDrawDown = 0;
        private decimal relDrawDown = 0;
        private readonly List<decimal> equity = new List<decimal>();

        public abstract decimal Balance();
        public virtual void Loss()
        {
            var v = Balance();
            equity.Add(v);
            if (v < minBalance)
            {
                minBalance = v;
                UpdateDrawDown();
            }
        }
        public virtual void Profit()
        {
            var v = Balance();
            equity.Add(v);
            if (v > maxBalance)
            {
                maxBalance = v;
                minBalance = decimal.MaxValue;
            }
        }
        private void UpdateDrawDown()
        {
            if (maxBalance > minBalance && maxDrawDown < maxBalance - minBalance)
            {
                maxDrawDown = maxBalance - minBalance;
                var prc = maxDrawDown / maxBalance * 100;
                if (prc > relDrawDown)
                {
                    relDrawDown = Math.Round(prc, 2);
                }
            }
        }
        public decimal MaxDrawDown()
        {
            return maxDrawDown;
        }
        public decimal RelativeDrawDown()
        {
            return relDrawDown;
        }
        public List<decimal> Equity()
        {
            return equity;
        }
        protected int CalculateLot(decimal balance, decimal go, decimal prc)
        {
            if (prc > 100)
            {
                prc = 100;
            }
            decimal l = balance / go / 100 * prc;
            return (int)l;
        }
    }

    internal class FixedStrategyCalculator : AbstractStrategyCalculator
    {
        private readonly CalcConfig cfg;
        private decimal balance;
        internal FixedStrategyCalculator(CalcConfig cfg)
        {
            this.balance = cfg._depo;
            this.cfg = cfg;
        }
        public override void Loss()
        {
            balance -= cfg._lot * (cfg._stop + cfg._fee * 2);
            if (balance < 0)
            {
                balance = 0;
            }
            base.Loss();
        }
        public override void Profit()
        {
            balance += cfg._lot * (cfg._take - cfg._fee * 2);
            base.Profit();
        }
        public override decimal Balance()
        {
            return balance;
        }
    }
    internal class CapitalizedStrategyCalculator : AbstractStrategyCalculator
    {
        private readonly CalcConfig cfg;
        private readonly decimal dealPercent; // часть депозита под сделку
        private decimal balance;
        private int lotSize;
        internal CapitalizedStrategyCalculator(CalcConfig cfg)
        {
            this.balance = cfg._depo;
            this.lotSize = cfg._lot;
            this.cfg = cfg;
            this.dealPercent = cfg._go * cfg._lot / cfg._depo * 100 ;
        }
        public override void Loss()
        {
            balance -= lotSize * (cfg._stop + cfg._fee * 2);
            if (balance < 0)
            {
                balance = 0;
            }
            base.Loss();
        }
        public override void Profit()
        {
            balance += lotSize * (cfg._take - cfg._fee * 2);
            UpdateLotSize();
            base.Profit();
        }
        public override decimal Balance()
        {
            return balance;
        }
        private void UpdateLotSize()
        {
            var newLot = CalculateLot(balance, cfg._go, dealPercent);
            if (newLot > lotSize)
            {
                lotSize = newLot;
            }
        }
    }
    internal class ProgressiveStrategyCalculator : AbstractStrategyCalculator
    {
        private readonly CalcConfig cfg;
        private readonly decimal multiplier;
        private int lotSize;
        private decimal balance;
        internal ProgressiveStrategyCalculator(CalcConfig cfg)
        {
            this.multiplier = cfg._take / cfg._stop;
            this.lotSize = CalculateLot(cfg._depo, cfg._go, cfg._minDepoPrc);
            this.balance = cfg._depo;
            this.cfg = cfg; 
        }
        public override void Loss()
        {
            balance -= (cfg._stop + cfg._fee * 2) * lotSize;
            UpdateLotSize(1);
            base.Loss();
        }
        public override void Profit()
        {
            balance += (cfg._take - cfg._fee * 2) * lotSize;
            UpdateLotSize(multiplier);
            base.Profit();
        }
        public override decimal Balance()
        {
            return balance;
        }
        private void UpdateLotSize(decimal m)
        {
            lotSize = CalculateLot(cfg._depo, cfg._go, cfg._minDepoPrc * m);
        }
    }
    internal class DowngradedStrategyCalculator : AbstractStrategyCalculator
    {
        private readonly CalcConfig cfg;
        private int lotSize;
        private decimal balance;
        internal DowngradedStrategyCalculator(CalcConfig cfg)
        {
            this.cfg = cfg;
            this.lotSize = cfg._lot;
            this.balance = cfg._depo;
        }
        public override void Loss()
        {
            balance -= (cfg._stop + cfg._fee * 2) * lotSize;
            UpdateLotSize();
            base.Loss();
        }
        public override void Profit()
        {
            balance += (cfg._take - cfg._fee * 2) * lotSize;
            lotSize = cfg._lot;
            base.Profit();
        }
        public override decimal Balance()
        {
            return balance;
        }
        private void UpdateLotSize()
        {
            lotSize /= 2;
            if (lotSize == 0)
            {
                lotSize = 1;
            }
        }
    }
    
    internal class StrategyCalculatorFactory
    {
        public static IStrategyCalculator Get(Strategy strategy, CalcConfig cfg)
        {
            switch (strategy)
            {
                case Strategy.FIXED:
                    return new FixedStrategyCalculator(cfg);
                case Strategy.CPT:
                    return new CapitalizedStrategyCalculator(cfg);
                case Strategy.UG:
                    return new ProgressiveStrategyCalculator(cfg);
                case Strategy.DG:
                    return new DowngradedStrategyCalculator(cfg);
                default:
                    throw new Exception("illegal strategy requested");
            }
        }

    }

    #endregion

}
