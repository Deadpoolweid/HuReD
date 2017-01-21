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
using Hured.Tables_templates;
using MahApps.Metro.Controls;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для BusinessTrip.xaml
    /// </summary>
    public partial class BusinessTrip : MetroWindow
    {
        public BusinessTrip(ПриказКомандировка order = null)
        {
            InitializeComponent();

            if (order != null)
            {
                tbМесто.Text = order.Место;
                dpBegin.Text = order.НачалоКомандировки.ToString();
                dpEnd.Text = order.КонецКомандировки.ToString();
                tbЦель.Text = order.Цель;
                tbЗаСчёт.Text = order.ЗаСчёт;
                tbОснование.Text = order.Основание;
            }
        }

        private void bPrint_Click(object sender, RoutedEventArgs e)
        {
            // TODO Перенести функцию

        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void bOk_Click(object sender, RoutedEventArgs e)
        {

            var order = new ПриказКомандировка()
            {
                Место = tbМесто.Text,
                НачалоКомандировки = dpBegin.DisplayDate,
                КонецКомандировки = dpEnd.DisplayDate,
                Цель = tbЦель.Text,
                ЗаСчёт = tbЗаСчёт.Text,
                Основание = tbОснование.Text
            };
            Tag = order;

            DialogResult = true;
            Close();
        }
    }
}
