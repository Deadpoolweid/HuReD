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
using Hured.DBModel;
using Hured.Tables_templates;
using MahApps.Metro.Controls;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Dismissal.xaml
    /// </summary>
    public partial class Dismissal : MetroWindow
    {
        public Dismissal(int employeeId = 0, ПриказУвольнение order = null)
        {
            InitializeComponent();

            Controller.OpenConnection();

            var employee = Controller.Find(new Сотрудник(),
                e => e.СотрудникId == employeeId);

            lEmployee.Content = employee.ОсновнаяИнформация.Фамилия + " " +
                                employee.ОсновнаяИнформация.Имя + " " +
                                employee.ОсновнаяИнформация.Отчество;

            Controller.CloseConnection();

            if (order != null)
            {
                tbНомерДоговора.Text = order.НомерТрудовогоДоговора;
                dpДатаДоговора.Text = order.ДатаТрудовогоДоговора.ToString();
                tbОснование.Text = order.Основание;
                Functions.SetRTBText(rtbПримечание, order.Примечание);
                filePath = order.Файл;
            }
        }

        private string filePath;

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void bOk_Click(object sender, RoutedEventArgs e)
        {
            Controller.OpenConnection();

            var order = new ПриказУвольнение()
            {
                НомерТрудовогоДоговора = tbНомерДоговора.Text,
                ДатаТрудовогоДоговора = dpДатаДоговора.DisplayDate,
                Основание = tbОснование.Text,
                Примечание = Functions.GetRTBText(rtbПримечание),
                Файл = filePath
            };
            Tag = order;

            DialogResult = true;
            Close();
        }

    }
}
