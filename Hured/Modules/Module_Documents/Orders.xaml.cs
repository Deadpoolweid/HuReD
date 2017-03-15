using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Catel.Collections;
using Hured.DBModel;
using Hured.Tables_templates;
using Hured.Tools_and_extensions;
using Microsoft.Win32;

namespace Hured
{
    public class OrderInfo
    {
        public OrderInfo(string номер, string фио, string тип, string дата)
        {
            Номер = номер;
            Дата = дата;
            Тип = тип;
            Фио = фио;
        }

        public string Номер { get; set; }

        public string Дата { get; set; }

        public string Фио { get; set; }

        public string Тип { get; set; }

        public int Id { get; set; }

        public OrderType OrderType { get; set; }

        public Type Type { get; set; }
    }

    /// <summary>
    /// Логика взаимодействия для Orders.xaml
    /// </summary>
    public partial class Orders
    {
        public Orders()
        {
            InitializeComponent();

            bool isFirstElement = true;
            foreach (var document in DocumentsTypeComparer.GetAllDocuments())
            {
                // Получаем название документа (Исключая слово приказ из имени)
                var name = document.Name.Substring(6, document.Name.Length - 6);

                var rb = new RadioButton()
                {
                    Content = name,
                    GroupName = "DocumentTypes",
                    Name = "rb" + DocumentsTypeComparer.GetTypeOfDocument(document.Name)
                };
                rb.Click += SelectedOrderType_OnChanged;

                if (isFirstElement)
                {
                    rb.IsChecked = true;
                    isFirstElement = false;
                }

                spDocumentTypes.Children.Add(rb);

            }

            SyncOrders();

            Functions.AddSortingToListView(LvOrders);


        }

        public List<T> FilterOrdersByTags<T>(string[] tags, List<T> employees) where T:Приказ
        {
            var searchResult = employees.Where(
                q => new Regex(string.Join("|", tags.Select(Regex.Escape)), RegexOptions.IgnoreCase).IsMatch(
                    q.Сотрудник.ОсновнаяИнформация.Имя + q.Сотрудник.ОсновнаяИнформация.Фамилия +
                    q.Сотрудник.ОсновнаяИнформация.Отчество)
                    );
            if (searchResult != null)
            {
                employees = searchResult.ToList();
            }

            return employees;
        }

        public ArrayList FilterOrdersByTagsNotGeneric(string[] tags, ArrayList employees, Type type)
        {
            var result = new ArrayList();

            var regex = new Regex(string.Join("|",tags.Select(Regex.Escape)),RegexOptions.IgnoreCase);

            foreach (var e in employees)
            {
                dynamic element = e as Приказ;
                if (
                    regex.IsMatch(element.Сотрудник.ОсновнаяИнформация.Имя +
                                  element.Сотрудник.ОсновнаяИнформация.Фамилия +
                                  element.Сотрудник.ОсновнаяИнформация.Отчество))
                {
                    result.Add(element);
                }
            }

            return result;
        }

        void SyncOrders()
        {
            LvOrders.Items.Clear();

            Controller.OpenConnection();

            bool needFilter = tbSearch.Text != String.Empty && !tbSearch.IsHavePlaceholder();
            var tags = tbSearch.Text.Split(' ');

            foreach (var child in spDocumentTypes.Children)
            {
                var rb = child as RadioButton;


                var type = DocumentsTypeComparer.GetDocumentType(rb.Name.Substring(2, rb.Name.Length - 2));
                if (rb.IsChecked == true)
                {
                    
                    var selectMethod = typeof(Controller).GetMethod("SelectAll");


                    //selectMethod.MakeGenericMethod(type);

                    var listoftype = typeof(ArrayList);

                    dynamic documents = Activator.CreateInstance(listoftype);

                    object result = selectMethod.Invoke(null, new object[]
                    {
                        type
                    });

                    documents = Convert.ChangeType(result,listoftype);

                    if (needFilter)
                    {
                        var filterOrdersByTags = typeof(Hured.Orders).GetMethod("FilterOrdersByTagsNotGeneric");
                        //filterOrdersByTags.MakeGenericMethod(type);
                        documents = Convert.ChangeType(filterOrdersByTags.Invoke(this,new object[] {tags,documents,type}),listoftype);
                    }

                    foreach (var e in documents)
                    {
                        //dynamic e = Convert.ChangeType(_e, type);
                        var id = type.GetProperty(type.Name + "Id").GetValue(e);

                        LvOrders.Items.Add(new OrderInfo(e.Номер,
    e.Сотрудник.ОсновнаяИнформация.Фамилия + " " +
    e.Сотрудник.ОсновнаяИнформация.Имя + " " +
    e.Сотрудник.ОсновнаяИнформация.Отчество, type.Name.Substring(6,type.Name.Length - 6), e.Дата.ToShortDateString())
                        {
                            Id = id,
                            OrderType = OrderType.Recruitment,
                            Type = e.GetType()
                        });
                    }

                }
            }

            Controller.CloseConnection();
        }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;

