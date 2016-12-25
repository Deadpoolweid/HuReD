using System;
using System.Collections.Generic;
using System.Data;
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

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // TODO Инициализация приложения, первичная настройка
        }

        private void BEmployees_OnClick(object sender, RoutedEventArgs e)
        {
            this.IsManipulationEnabled = false;
            Employees w = new Employees();
            w.ShowDialog();
            IsManipulationEnabled = true;
        }

        private void bOrders_Click(object sender, RoutedEventArgs e)
        {
            this.IsManipulationEnabled = false;
            Orders w = new Orders();
            w.ShowDialog();
            IsManipulationEnabled = true;
        }

        private void bTimesheet_Click(object sender, RoutedEventArgs e)
        {
            this.IsManipulationEnabled = false;
            Timesheet w = new Timesheet();
            w.ShowDialog();
            IsManipulationEnabled = true;
        }

        private void bSettings_Click(object sender, RoutedEventArgs e)
        {
            this.IsManipulationEnabled = false;
            Settings w = new Settings();
            w.ShowDialog();
            IsManipulationEnabled = true;
        }
    }
}
