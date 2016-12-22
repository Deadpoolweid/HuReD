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
    /// Логика взаимодействия для BusinessTrip.xaml
    /// </summary>
    public partial class BusinessTrip : Window
    {
        public BusinessTrip()
        {
            InitializeComponent();
        }

        private void bPrint_Click(object sender, RoutedEventArgs e)
        {
            // TODO Реализация функции
            Functions.Print();
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void bOk_Click(object sender, RoutedEventArgs e)
        {
            // TODO Добваить логику при сохранении
            DialogResult = true;
            Close();
        }
    }
}
