using Microsoft.Win32;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TradesAnalyser.Commands;
using TradesAnalyser.Trades;
using WpfCommon;

namespace TradesAnalyser.Models
{
    public class ApplicationModel: BaseModel
    {
        private static string defaultTitle = "Trades Analyser";

        #region === Fields ====================================

        private readonly TradesProvider  _tradesProvider;
        private readonly TradesProcessor _tradesProcessor;

        #endregion

        #region === Properties ================================

        #region - Params

        private decimal _depo;
        public decimal Depo {
            get => _depo;
            set
            {
                _depo = value; 
                OnPropertyChanged(nameof(Depo));
            }
        }

        private DateTime _dateFrom = new DateTime(2010, 1, 1);
        public DateTime DateFrom
        {
            get => _dateFrom;
            set
            {
                _dateFrom = value;
                OnPropertyChanged(nameof(DateFrom));
            }
        }

        private DateTime _dateTo = DateTime.Now;
        public DateTime DateTo
        {
            get => _dateTo;
            set
            {
                _dateTo = value;
                OnPropertyChanged(nameof(DateTo));
            }
        }

        public ICommand CmdFileOpen
        {
            get => cmdFileOpen;
        }

        public ICommand CmdReset
        {
            get => cmdReset;
        }

        #endregion
        #region - UI elements

        private string _windowTitle;
        public string WindowTitle { 
            get => _windowTitle; 
            set { _windowTitle = value; OnPropertyChanged(nameof(WindowTitle)); }
        }
        
        private string _pnlStatus;
        public string PnlStatus { 
            get => _pnlStatus; 
            set { _pnlStatus = value; OnPropertyChanged(nameof(PnlStatus)); } 
        }
        private string _pnlUpdStatus;
        public string PnlUpdStatus { 
            get => _pnlUpdStatus;
            set { _pnlUpdStatus = value; OnPropertyChanged(nameof(PnlUpdStatus)); }
        }
        private string _pnlFltStatus;
        public string PnlFltStatus { 
            get => _pnlFltStatus;
            set { _pnlFltStatus = value; OnPropertyChanged(nameof(PnlFltStatus)); }
        }
        private string _pnlFltUpdStatus;
        public string PnlFltUpdStatus {
            get => _pnlFltUpdStatus;
            set { _pnlFltUpdStatus = value; OnPropertyChanged(nameof(PnlFltUpdStatus)); }
        }

        private int _totalTradesStatus;
        public int TotalTradesStatus
        {
            get => _totalTradesStatus;
            set
            {
                _totalTradesStatus = value;
                OnPropertyChanged(nameof(TotalTradesStatus));
            }
        }
        private int _filteredTradesStatus;
        public int FilteredTradesStatus
        {
            get => _filteredTradesStatus;
            set
            {
                _filteredTradesStatus = value;
                OnPropertyChanged(nameof(FilteredTradesStatus));
            }
        }

        #endregion
        #region - Data

        private readonly TradeHourContainer _hourlyTradesContainer;
        public List<List<TradeHour>> TradeWeek { get => _hourlyTradesContainer.Get(); }

        //public List<ResultData> CalculatedData { get; set; }

        #endregion

        #endregion

        #region === Commands ==================================

        private CommandDelegate cmdFileOpen;
        private CommandDelegate cmdReset;

        #endregion

        #region === Constructors & Initializers =======

        public ApplicationModel()
        {
            // configure commands
            cmdFileOpen = new CommandDelegate(FileOpenButtonClickHandler);
            cmdReset = new CommandDelegate(ResetButtonClickHandler);

            // initialize fields
            _tradesProvider        = new TradesProvider();
            _tradesProcessor       = new TradesProcessor();
            _hourlyTradesContainer = new TradeHourContainer(Calculate);

            // initialize properties
            Reset();
            //WindowTitle = defaultTitle;
            //PnlStatus = "PNL: N/A";
            //PnlUpdStatus = "PNL UPD: N/A";
            //PnlFltStatus = "PNL FILTERED: N/A";
            //PnlFltUpdStatus = "PNL FILTERED UPD: N/A";
        }


        #endregion

        #region === Handlers ==================================

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
                    WindowTitle = $"{defaultTitle}: {ofd.FileName}";
                    _tradesProvider.Load(ofd.FileName);
                    Calculate();
                }                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message); 
            }
        }
        private void ResetButtonClickHandler(object param)
        {
            Reset();
            //MessageBox.Show("RESET");
        }

        #endregion

        #region === Methods ===================================

        private void Reset()
        {
            _hourlyTradesContainer.Reset(true);
            _tradesProvider.Reset();
            UpdateUi();
            WindowTitle = defaultTitle;
        }
        private void Calculate()
        {
            _tradesProcessor.ProcessPnl(_tradesProvider.Trades, 3);
            ProcessTrades();
            UpdateUi();
        }
        private void ProcessTrades()
        {
            if (_tradesProvider.Trades == null || _tradesProvider.Trades.Count == 0)
                return;
            ProcessDailyTrades();
        }
        private void ProcessDailyTrades()
        {
            _tradesProcessor.ProcessDailyTrades(
                DateFrom,
                DateTo,
                _tradesProvider.Trades,
                _hourlyTradesContainer
            );
        }
        private void UpdateUi()
        {
            var pnl1 = _tradesProcessor.Pnl(SourceType.ORIGINAL, _tradesProvider.Trades);
            if (pnl1 > 0)
                PnlStatus = $"PNL: {pnl1:N0}";
            else
                PnlStatus = "PNL: 0";

            var pnl2 = _tradesProcessor.Pnl(SourceType.UPDATED, _tradesProvider.Trades);
            if (pnl2 > 0)
                PnlUpdStatus = $"PNL UPD: {pnl2:N0}";
            else
                PnlUpdStatus = "PNL UPD: 0";

            var pnl3 = _tradesProcessor.Pnl(SourceType.ORIGINAL, _tradesProvider.Trades, _hourlyTradesContainer);
            if (pnl3 > 0)
                PnlFltStatus = $"PNL FLT: {pnl3:N0}";
            else
                PnlFltStatus = "PNL FLT: 0";

            var pnl4 = _tradesProcessor.Pnl(SourceType.UPDATED, _tradesProvider.Trades, _hourlyTradesContainer);
            if (pnl4 > 0)
                PnlFltUpdStatus = $"PNL FLT UPD: {pnl4:N0}";
            else
                PnlFltUpdStatus = $"PNL FLT UPD: 0";

            TotalTradesStatus = _tradesProvider.Trades.Count;
            FilteredTradesStatus = _tradesProvider.Trades.Where(t => _hourlyTradesContainer.Accepts(t)).ToList().Count;
        }
        #endregion
    }
}
