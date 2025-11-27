using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;
using TradesAnalyser.Commands;
using TradesAnalyser.Trades;
using WpfCommon;

namespace TradesAnalyser.Models
{
    public class Model: BaseModel
    {
        #region === Fields ====================================

        private TradeDataProvider _tradesProvider;
        private TradeAnalyser     _tradesAnalyser;

        #endregion

        #region === Properties ================================

        private decimal _depo;
        public decimal Depo {
            get => _depo;
            set
            {
                _depo = value; 
                OnPropertyChanged(nameof(Depo));
            }
        }

        public ICommand CmdCalculate
        {
            get => cmdCalculate;
        }
        public ICommand CmdFileOpen
        {
            get => cmdFileOpen;
        }

        #endregion

        #region === Commands ==================================

        private CommandDelegate cmdCalculate;
        private CommandDelegate cmdFileOpen;

        #endregion

        #region === Constructors ==============================

        public Model()
        {
            _tradesProvider = new TradeDataProvider();
            _tradesAnalyser = new TradeAnalyser();
            cmdCalculate = new CommandDelegate(CalculateButtonClickHandler);
            cmdFileOpen  = new CommandDelegate(FileOpenButtonClickHandler);
        }

        #endregion

        #region === Handlers ==================================

        private void CalculateButtonClickHandler(object param)
        {
            _tradesAnalyser.Process(_tradesProvider.Trades, 3);
            var pnl1 = _tradesAnalyser.OriginalPnl(_tradesProvider.Trades);
            var pnl2 = _tradesAnalyser.UpdatedPnl(_tradesProvider.Trades);
            MessageBox.Show($"Original: {pnl1}\nUpdated: {pnl2}");
        }
        private void FileOpenButtonClickHandler(object param)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Filter = "Файлы сделок|*.csv",
                    Multiselect = false
                };
                if (ofd.ShowDialog() == true)
                {
                    _tradesProvider.Load(ofd.FileName);
                    MessageBox.Show($"{_tradesProvider.Trades.Count} trade(s) loaded");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message); 
            }
        }

        #endregion
    }
}
