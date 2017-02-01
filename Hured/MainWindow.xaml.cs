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
using Hured.DBModel;
using MahApps.Metro.Controls;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            // TODO Инициализация приложения, первичная настройка
            if (Properties.Settings.Default.IsFirstLaunch)
            {
                Wizard w = new Wizard();
                w.ShowDialog();

                Controller.InitDB();
                Properties.Settings.Default.IsFirstLaunch = false;
            }

        }

        public void BEmployees_OnClick(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;
            Employees w = new Employees();
            w.ShowDialog();
            IsHitTestVisible = true;
        }

        public void bOrders_Click(object sender, RoutedEventArgs e)
        {
            this.IsHitTestVisible = false;
            Orders w = new Orders();
            w.ShowDialog();
            IsHitTestVisible = true;
        }

        public void bTimesheet_Click(object sender, RoutedEventArgs e)
        {
            this.IsHitTestVisible = false;
            Timesheet w = new Timesheet();
            w.ShowDialog();
            IsHitTestVisible = true;
        }

        private void bSettings_Click(object sender, RoutedEventArgs e)
        {
            this.IsHitTestVisible = false;
            Settings w = new Settings();
            w.ShowDialog();
            IsHitTestVisible = true;
        }
    }
}
