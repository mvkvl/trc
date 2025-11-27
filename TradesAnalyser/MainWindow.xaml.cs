using System.Windows;
using TradesAnalyser.Models;

namespace TradesAnalyser
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ApplicationModel(_resultsDataGrid);
        }

    }
}
