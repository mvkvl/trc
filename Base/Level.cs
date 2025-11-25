using System.Text;

namespace Base
{

    #region === LEVEL ===
    public class Level
    {
        #region ====== Constants ======
        private static readonly decimal LOT_SIZE = 1;
        #endregion

        #region ====== Fields ======

        /// <summary>
        /// Цена
        /// </summary>
        public decimal Price = 0;

        /// <summary>
        /// Рабочий (целевой) лот
        /// </summary>
        public decimal TargetLot = LOT_SIZE;

        /// <summary>
        /// Открытый объём
        /// </summary>
        public decimal Volume = 0;

        #endregion

        #region === Constructors ===

        public Level()
        {
        }
        public Level(decimal price)
        {
            Price = price;
        }

        #endregion

        #region ====== Methods ======

        public override string ToString()
        {
            return $"Level: Price={this.Price}; TargetLots={this.TargetLot}; OpenVolume={this.Volume}";
        }
  
        #endregion

    }
    #endregion

    #region === NETWORK ===
    public class Network
    {

        #region ====== Fields ======

        /// <summary>
        /// Уровни сетки
        /// </summary>
        private readonly List<Level> Levels = [];

        /// <summary>
        /// Направление сетки
        /// </summary>
        private readonly Side Side = Side.Sell;

        /// <summary>
        /// Шаг сетки (если сетка равномерная)
        /// </summary>
        private readonly decimal Step = 0;

        #endregion

        #region === Constructors ===

        /// <summary>
        /// Создать неравномерную short-сетку
        /// </summary>
        public Network()
        {
        }

        /// <summary>
        /// Создать неравномерную направленную сетку
        /// </summary>
        /// <param name="side">направление сетки</param>
        public Network(Side side)
        {
            this.Side = side;
        }
        
        /// <summary>
        /// Создать равномерную направленную сетку.
        /// </summary>
        /// <param name="side">направление</param>
        /// <param name="step">шаг цены</param>
        /// <exception cref="InvalidDataException"></exception>
        public Network(Side side, decimal step)
        {
            if (step < 0)
            {
                throw new InvalidDataException("step should not be negative");
            }
            this.Side = side;
            this.Step = step;
        }

        #endregion

        #region ===== Methods =====

        /// <summary>
        /// Преобразование направления сетки в строку
        /// </summary>
        /// <returns></returns>
        private string SideStr()
        {
            switch (Side) {
                case Side.Sell:
                    return "SHORT";
                case Side.Buy:
                    return "LONG";
            }
            return "";
        }

        /// <summary>
        /// Проверка корректности добавляемого уровня
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        private bool EnsureLevelIsOk(decimal price)
        {
            // ensure new level is above (long) or below (short) latest one
            if (this.Side == Side.Sell)
            {
                if (this.Levels[Levels.Count - 1].Price <= price)
                {
                    return false;
                }
            }
            else
            {
                if (this.Levels[Levels.Count - 1].Price >= price)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Получить цену следующего уровня для "равномерной" сетки
        /// </summary>
        /// <returns></returns>
        private decimal GetNextLevel()
        {
            if (this.Levels.Count == 0)
            {
                throw new InvalidOperationException("unable to get next level for empty network");
            }
            if (this.Step == 0)
            {
                throw new InvalidOperationException("unable to get next level for non-uniform network");
            }
            var previousLevel = this.Levels[Levels.Count - 1].Price;
            if (this.Side == Side.Sell)
            {
                var newLevel = previousLevel - this.Step;
                if (newLevel < 0)
                {
                    newLevel = 0;
                }
                return newLevel;
            }
            return previousLevel + this.Step;
        }

        /// <summary>
        /// Добавить уровень сетки с указанной ценой
        /// </summary>
        /// <param name="price">цена уровня</param>
        public void Add(decimal price)
        {
            if (this.Step > 0 && this.Levels.Count > 0)
            {
                throw new InvalidOperationException("for uniform network use no-args Add() method to add level(s)");
            }
            if (this.Levels.Count == 0)
            {
                Levels.Add(new Level(price));
                return;
            }
            if (!EnsureLevelIsOk(price))
            {
                throw new InvalidDataException("new level violates network direction");
            }
            Levels.Add(new Level(price));
        }

        /// <summary>
        /// Добавить уровень
        /// </summary>
        public void Add()
        {
            if (this.Levels.Count == 0)
            {
                throw new InvalidOperationException("for empty network use Add(price) method to add first level");
            }
            if (this.Step == 0)
            {
                throw new InvalidOperationException("for non-uniform network use Add(price) method to add level(s)");
            }
            Levels.Add(new Level(GetNextLevel()));
        }

        /// <summary>
        /// Преобразование сетки в строку (для вывода в терминал)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{this.SideStr()} Network:");
            foreach (Level level in Levels)
            {
                sb.AppendLine($"  {level.ToString()}");
            }
            return sb.ToString();
        }

        #endregion

    }
    #endregion

}
