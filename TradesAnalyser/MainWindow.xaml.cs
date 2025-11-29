using System.Windows;
using System.Windows.Controls;
using TradesAnalyser.Charts;
using TradesAnalyser.Models;

namespace TradesAnalyser
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ApplicationModel app;

        public MainWindow()
        {
            InitializeComponent();
            app = new ApplicationModel
            {
                chart1 = new Chart(),
                chart2 = new Chart(),
                chart3 = new Chart(),
                chart4 = new Chart(),
            };
            DataContext = app;
            Chart1.DataContext = app.chart1;
            Chart2.DataContext = app.chart2;
            Chart3.DataContext = app.chart3;
            Chart4.DataContext = app.chart4;
        }
        private void DateChangeHandler(object sender, SelectionChangedEventArgs e)
        {
            app.Calculate();
        }

    }
}