            var w = new Order();
            w.ShowDialog();

            IsHitTestVisible = true;

            SyncOrders();
        }

        private void bChange_Click(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;


            var item = LvOrders.SelectedItem as OrderInfo;
            if (item != null)
            {
                var w = new Order(item);
                w.ShowDialog();
            }

            IsHitTestVisible = true;

            SyncOrders();
        }

        private void BOpen_OnClick(object sender, RoutedEventArgs e)
        {
            var item = LvOrders.SelectedItem as OrderInfo;
            if (item != null)
            {
                Controller.OpenConnection();


                WordDocument wordDocument;

                dynamic document = Controller.FindDocumentNotGeneric(item.Id,
                    DocumentsTypeComparer.GetDocumentType(item.OrderType));

                wordDocument = Functions.CreateOrder(item.OrderType, document);

                //switch (item.OrderType)
                //{
                //    case OrderType.Recruitment:
                //        wordDocument = Functions.CreateOrder(item.OrderType, Controller.Find<ПриказПриём>(
                //            q => q.ПриказПриёмId == item.Id));
                //        break;
                //    case OrderType.Dismissal:
                //        wordDocument = Functions.CreateOrder(item.OrderType, Controller.Find<ПриказУвольнение>(
                //            q => q.ПриказУвольнениеId == item.Id));
                //        break;
                //    case OrderType.Vacation:
                //        wordDocument = Functions.CreateOrder(item.OrderType, Controller.Find<ПриказОтпуск>(
                //            q => q.ПриказОтпускId == item.Id));
                //        break;
                //    case OrderType.BusinessTrip:
                //        wordDocument = Functions.CreateOrder(item.OrderType, Controller.Find<ПриказКомандировка>(
                //            q => q.ПриказКомандировкаId == item.Id));
                //        break;
                //    default:
                //        wordDocument = null;
                //        break;
                //}
                var savePath = Directory.GetCurrentDirectory() + @"\Temp.docx";
                if (wordDocument != null)
                {
                    wordDocument.Save(savePath);
                    wordDocument.Path = savePath;
                    wordDocument.Close();
                    wordDocument.OpenWithWord();
                }
                Controller.CloseConnection();
            }
        }

        private void BSave_OnClick(object sender, RoutedEventArgs e)
        {
            var item = LvOrders.SelectedItem as OrderInfo;
            if (item == null) return;
            var sfd = new SaveFileDialog
            {
                InitialDirectory = Directory.GetCurrentDirectory(),
                    //initialDirectory == null
                    //    ? Directory.GetCurrentDirectory() + @"\Documents"
                    //    : System.IO.Path.GetDirectoryName(initialDirectory),
                Filter = "Word Document | *.docx | Все файлы (*.*)|*.*",
                FileName = "Новый приказ"
            };

            sfd.ShowDialog();


            WordDocument wordDocument;

            Controller.OpenConnection();


            dynamic document = Controller.FindDocumentNotGeneric(item.Id,
                DocumentsTypeComparer.GetDocumentType(item.OrderType));

            wordDocument = Functions.CreateOrder(item.OrderType, document);

            //switch (item.OrderType)
            //{
            //    case OrderType.Recruitment:
            //        wordDocument = Functions.CreateOrder(item.OrderType, Controller.Find<ПриказПриём>(
            //            q => q.ПриказПриёмId == item.Id));
            //        break;
            //    case OrderType.Dismissal:
            //        wordDocument = Functions.CreateOrder(item.OrderType, Controller.Find<ПриказУвольнение>(
            //            q => q.ПриказУвольнениеId == item.Id));
            //        break;
            //    case OrderType.Vacation:
            //        wordDocument = Functions.CreateOrder(item.OrderType, Controller.Find<ПриказОтпуск>(
            //            q => q.ПриказОтпускId == item.Id));
            //        break;
            //    case OrderType.BusinessTrip:
            //        wordDocument = Functions.CreateOrder(item.OrderType, Controller.Find<ПриказКомандировка>(
            //            q => q.ПриказКомандировкаId == item.Id));
            //        break;
            //    default:
            //        wordDocument = null;
            //        break;
            //}


            if (wordDocument != null)
            {
                wordDocument.Save(sfd.FileName,false);

                wordDocument.Close();
            }
            Controller.CloseConnection();
        }

