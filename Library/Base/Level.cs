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
}
