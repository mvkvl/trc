using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
        private List<CalculatedData> _data;
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

            this.SizeChanged += WindowSizeChanged;

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

        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_data != null)
            {
                Draw(_data[_methodSelector.SelectedIndex]);
            }
        }

        private void MethodSelectorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_data == null)
            {
                return;
            }
            ComboBox cb = sender as ComboBox;
            Draw(_data[cb.SelectedIndex]);
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
                _data = _calc.Calculate(cc);
                _dataGrid.ItemsSource = _data;
                FormatDataGrid();
                Draw(_data[_methodSelector.SelectedIndex]);
            }
            catch (Exception err)
            {
                MessageBox.Show($"{err}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FormatDataGrid()
        {
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
        
        private void DrawLine(double y, string text)
        {
            Line ln = new Line()
            {
                X1 = 0,
                Y1 = y,
                X2 = _canvas.ActualWidth,
                Y2 = y,
                Stroke = Brushes.Gray,
                StrokeThickness = 1,
            };
            _canvas.Children.Add(ln);

            TextBlock tb = new TextBlock
            {
                Text = text,
                FontSize = 12
            };
            Canvas.SetLeft(tb, 5);
            Canvas.SetTop(tb, y - 15);
            _canvas.Children.Add(tb);
        }
        private void Draw(CalculatedData data)
        {
            if (data == null)
            {
                return;
            }
            _canvas.Children.Clear();

            int cnt = data.Equity.Count;
            decimal maxEquity = data.Equity.Max();
            decimal minEquity = data.Equity.Min();
            double sx = _canvas.ActualWidth / cnt;
            double ky = (double)(maxEquity - minEquity) / _canvas.ActualHeight;
            double x = 0, y = 0, y2;

            DrawLine(_canvas.ActualHeight - (double)(data._initialDepo - minEquity) / ky, "initial");
            if (data._profitPercent > 100)
                DrawLine(_canvas.ActualHeight - (double)(data._initialDepo * 2 - minEquity) / ky, "x2");
            if (data._profitPercent > 200)
                DrawLine(_canvas.ActualHeight - (double)(data._initialDepo * 3 - minEquity) / ky, "x3");

            for (int i = 0; i < cnt - 1; i++)
            {
                y  = _canvas.ActualHeight - (double)(data.Equity[i]   - minEquity) / ky;
                //Ellipse el = new Ellipse()
                //{
                //    Width = 2,
                //    Height = 2,
                //    Stroke = Brushes.Teal,
                //    Fill = new SolidColorBrush(Colors.Teal),
                //};
                //Canvas.SetLeft(el, x);
                //Canvas.SetTop(el, y);
                //_canvas.Children.Add(el);

                y2 = _canvas.ActualHeight - (double)(data.Equity[i + 1] - minEquity) / ky;
                Line ln = new Line()
                {
                    X1 = x,
                    Y1 = y,
                    X2 = x + sx,
                    Y2 = y2,
                    Stroke = Brushes.Red,
                    StrokeThickness = 2,
                };
                _canvas.Children.Add(ln);

                x += sx;                
            }
        }
    }
}
