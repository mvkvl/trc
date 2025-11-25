namespace Base
{
    public class Trade
    {
        #region ===== Fields =====
        
        public required decimal  Price;
        public required decimal  Volume;
        public required string   SecurityCode;
        public required string   ClassCode;
        public required string   Portfolio;
        public required DateTime Time;
        
        #endregion
    }
}
