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
using MahApps.Metro.Controls;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Timesheet.xaml
    /// </summary>
    public partial class Timesheet : MetroWindow
    {
        public Timesheet()
        {
            InitializeComponent();
            // TODO Заполнение списка должностей
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
