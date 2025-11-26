using MvvmTest.Models;
using System.Windows;

namespace MvvmTest
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ExampleModel();
        }
    }
}
