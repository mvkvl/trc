using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

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
        public decimal InitialDepo { get; set; }
        public decimal ResultingDepo { get; set; }
        /// <summary>
        /// П/У
        /// </summary>        private decimal Profit { get; set; }
        /// <summary>
        /// П/У %
        /// </summary>
        public decimal ProfitPercent { get; set; }
        /// <summary>
        /// Максимальная просадка абсолютная
        /// </summary>
        public decimal MaxDrawDown { get; set; }
        /// <summary>
        /// Максимальная просадка в процентах
        /// </summary>
        public decimal MaxDrawDownPercent { get; set; }

        public CalculatedData(decimal initialDepo, Strategy strategy)
        {
            InitialDepo = initialDepo;
            Strategy = strategy;
        }

    }

    internal class Calc
    {

        private Random _rnd = new Random();
        private decimal _lotPercent = 0;

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

        private List<decimal> RandomDeals(CalcConfig cfg)
        {
            List<decimal> result = new List<decimal>();
            for (int i = 0; i < cfg._deals; i++)
            {
                result.Add(Convert.ToDecimal(_rnd.Next(0, 100)));
            }
            return result;
        }
        private void Simulate(CalcConfig cfg, CalculatedData data, List<decimal> deals)
        {
            var stc = StrategyCalculatorFactory.Get(data.Strategy, cfg);
            foreach (decimal d in deals)
            {
                if (d < cfg._profit) stc.Profit();
                else stc.Loss();
            }
            data.ResultingDepo = stc.Balance();
        }

    }

    #region ==== Strategy Calculators 

    internal interface IStrategyCalculator
    {
        void Profit();
        void Loss();
        decimal Balance();
    }
    internal abstract class AbstractStrategyCalculator : IStrategyCalculator
    {
        public abstract decimal Balance();
        public abstract void Loss();
        public abstract void Profit();
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
        private CalcConfig cfg;
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
        }
        public override void Profit()
        {
            balance += cfg._lot * (cfg._take - cfg._fee * 2);
        }
        public override decimal Balance()
        {
            return balance;
        }
    }
    internal class CapitalizedStrategyCalculator : AbstractStrategyCalculator
    {
        private CalcConfig cfg;
        private decimal balance;
        private int lotSize;
        private decimal dealPercent; // часть депозита под сделку
        internal CapitalizedStrategyCalculator(CalcConfig cfg)
        {
            this.balance = cfg._depo;
            this.lotSize = cfg._lot;
            this.cfg = cfg;
            this.dealPercent = cfg._go * cfg._lot * 100 / cfg._depo;
        }
        public override void Loss()
        {
            balance -= lotSize * (cfg._stop + cfg._fee * 2);
            if (balance < 0)
            {
                balance = 0;
            }
        }
        public override void Profit()
        {
            balance += lotSize * (cfg._take - cfg._fee * 2);
            UpdateLotSize();
        }
        public override decimal Balance()
        {
            return balance;
        }
        private void UpdateLotSize()
        {
            decimal l = balance / cfg._go / 100 * dealPercent;
            var newLot = CalculateLot(balance, cfg._go, dealPercent);
            if (newLot > lotSize)
            {
                lotSize = newLot;
            }
        }
    }
    internal class ProgressiveStrategyCalculator : AbstractStrategyCalculator
    {
        private CalcConfig cfg;
        private decimal multiplier;
        private int currentLot;
        private decimal balance;
        internal ProgressiveStrategyCalculator(CalcConfig cfg)
        {
            this.multiplier = cfg._take / cfg._stop;
            this.currentLot = CalculateLot(cfg._depo, cfg._go, cfg._minDepoPrc);
            this.cfg = cfg; 
        }
        public override void Loss()
        {
            balance -= (cfg._stop + cfg._fee * 2) * currentLot;
            UpdateLotSize(1);
        }
        public override void Profit()
        {
            balance += (cfg._take - cfg._fee * 2) * currentLot;
            UpdateLotSize(multiplier);
        }
        public override decimal Balance()
        {
            return balance;
        }
        private void UpdateLotSize(decimal m)
        {
            currentLot = CalculateLot(cfg._depo, cfg._go, cfg._minDepoPrc * m);
        }
    }
    internal class DowngradedStrategyCalculator : AbstractStrategyCalculator
    {
        private CalcConfig cfg;
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
        }
        public override void Profit()
        {
            balance += (cfg._take - cfg._fee * 2) * lotSize;
            lotSize = cfg._lot;
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
