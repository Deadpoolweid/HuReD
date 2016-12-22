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
    /// Логика взаимодействия для Position.xaml
    /// </summary>
    public partial class Position : Window
    {
        public Position()
        {
            InitializeComponent();
            // TODO Заполнение списка подразделений
        }

        private void bOk_Click(object sender, RoutedEventArgs e)
        {
            // TODO Реализовать добавление должности
            DialogResult = true;
            Close();
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
