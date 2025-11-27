using System;
using System.Windows.Media;
using WpfCommon;

namespace TradesAnalyser.Models
{
    public class TradeHour : BaseModel
    {
        #region === Properties ========================

        #region isActive
        private bool _isActive = true;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                OnPropertyChanged(nameof(IsActive));
                SelectionUpdated?.Invoke();
            }
        }
        #endregion
        #region Hour
        private int _hour = 0;
        public int Hour
        {
            get => _hour;
            set
            {
                _hour = value;
                OnPropertyChanged(nameof(Hour));
            }
        }
        #endregion
        #region PnL
        private decimal _pnl = 0;
        public decimal Pnl
        {
            get => _pnl;
            set
            {
                _pnl = value;
                Value = Math.Abs(value);

                if (_pnl > 0)
                    Color = Brushes.Green;
                else if (_pnl < 0)
                    Color = Brushes.Red;
                else
                    Color = Brushes.Transparent;
            }
        }
        #endregion
        #region Value
        private decimal _value;
        public decimal Value
        {
            get => _value;
            set
            {
                _value = value;
                if (_value > _max) Max = _pnl;
                OnPropertyChanged(nameof(Value));
            }
        }
        #endregion
        #region Max
        private decimal _max;
        public decimal Max
        {
            get => _max;
            set
            {
                _max = value;
                OnPropertyChanged(nameof(Max));
            }
        }
        #endregion
        #region Color
        private SolidColorBrush _color = Brushes.Transparent;
        public SolidColorBrush Color
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged(nameof(Color));
            }
        }
        #endregion

        #endregion // Properties

        #region === Delegates & Events ============

        public delegate void SelectionUpdate();
        public event SelectionUpdate SelectionUpdated;

        #endregion // Delegates
    }
}
