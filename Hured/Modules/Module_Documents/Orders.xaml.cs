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
using Hured.DataBase;
using Hured.Tables_templates;
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
            foreach (var document in DocumentsTypeDictionary.GetTypesOfAllDocuments())
            {
                // Получаем название документа (Исключая слово приказ из имени)
                var name = string.Concat(document.Name.Select((x, i) => i > 0 && char.IsUpper(x) ? " " + x.ToString() : x.ToString()));

                var rb = new RadioButton()
                {
                    Content = name,
                    GroupName = "DocumentTypes",
                    Name = "rb" + DocumentsTypeDictionary.GetEnumTypeOfDocumentByName(document.Name)
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

        public List<T> FilterOrdersByTags<T>(string[] tags, List<T> employees) where T:IДокумент
        {
            var searchResult = employees.Where(
                q => new Regex(string.Join("|", tags.Select(Regex.Escape)), RegexOptions.IgnoreCase).IsMatch(
                    q.Сотрудник.ОсновнаяИнформация.Имя + q.Сотрудник.ОсновнаяИнформация.Фамилия +
                    q.Сотрудник.ОсновнаяИнформация.Отчество)
                    );
            return searchResult.ToList();
        }

        public ArrayList FilterOrdersByTagsNotGeneric(string[] tags, ArrayList employees, Type type)
        {
            var result = new ArrayList();

            var regex = new Regex(string.Join("|",tags.Select(Regex.Escape)),RegexOptions.IgnoreCase);

            foreach (var e in employees)
            {
                dynamic element = e as IДокумент;
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
            try
            {
                LvOrders.Items.Clear();


                bool needFilter = tbSearch.Text != String.Empty && !tbSearch.IsHavePlaceholder();
                var tags = tbSearch.Text.Split(' ');

                foreach (var child in spDocumentTypes.Children)
                {
                    var rb = child as RadioButton;

                    var type = DocumentsTypeDictionary.GetDocumentTypeByName(rb.Name.Substring(2, rb.Name.Length - 2));
                    if (rb.IsChecked == true)
                    {
                        var selectMethod = typeof(Controller).GetMethod("SelectAll");

                        var listoftype = typeof(ArrayList);

                        dynamic documents = Activator.CreateInstance(listoftype);

                        Controller.OpenConnection();
                        object result = selectMethod.Invoke(null, new object[]
                        {
                            type
                        });
                        Controller.CloseConnection();

                        documents = Convert.ChangeType(result, listoftype);

                        if (needFilter)
                        {
                            var filterOrdersByTags = typeof(Hured.Orders).GetMethod("FilterOrdersByTagsNotGeneric");
                            documents =
                                Convert.ChangeType(
                                    filterOrdersByTags.Invoke(this, new object[] {tags, documents, type}), listoftype);
                        }

                        foreach (var e in documents)
                        {
                            var id = type.GetProperty(type.Name + "Id").GetValue(e);

                            LvOrders.Items.Add(new OrderInfo(e.Номер,
                                e.Сотрудник.ОсновнаяИнформация.Фамилия + " " +
                                e.Сотрудник.ОсновнаяИнформация.Имя + " " +
                                e.Сотрудник.ОсновнаяИнформация.Отчество, type.Name.Substring(6, type.Name.Length - 6),
                                e.Дата.ToShortDateString())
                            {
                                Id = id,
                                OrderType = OrderType.Recruitment,
                                Type = e.GetType()
                            });
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Close();
                //throw;
            }
            finally
            {
                Controller.CloseConnection(true);
            }
        }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IsHitTestVisible = false;

                var w = new Order();
                w.ShowDialog();

                SyncOrders();
            }
            catch (Exception ex)
            {
                Functions.ShowPopup(sender as Button, "Не удалось добавить документ. Информация: " + ex);
            }
            finally
            {
                IsHitTestVisible = true;
            }
        }

        private void bChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IsHitTestVisible = false;


                var item = LvOrders.SelectedItem as OrderInfo;
                if (item != null)
                {
                    var w = new Order(item);
                    w.ShowDialog();
                }

                SyncOrders();
            }
            catch (Exception ex)
            {
                Functions.ShowPopup(sender as Button, "Не удалось изменить документ. Информация: " + ex);
            }
            finally
            {
                IsHitTestVisible = true;
            }
        }

        private void BOpen_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = LvOrders.SelectedItem as OrderInfo;
                if (item != null)
                {
                    WordDocument wordDocument;

                    dynamic document = ControllerExtensions.FindDocumentNotGeneric(item.Id,
                        DocumentsTypeDictionary.GetDocumentTypeByEnum(item.OrderType));

                    Controller.OpenConnection();
                    wordDocument = Functions.CreateOrder(item.OrderType, document);
                    Controller.CloseConnection();

                    var savePath = Directory.GetCurrentDirectory() + @"\Temp.docx";
                    if (wordDocument != null)
                    {
                        wordDocument.Save(savePath);
                        wordDocument.Path = savePath;
                        wordDocument.Close();
                        wordDocument.OpenWithWord();
                    }
                }
            }
            catch (Exception ex)
            {
                Functions.ShowPopup(sender as Button, "Не удалось открыть документ. Информация: " + ex);
            }
            finally
            {
                Controller.CloseConnection(true);
            }
        }

        private void BSave_OnClick(object sender, RoutedEventArgs e)
        {
            try
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
                dynamic document = ControllerExtensions.FindDocumentNotGeneric(item.Id,
                    DocumentsTypeDictionary.GetDocumentTypeByEnum(item.OrderType));
                Controller.CloseConnection();

                wordDocument = Functions.CreateOrder(item.OrderType, document);

                if (wordDocument != null)
                {
                    wordDocument.Save(sfd.FileName, false);

                    wordDocument.Close();
                }
            }
            catch (Exception ex)
            {
                Functions.ShowPopup(sender as Button, "Не удалось сохранить документ. Информация: " + ex);
            }
            finally
            {
                Controller.CloseConnection(true);
            }
        }

        private void BPrint_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = LvOrders.SelectedItem as OrderInfo;
                if (item != null)
                {
                    WordDocument wordDocument;

                    Controller.OpenConnection();
                    dynamic document = ControllerExtensions.FindDocumentNotGeneric(item.Id,
                        DocumentsTypeDictionary.GetDocumentTypeByEnum(item.OrderType));
                    Controller.CloseConnection();

                    wordDocument = Functions.CreateOrder(item.OrderType, document);
                    var savePath = Directory.GetCurrentDirectory() + @"\Temp.docx";
                    if (wordDocument != null)
                    {
                        wordDocument.Save(savePath, false);
                        wordDocument.Path = savePath;
                        wordDocument.Print();
                        wordDocument.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Functions.ShowPopup(sender as Button, "Не удалось распечатать документ. Информация: " + ex);
            }
            finally
            {
                Controller.CloseConnection(true);
            }
        }

        private void bRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IsHitTestVisible = false;

                var item = LvOrders.SelectedItem as OrderInfo;
                if (item != null)
                {
                    Controller.OpenConnection();

                    ControllerExtensions.RemoveDocumentNotGeneric(item.Id,
                        DocumentsTypeDictionary.GetDocumentTypeByEnum(item.OrderType));

                    Controller.CloseConnection();
                }

                SyncOrders();
            }
            catch (Exception ex)
            {
                Functions.ShowPopup(sender as Button, "Не удалось удалить документ. Информация: " + ex);
            }
            finally
            {
                IsHitTestVisible = true;

            }
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
