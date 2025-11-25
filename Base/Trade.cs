namespace Base
{
    public class Trade
    {
        #region ===== Fields =====
        
        public required decimal  Price        = 0;
        public required decimal  Volume       = 0;
        public required string   SecurityCode = "";
        public required string   ClassCode    = "";
        public required string   Portfolio    = "";
        public required DateTime Time         = DateTime.MinValue;
        
        #endregion
    }
}
