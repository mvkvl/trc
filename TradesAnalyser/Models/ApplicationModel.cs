using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TradesAnalyser.Charts;
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
        private readonly StatsCalculator _statsCalculator;

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
        //public ICommand CmdSelectedDateChanged
        //{
        //    get => cmdSelectedDateChanged;
        //}

        #endregion
        #region - UI elements

        private string _windowTitle;
        public string WindowTitle { 
            get => _windowTitle; 
            set { _windowTitle = value; OnPropertyChanged(nameof(WindowTitle)); }
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


        private string _calcStatus;
        public string CalcStatus
        {
            get => _calcStatus;
            set
            {
                _calcStatus = value;
                OnPropertyChanged(nameof(CalcStatus));
            }
        }
        
        #endregion
        #region - Data

        private readonly TradeHourContainer _hourlyTradesContainer;
        public List<List<TradeHour>> TradeWeek { get => _hourlyTradesContainer.Get(); }

        public ObservableCollection<StatsRecord> CalculatedData { get; set; } = new ObservableCollection<StatsRecord>();

        #endregion
        #region - Charts

        public Chart chart1;
        public Chart chart2;
        public Chart chart3;
        public Chart chart4;

        #endregion

        #endregion

        #region === Commands ==================================

        private CommandDelegate cmdFileOpen;
        private CommandDelegate cmdReset;
        //private CommandDelegate cmdSelectedDateChanged;

        #endregion

        #region === Constructors & Initializers =======

        public ApplicationModel()
        {
            // configure commands
            cmdFileOpen = new CommandDelegate(FileOpenButtonClickHandler);
            cmdReset = new CommandDelegate(ResetButtonClickHandler);
            //cmdSelectedDateChanged = new CommandDelegate(DateChangeHandler);

            // initialize fields
            _statsCalculator       = new StatsCalculator();
            _tradesProvider        = new TradesProvider();
            _tradesProcessor       = new TradesProcessor();
            _hourlyTradesContainer = new TradeHourContainer(Calculate);

            // initialize properties
            Reset();
        }


        #endregion

        #region === Handlers ==================================

        private void FileOpenButtonClickHandler(object param)
        {
            Task.Run(async () =>
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
            });
        }


        private void ResetButtonClickHandler(object param)
        {
            Reset();
            //MessageBox.Show("RESET");
        }
        //private void DateChangeHandler(object sender, SelectionChangedEventArgs e)
        //{
        //    Calculate();
        //}
        //private void myDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    // Get the DatePicker control that raised the event
        //    DatePicker datePicker = sender as DatePicker;

        //    // Get the newly selected date
        //    DateTime? newSelectedDate = datePicker.SelectedDate;

        //    // Perform your desired actions based on the new date
        //    if (newSelectedDate.HasValue)
        //    {
        //        // Example: Display the selected date
        //        MessageBox.Show($"Selected date: {newSelectedDate.Value.ToShortDateString()}");
        //    }
        //    else
        //    {
        //        // Handle the case where no date is selected (e.g., cleared)
        //        MessageBox.Show("No date selected.");
        //    }
        //}

        #endregion

        #region === Methods ===================================

        private void Reset()
        {
            _hourlyTradesContainer.Reset(true);
            _tradesProvider.Reset();
            _statsCalculator.Reset();
            CalculatedData.Clear();
            _statsCalculator.Data.ForEach(CalculatedData.Add);
            UpdateUi();
            chart1?.Clear();
            chart2?.Clear();
            chart3?.Clear();
            chart4?.Clear();
            WindowTitle = defaultTitle;
            CalcStatus = "";
        }
        internal void Calculate()
        {
            //CalcStatus = "Вычисляю";
            Task.Run(async () =>
            {
                _tradesProcessor.ProcessPnl(_tradesProvider.Trades, 2, 10);
                ProcessTrades();
                _statsCalculator.Calculate(DateFrom, DateTo, _tradesProvider.Trades, _hourlyTradesContainer);
                UpdateUi();
                //CalcStatus = "Готово";
            });
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
                _tradesProvider.Max,
                _hourlyTradesContainer
            );
        }
        private async void UpdateUi()
        {

            TotalTradesStatus = _tradesProvider.
                Trades.
                Where(t => t.Time >= DateFrom && t.Time <= DateTo).
                ToList().
                Count;
            FilteredTradesStatus = _tradesProvider.
                Trades.
                Where(t => t.Time >= DateFrom && t.Time <= DateTo).
                Where(t => _hourlyTradesContainer.Accepts(t)).
                ToList().
                Count;

            UpdateCharts();

        }

        private void UpdateCharts()
        {
            if (_tradesProvider.Trades == null || _tradesProvider.Trades.Count == 0)
            {
                return;
            }
            Task.Run(async () =>
            {
                chart1?.Draw(
                    _statsCalculator.Equity(SourceType.ORIGINAL, _tradesProvider.Trades)
                );

                chart2?.Draw(
                    _statsCalculator.Equity(SourceType.ORIGINAL, _tradesProvider.Trades),
                    _statsCalculator.Equity(SourceType.UPDATED, _tradesProvider.Trades)
                );

                chart3?.Draw(
                    _statsCalculator.Equity(SourceType.ORIGINAL, _tradesProvider.Trades),
                    _statsCalculator.Equity(DateFrom, DateTo, SourceType.ORIGINAL, _tradesProvider.Trades, _hourlyTradesContainer)
                );
                chart4?.Draw(
                    _statsCalculator.Equity(SourceType.UPDATED, _tradesProvider.Trades),
                    _statsCalculator.Equity(DateFrom, DateTo, SourceType.UPDATED, _tradesProvider.Trades, _hourlyTradesContainer)
                );
            });
        }

        #endregion
    }
}
