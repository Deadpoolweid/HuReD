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
    /// Логика взаимодействия для BusinessTrip.xaml
    /// </summary>
    public partial class BusinessTrip
    {
        public BusinessTrip(ПриказКомандировка order = null)
        {
            InitializeComponent();

            if (order != null)
            {
                TbМесто.Text = order.Место;
                DpBegin.Text = order.НачалоКомандировки.ToShortDateString();
                DpEnd.Text = order.КонецКомандировки.ToShortDateString();
                TbЦель.Text = order.Цель;
                TbЗаСчёт.Text = order.ЗаСчёт;
                TbОснование.Text = order.Основание;
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

            var order = new ПриказКомандировка
            {
                Место = TbМесто.Text,
                НачалоКомандировки = DateTime.Parse(DpBegin.Text),
                КонецКомандировки = DateTime.Parse(DpEnd.Text),
                Цель = TbЦель.Text,
                ЗаСчёт = TbЗаСчёт.Text,
                Основание = TbОснование.Text
            };
            Tag = order;

            DialogResult = true;
            Close();
        }
    }
}
