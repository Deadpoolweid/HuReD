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
    public class OrderInfo
    {
        public OrderInfo(string номер, string фио, string тип, string дата, string файл)
        {
            Номер = номер;
            Дата = дата;
            Тип = тип;
            ФИО = фио;
            Файл = файл;
        }

        public string Номер { get; set; }

        public string Дата { get; set; }

        public string ФИО { get; set; }

        public string Тип { get; set; }

        public string Файл { get; set; }
    }

    /// <summary>
    /// Логика взаимодействия для Orders.xaml
    /// </summary>
    public partial class Orders : MetroWindow
    {
        public Orders()
        {
            InitializeComponent();

            SyncOrders();
        }

        void SyncOrders()
        {
            lvOrders.Items.Clear();

            Controller.OpenConnection();

            var приказыПриём = Controller.Select(new ПриказПриём(), e => e != null);
            foreach (var e in приказыПриём)
            {
                lvOrders.Items.Add(new OrderInfo(e.Номер,
                    e.Сотрудник.ОсновнаяИнформация.Фамилия + " " +
                    e.Сотрудник.ОсновнаяИнформация.Имя + " " +
                    e.Сотрудник.ОсновнаяИнформация.Отчество, "Приём", e.Дата.ToShortDateString(),e.Файл));
            }

            var приказыУвольнение = Controller.Select(new ПриказУвольнение(), e => e != null);
            foreach (var e in приказыУвольнение)
            {
                lvOrders.Items.Add(new OrderInfo(e.Номер,
                    e.Сотрудник.ОсновнаяИнформация.Фамилия + " " +
                    e.Сотрудник.ОсновнаяИнформация.Имя + " " +
                    e.Сотрудник.ОсновнаяИнформация.Отчество, "Увольнение", e.Дата.ToShortDateString(), e.Файл));
            }

            var приказыОтпуск = Controller.Select(new ПриказОтпуск(), e => e != null);
            foreach (var e in приказыОтпуск)
            {
                lvOrders.Items.Add(new OrderInfo(e.Номер,
                    e.Сотрудник.ОсновнаяИнформация.Фамилия + " " +
                    e.Сотрудник.ОсновнаяИнформация.Имя + " " +
                    e.Сотрудник.ОсновнаяИнформация.Отчество, "Отпуск", e.Дата.ToShortDateString(), e.Файл));
            }

            var приказыКомандировка = Controller.Select(new ПриказКомандировка(), e => e != null);
            foreach (var e in приказыКомандировка)
            {
                lvOrders.Items.Add(new OrderInfo(e.Номер,
                    e.Сотрудник.ОсновнаяИнформация.Фамилия + " " +
                    e.Сотрудник.ОсновнаяИнформация.Имя + " " +
                    e.Сотрудник.ОсновнаяИнформация.Отчество, "Командировка", e.Дата.ToShortDateString(), e.Файл));
            }

            Controller.CloseConnection();
        }

        // TODO Номер приказа должен быть уникальным

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            Order w = new Order();
            w.ShowDialog();

            IsEnabled = true;

            SyncOrders();
        }

        private void bChange_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;


            var item = lvOrders.SelectedItem as OrderInfo;
            if (item != null)
            {
                Order w = new Order(item);
                w.ShowDialog();
            }

            IsEnabled = true;

            SyncOrders();
        }

        private void BOpen_OnClick(object sender, RoutedEventArgs e)
        {
            var item = lvOrders.SelectedItem as OrderInfo;
            if (item != null)
            {
                WordDocument document = new WordDocument(item.Файл);
                document.OpenWithWord();
            }
        }


        private void BPrint_OnClick(object sender, RoutedEventArgs e)
        {
            var item = lvOrders.SelectedItem as OrderInfo;
            if (item != null)
            {
                WordDocument document = new WordDocument(item.Файл);
                document.Open();
                document.Print();
                document.Close();
            }
        }

        private void bRemove_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;


            var item = lvOrders.SelectedItem as OrderInfo;
            if (item != null)
            {
                if (item.Тип == "Приём")
                {
                    Controller.OpenConnection();
                    Controller.Remove(new ПриказПриём(), 
                        q => q.Номер == item.Номер);
                    Controller.CloseConnection();
                }
                else if (item.Тип == "Увольнение")
                {
                    Controller.OpenConnection();
                    Controller.Remove(new ПриказУвольнение(), 
                        q => q.Номер == item.Номер);
                    Controller.CloseConnection();
                }
                else if (item.Тип == "Отпуск")
                {
                    Controller.OpenConnection();
                    Controller.Remove(new ПриказОтпуск(), 
                        q => q.Номер == item.Номер);
                    Controller.CloseConnection();
                }
                else if (item.Тип == "Командировка")
                {
                    Controller.OpenConnection();
                    Controller.Remove(new ПриказКомандировка(), 
                        q => q.Номер == item.Номер);
                    Controller.CloseConnection();
                }
            }

            IsEnabled = true;

            SyncOrders();
        }

        private void bOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
