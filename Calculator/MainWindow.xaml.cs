using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculator
{
    public enum Strategy
    {
        FIXED,
        CPT,
        UG,
        DG
    }


    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Calc _calc;
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            _methodSelector.ItemsSource = new List<Strategy>() { Strategy.FIXED, Strategy.CPT, Strategy.UG, Strategy.DG };
            _methodSelector.SelectedIndex = 0;
            _methodSelector.SelectionChanged += MethodSelectorSelectionChanged;

            _depo.Text = "100000";
            _startLot.Text = "10";
            _take.Text = "300";
            _stop.Text = "100";
            _fee.Text = "7";
            _deals.Text = "1000";
            _profit.Text = "30";
            _go.Text = "5000";
            _minDepoPrc.Text = "50";

            _calc = new Calc();
        }

        private void MethodSelectorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            MessageBox.Show(cb.SelectedItem.ToString());
        }

        private void CalculateButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                CalcConfig cc = new CalcConfig()
                    .WithDepo(_depo.Text)
                    .WithLot(_startLot.Text)
                    .WithTake(_take.Text)
                    .WithStop(_stop.Text)
                    .WithFee(_fee.Text)
                    .WithDealsAmount(_deals.Text)
                    .WithProfit(_profit.Text)
                    .WithGo(_go.Text)
                    .WithMinDepoPrc(_minDepoPrc.Text);
                _dataGrid.ItemsSource = _calc.Calculate(cc);
                var v = this.TryFindResource("DataGridCellRighAligned");
                if (v != null)
                {
                    _dataGrid.Columns[1].CellStyle = (Style)v;
                    _dataGrid.Columns[2].CellStyle = (Style)v;
                    _dataGrid.Columns[3].CellStyle = (Style)v;
                    _dataGrid.Columns[5].CellStyle = (Style)v;
                }
                v = this.TryFindResource("DataGridCellCenterAligned");
                if (v != null)
                {
                    _dataGrid.Columns[4].CellStyle = (Style)v;
                    _dataGrid.Columns[6].CellStyle = (Style)v;
                }

                _dataGrid.Columns[0].Header = "Стратегия";
                _dataGrid.Columns[1].Header = "Депозит";
                _dataGrid.Columns[2].Header = "Результат";
                _dataGrid.Columns[3].Header = "П/У";
                _dataGrid.Columns[4].Header = "П/У %";
                _dataGrid.Columns[5].Header = "Макс. просадка";
                _dataGrid.Columns[6].Header = "Макс. просадка %";


                _dataGrid.Columns[1].Width = 100;
                _dataGrid.Columns[2].Width = 100;
                _dataGrid.Columns[3].Width = 100;
                _dataGrid.Columns[4].Width = 100;

            }
            catch (Exception err)
            {
                MessageBox.Show($"{err}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
