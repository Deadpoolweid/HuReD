using System;
using System.Collections.Generic;
using System.IO;
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
using Hured.DBModel;
using Hured.Tables_templates;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using Path = System.Windows.Shapes.Path;

namespace Hured
{
    public class OrderInfo
    {
        public OrderInfo(string номер, string фио, string тип, string дата)
        {
            Номер = номер;
            Дата = дата;
            Тип = тип;
            ФИО = фио;
        }

        public string Номер { get; set; }

        public string Дата { get; set; }

        public string ФИО { get; set; }

        public string Тип { get; set; }

        public int ID { get; set; }

        public OrderType OrderType { get; set; }

        public Type Type { get; set; }
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
                    e.Сотрудник.ОсновнаяИнформация.Отчество, "Приём", e.Дата.ToShortDateString())
                {
                    ID = e.ПриказПриёмId,
                    OrderType = OrderType.Recruitment,
                    Type = e.GetType()
                });
            }

            var приказыУвольнение = Controller.Select(new ПриказУвольнение(), e => e != null);
            foreach (var e in приказыУвольнение)
            {
                lvOrders.Items.Add(new OrderInfo(e.Номер,
                    e.Сотрудник.ОсновнаяИнформация.Фамилия + " " +
                    e.Сотрудник.ОсновнаяИнформация.Имя + " " +
                    e.Сотрудник.ОсновнаяИнформация.Отчество, "Увольнение", e.Дата.ToShortDateString())
                {
                    ID = e.ПриказУвольнениеId,
                    OrderType = OrderType.Dismissal,
                    Type = e.GetType()
                });
            }

            var приказыОтпуск = Controller.Select(new ПриказОтпуск(), e => e != null);
            foreach (var e in приказыОтпуск)
            {
                lvOrders.Items.Add(new OrderInfo(e.Номер,
                    e.Сотрудник.ОсновнаяИнформация.Фамилия + " " +
                    e.Сотрудник.ОсновнаяИнформация.Имя + " " +
                    e.Сотрудник.ОсновнаяИнформация.Отчество, "Отпуск", e.Дата.ToShortDateString())
                {
                    ID = e.ПриказОтпускId,
                    OrderType = OrderType.Vacation,
                    Type = e.GetType()
                });
            }

            var приказыКомандировка = Controller.Select(new ПриказКомандировка(), e => e != null);
            foreach (var e in приказыКомандировка)
            {
                lvOrders.Items.Add(new OrderInfo(e.Номер,
                    e.Сотрудник.ОсновнаяИнформация.Фамилия + " " +
                    e.Сотрудник.ОсновнаяИнформация.Имя + " " +
                    e.Сотрудник.ОсновнаяИнформация.Отчество, "Командировка", e.Дата.ToShortDateString())
                {
                    ID = e.ПриказКомандировкаId,
                    OrderType = OrderType.BusinessTrip,
                    Type = e.GetType()
                });
            }

            Controller.CloseConnection();
        }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;

            Order w = new Order();
            w.ShowDialog();

            IsHitTestVisible = true;

            SyncOrders();
        }

        private void bChange_Click(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;


            var item = lvOrders.SelectedItem as OrderInfo;
            if (item != null)
            {
                Order w = new Order(item);
                w.ShowDialog();
            }

            IsHitTestVisible = true;

            SyncOrders();
        }

        private void BOpen_OnClick(object sender, RoutedEventArgs e)
        {
            var item = lvOrders.SelectedItem as OrderInfo;
            Controller.OpenConnection();
            if (item != null)
            {
                WordDocument document;
                switch (item.OrderType)
                {
                    case OrderType.Recruitment:
                        document = Functions.CreateOrder(item.OrderType, Controller.Find(new ПриказПриём(),
                            q => q.ПриказПриёмId == item.ID));
                        break;
                    case OrderType.Dismissal:
                        document = Functions.CreateOrder(item.OrderType, Controller.Find(new ПриказУвольнение(),
                            q => q.ПриказУвольнениеId == item.ID));
                        break;
                    case OrderType.Vacation:
                        document = Functions.CreateOrder(item.OrderType, Controller.Find(new ПриказОтпуск(),
                            q => q.ПриказОтпускId == item.ID));
                        break;
                    case OrderType.BusinessTrip:
                        document = Functions.CreateOrder(item.OrderType, Controller.Find(new ПриказКомандировка(),
                            q => q.ПриказКомандировкаId == item.ID));
                        break;
                    default:
                        document = null;
                        break;
                }
                string savePath = Directory.GetCurrentDirectory() + @"\Temp.docx";
                document.Save(savePath);
                document.Path = savePath;
                document.Close();
                document.OpenWithWord();
                Controller.CloseConnection();
            }
        }

        private void BSave_OnClick(object sender, RoutedEventArgs e)
        {
            var item = lvOrders.SelectedItem as OrderInfo;
            if (item == null) return;
            var sfd = new SaveFileDialog()
            {
                InitialDirectory = Directory.GetCurrentDirectory(),
                    //initialDirectory == null
                    //    ? Directory.GetCurrentDirectory() + @"\Documents"
                    //    : System.IO.Path.GetDirectoryName(initialDirectory),
                Filter = "Word Document | *.docx | Все файлы (*.*)|*.*",
                FileName = "Новый приказ"
            };

            sfd.ShowDialog();


            WordDocument document;
            Controller.OpenConnection();
            switch (item.OrderType)
            {
                case OrderType.Recruitment:
                    document = Functions.CreateOrder(item.OrderType, Controller.Find(new ПриказПриём(),
                        q => q.ПриказПриёмId == item.ID));
                    break;
                case OrderType.Dismissal:
                    document = Functions.CreateOrder(item.OrderType, Controller.Find(new ПриказУвольнение(), 
                        q => q.ПриказУвольнениеId == item.ID));
                    break;
                case OrderType.Vacation:
                    document = Functions.CreateOrder(item.OrderType, Controller.Find(new ПриказОтпуск(), 
                        q => q.ПриказОтпускId == item.ID));
                    break;
                case OrderType.BusinessTrip:
                    document = Functions.CreateOrder(item.OrderType, Controller.Find(new ПриказКомандировка(), 
                        q => q.ПриказКомандировкаId == item.ID));
                    break;
                default:
                    document = null;
                    break;
            }


            document.Save(sfd.FileName,false);

            document.Close();
            Controller.CloseConnection();
        }

        private void BPrint_OnClick(object sender, RoutedEventArgs e)
        {
            var item = lvOrders.SelectedItem as OrderInfo;
            if (item != null)
            {
                WordDocument document;
                Controller.OpenConnection();
                switch (item.OrderType)
                {
                    case OrderType.Recruitment:
                        document = Functions.CreateOrder(item.OrderType, Controller.Find(new ПриказПриём(),
                            q => q.ПриказПриёмId == item.ID));
                        break;
                    case OrderType.Dismissal:
                        document = Functions.CreateOrder(item.OrderType, Controller.Find(new ПриказУвольнение(),
                            q => q.ПриказУвольнениеId == item.ID));
                        break;
                    case OrderType.Vacation:
                        document = Functions.CreateOrder(item.OrderType, Controller.Find(new ПриказОтпуск(),
                            q => q.ПриказОтпускId == item.ID));
                        break;
                    case OrderType.BusinessTrip:
                        document = Functions.CreateOrder(item.OrderType, Controller.Find(new ПриказКомандировка(),
                            q => q.ПриказКомандировкаId == item.ID));
                        break;
                    default:
                        document = null;
                        break;
                }
                string savePath = Directory.GetCurrentDirectory() + @"\Temp.docx";
                document.Save(savePath,false);
                document.Path = savePath;
                document.Print();
                document.Close();
                Controller.CloseConnection();
            }
        }

        private void bRemove_Click(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;


            var item = lvOrders.SelectedItem as OrderInfo;
            if (item != null)
            {
                if (item.Тип == "Приём")
                {
                    Controller.OpenConnection();
                    Controller.Remove(new ПриказПриём(), q => q.Номер == item.Номер);
                    Controller.CloseConnection();
                }
                else if (item.Тип == "Увольнение")
                {
                    Controller.OpenConnection();
                    Controller.Remove(new ПриказУвольнение(), q => q.Номер == item.Номер);
                    Controller.CloseConnection();
                }
                else if (item.Тип == "Отпуск")
                {
                    Controller.OpenConnection();
                    Controller.Remove(new ПриказОтпуск(), q => q.Номер == item.Номер);
                    Controller.CloseConnection();
                }
                else if (item.Тип == "Командировка")
                {
                    Controller.OpenConnection();
                    Controller.Remove(new ПриказКомандировка(), q => q.Номер == item.Номер);
                    Controller.CloseConnection();
                }
            }

            IsHitTestVisible = true;

            SyncOrders();
        }

        private void bOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
