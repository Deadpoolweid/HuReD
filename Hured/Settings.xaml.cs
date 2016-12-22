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
using System.Windows.Shapes;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void bUnits_Click(object sender, RoutedEventArgs e)
        {
            this.IsManipulationEnabled = false;
            Units w = new Units();
            w.ShowDialog();
            IsManipulationEnabled = true;
        }

        private void bPositions_Click(object sender, RoutedEventArgs e)
        {
            this.IsManipulationEnabled = false;
            Positions w = new Positions();
            w.ShowDialog();
            IsManipulationEnabled = true;
        }

        private void bStatuses_Click(object sender, RoutedEventArgs e)
        {
            this.IsManipulationEnabled = false;
            Statuses w = new Statuses();
            w.ShowDialog();
            IsManipulationEnabled = true;
        }

        private void bPrintSettings_Click(object sender, RoutedEventArgs e)
        {
            // TODO Окно настройки печати
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            // TODO Добавить логику сохранения настроек при закрытии
            Close();
        }
    }
}
