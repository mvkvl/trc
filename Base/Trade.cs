namespace Base
{
    public class Trade
    {
        #region ===== Fields =====
        
        public required decimal  Price        = 0;
        public required string   SecurityCode = "";
        public required string   ClassCode    = "";
        public required string   Portfolio    = "";
        public required DateTime Time         = DateTime.MinValue;

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
