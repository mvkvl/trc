using System;
using System.IO;

namespace Base
{
    public class Trade
    {
        #region ===== Fields =====
        
        public decimal  Price        = 0;
        public string   SecurityCode = "";
        public string   ClassCode    = "";
        public string   Portfolio    = "";
        public DateTime Time         = DateTime.MinValue;

        #endregion
        #region ===== Properties =====
        private decimal _volume = 0;

        public decimal Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                if (value < 0)
                {
                    throw new InvalidDataException("volume can't be negative");
                }
                _volume = value;
            }
        }
        #endregion

    }
}
