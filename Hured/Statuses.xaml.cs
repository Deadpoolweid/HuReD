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
    /// Логика взаимодействия для Statuses.xaml
    /// </summary>
    public partial class Statuses : Window
    {
        public Statuses()
        {
            InitializeComponent();
        }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            // TODO Вызвать окно добавления статуса
        }

        private void bChange_Click(object sender, RoutedEventArgs e)
        {
            // TODO Вызвать окно изменения статуса
        }

        private void bRemove_Click(object sender, RoutedEventArgs e)
        {
            // TODO Удаление статуса
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