        private void BPrint_OnClick(object sender, RoutedEventArgs e)
        {
            var item = LvOrders.SelectedItem as OrderInfo;
            if (item != null)
            {
                WordDocument wordDocument;
                Controller.OpenConnection();

                dynamic document = Controller.FindDocumentNotGeneric(item.Id,
                DocumentsTypeComparer.GetDocumentType(item.OrderType));

                wordDocument = Functions.CreateOrder(item.OrderType, document);
                //switch (item.OrderType)
                //{
                //    case OrderType.Recruitment:
                //        wordDocument = Functions.CreateOrder(item.OrderType, Controller.Find<ПриказПриём>(
                //            q => q.ПриказПриёмId == item.Id));
                //        break;
                //    case OrderType.Dismissal:
                //        wordDocument = Functions.CreateOrder(item.OrderType, Controller.Find<ПриказУвольнение>(
                //            q => q.ПриказУвольнениеId == item.Id));
                //        break;
                //    case OrderType.Vacation:
                //        wordDocument = Functions.CreateOrder(item.OrderType, Controller.Find<ПриказОтпуск>(
                //            q => q.ПриказОтпускId == item.Id));
                //        break;
                //    case OrderType.BusinessTrip:
                //        wordDocument = Functions.CreateOrder(item.OrderType, Controller.Find<ПриказКомандировка>(
                //            q => q.ПриказКомандировкаId == item.Id));
                //        break;
                //    default:
                //        wordDocument = null;
                //        break;
                //}
                var savePath = Directory.GetCurrentDirectory() + @"\Temp.docx";
                if (wordDocument != null)
                {
                    wordDocument.Save(savePath,false);
                    wordDocument.Path = savePath;
                    wordDocument.Print();
                    wordDocument.Close();
                }
                Controller.CloseConnection();
            }
        }

        private void bRemove_Click(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;


            var item = LvOrders.SelectedItem as OrderInfo;
            if (item != null)
            {
                Controller.OpenConnection();

                Controller.RemoveDocumentNotGeneric(item.Id,DocumentsTypeComparer.GetDocumentType(item.OrderType));

                Controller.CloseConnection();

                //if (item.Тип == "Приём")
                //{
                //    Controller.OpenConnection();
                //    Controller.Remove<ПриказПриём>(q => q.Номер == item.Номер);
                //    Controller.CloseConnection();
                //}
                //else if (item.Тип == "Увольнение")
                //{
                //    Controller.OpenConnection();
                //    Controller.Remove<ПриказУвольнение>( q => q.Номер == item.Номер);
                //    Controller.CloseConnection();
                //}
                //else if (item.Тип == "Отпуск")
                //{
                //    Controller.OpenConnection();
                //    Controller.Remove<ПриказОтпуск>( q => q.Номер == item.Номер);
                //    Controller.CloseConnection();
                //}
                //else if (item.Тип == "Командировка")
                //{
                //    Controller.OpenConnection();
                //    Controller.Remove<ПриказКомандировка>( q => q.Номер == item.Номер);
                //    Controller.CloseConnection();
                //}
            }

            IsHitTestVisible = true;

            SyncOrders();
        }

        private void bOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SelectedOrderType_OnChanged(object sender, RoutedEventArgs e)
        {
            SyncOrders();
        }

        private void TbSearch_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            SyncOrders();
        }
    }
}
