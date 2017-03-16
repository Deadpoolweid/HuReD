using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Hured.DataBase;
using Hured.Tables_templates;
using MahApps.Metro.Controls;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Dismissal.xaml
    /// </summary>
    public partial class Dismissal
    {
        public Dismissal(int employeeId = 0, ПриказУвольнение order = null)
        {
            InitializeComponent();

            Controller.OpenConnection();

            var employee = Controller.Find<Сотрудник>(e => e.СотрудникId == employeeId);

            LEmployee.Content = employee.ОсновнаяИнформация.Фамилия + " " +
                                employee.ОсновнаяИнформация.Имя + " " +
                                employee.ОсновнаяИнформация.Отчество;

            Controller.CloseConnection();

            if (order != null)
            {
                TbНомерДоговора.Text = order.НомерТрудовогоДоговора;
                DpДатаДоговора.Text = order.ДатаТрудовогоДоговора.ToShortDateString();
                TbОснование.Text = order.Основание;
                DpДатаУвольнения.Text = order.ДатаУвольнения.ToShortDateString();
                TbОснованиеДокумент.Text = order.ОснованиеДокумент;
            }
        }


        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void bOk_Click(object sender, RoutedEventArgs e)
        {
            if (!Functions.ValidateAllTextboxes(this))
            {
                return;
            }

            Controller.OpenConnection();

            var order = new ПриказУвольнение
            {
                НомерТрудовогоДоговора = TbНомерДоговора.Text,
                ДатаТрудовогоДоговора = DateTime.Parse(DpДатаДоговора.Text),
                Основание = TbОснование.Text,
                ДатаУвольнения = DateTime.Parse(DpДатаУвольнения.Text),
                ОснованиеДокумент = TbОснованиеДокумент.Text
            };
            Tag = order;

            DialogResult = true;
            Close();
        }

    }
}
