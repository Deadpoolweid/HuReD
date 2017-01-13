using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Hured.DBModel;
using Hured.Tables_templates;
using Word = Microsoft.Office.Interop.Word;

namespace Hured
{
    static class Functions
    {
        public static void AddUnitsFromDB(ref ListView lvUnits)
        {
            Controller.OpenConnection();
            var units = Controller.Select(new Подразделение(), e => e != null);
            Controller.CloseConnection();

            var stringUnits = units.Select((подразделение) => подразделение.Название);

            foreach (var unit in stringUnits)
            {
                lvUnits.Items.Add(unit);
            }
        }

        public static void AddUnitsFromDB(ref ComboBox cbUnits)
        {
            Controller.OpenConnection();
            var units = Controller.Select(new Подразделение(), e => e != null);
            Controller.CloseConnection();

            var stringUnits = units.Select((подразделение) => подразделение.Название);

            foreach (var unit in stringUnits)
            {
                cbUnits.Items.Add(unit);
            }
        }

        public static List<Должность> GetPositionsForUnit(string unit)
        {
            Controller.OpenConnection();
            var positions = Controller.Select(new Должность(), e => e.Подразделение.Название == unit);
            Controller.CloseConnection();
            return positions;
        }

        public static void AddPositionsFromDB(ref ListView lvPositions, string unit = "")
        {
            List<Должность> positions = new List<Должность>();
            if (unit == "")
            {
                Controller.OpenConnection();
                positions = Controller.Select(new Должность(), e => e != null);
                Controller.CloseConnection();

            }
            else
            {
                positions = GetPositionsForUnit(unit);
            }
            foreach (var должность in positions)
            {
                lvPositions.Items.Add(должность);
            }
        }

        public static void AddPositionsFromDB(ref ComboBox cbPositions, string unit = "")
        {
            List<Должность> positions = new List<Должность>();
            if (unit == "")
            {
                Controller.OpenConnection();
                positions = Controller.Select(new Должность(), e => e != null);
                Controller.CloseConnection();

            }
            else
            {
                positions = GetPositionsForUnit(unit);
            }
            foreach (var должность in positions)
            {
                cbPositions.Items.Add(должность.Название);
            }
        }

        // Вызывает стандартный диалог печати
        public static void Print(OrderType orderType = OrderType.BusinessTrip)
        {

        }

        // Создаёт приказ с заданными параметрами
        public static void CreateOrder<T>(OrderType orderType, T _order, string savePath = null)
        {
            // TODO выводить только последние две цифры года

            // TODO Перенести в инициализацию
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Documents");

            Dictionary<string, string> bookmarks = new Dictionary<string, string>();
            string openPath;
            switch (orderType)
            {
                case OrderType.Recruitment:
                    var order = _order as ПриказПриём;
                    bookmarks.Add("Номер", order.Номер);
                    bookmarks.Add("Дата", order.Дата.ToString());
                    bookmarks.Add("НачалоРаботы", order.НачалоРаботы.ToString());
                    bookmarks.Add("КонецРаботы", order.КонецРаботы.ToString());
                    bookmarks.Add("ТабельныйНомер", order.Сотрудник.ОсновнаяИнформация.ТабельныйНомер);
                    bookmarks.Add("ФИО", order.Сотрудник.ОсновнаяИнформация.Фамилия + " " +
                        order.Сотрудник.ОсновнаяИнформация.Имя + " " +
                        order.Сотрудник.ОсновнаяИнформация.Отчество);
                    bookmarks.Add("Подразделение", order.Должность.Подразделение.Название);
                    bookmarks.Add("Должность", order.Должность.Название);
                    bookmarks.Add("Примечание", order.Примечания);
                    bookmarks.Add("Оклад", order.Оклад);
                    bookmarks.Add("Надбавка", order.Надбавка);
                    bookmarks.Add("ДатаТрудовогоДоговораЧисло", order.ДатаТрудовогоДоговора.Day.ToString());
                    bookmarks.Add("ДатаТрудовогоДоговораМесяц", order.ДатаТрудовогоДоговора.Month.ToString());
                    bookmarks.Add("ДатаТрудовогоДоговораГод", order.ДатаТрудовогоДоговора.Year.ToString());
                    bookmarks.Add("НомерТрудовогоДоговора", order.НомерТрудовогоДоговора);
                    bookmarks.Add("ДолжностьРуководителя", "");// TODO Глобальный класс с настройками
                    openPath = Directory.GetCurrentDirectory() + @"\Templates\Recruitment.dotx";
                    if (savePath == null)
                    {
                        savePath = Directory.GetCurrentDirectory() + @"\Documents\ПриказПриём.docx";

                    }
                    break;
                case OrderType.Dismissal:
                    var oDismissal = _order as ПриказУвольнение;
                    bookmarks.Add("Номер", oDismissal.Номер);
                    bookmarks.Add("Дата", oDismissal.Дата.ToString());
                    bookmarks.Add("ТабельныйНомер", oDismissal.Сотрудник.ОсновнаяИнформация.ТабельныйНомер);
                    bookmarks.Add("ФИО", oDismissal.Сотрудник.ОсновнаяИнформация.Фамилия + " " +
                        oDismissal.Сотрудник.ОсновнаяИнформация.Имя + " " +
                        oDismissal.Сотрудник.ОсновнаяИнформация.Отчество);
                    bookmarks.Add("Подразделение", oDismissal.Сотрудник.ОсновнаяИнформация.Должность.Подразделение.Название);
                    bookmarks.Add("Должность", oDismissal.Сотрудник.ОсновнаяИнформация.Должность.Название);
                    bookmarks.Add("Примечание", oDismissal.Примечание);
                    // TODO Дата увольнения
                    bookmarks.Add("ДатаУвольненияЧисло", "");
                    bookmarks.Add("ДатаУвольненияМесяц", "");
                    bookmarks.Add("ДатаУвольненияГод", "");
                    bookmarks.Add("ДатаТрудовогоДоговораЧисло", oDismissal.ДатаТрудовогоДоговора.Day.ToString());
                    bookmarks.Add("ДатаТрудовогоДоговораМесяц", oDismissal.ДатаТрудовогоДоговора.Month.ToString());
                    bookmarks.Add("ДатаТрудовогоДоговораГод", oDismissal.ДатаТрудовогоДоговора.Year.ToString());
                    bookmarks.Add("НомерТрудовогоДоговора", oDismissal.НомерТрудовогоДоговора);
                    bookmarks.Add("ДолжностьРуководителя", "");// TODO Глобальный класс с настройками
                    openPath = Directory.GetCurrentDirectory() + @"\Templates\Dismissal.dotx";
                    if (savePath == null)
                    {
                        savePath = Directory.GetCurrentDirectory() + @"\Documents\ПриказУвольнение.docx";

                    }
                    break;
                case OrderType.Vacation:
                    var oVacation = _order as ПриказОтпуск;
                    bookmarks.Add("Номер", oVacation.Номер);
                    bookmarks.Add("Дата", oVacation.Дата.ToString());
                    bookmarks.Add("ТабельныйНомер", oVacation.Сотрудник.ОсновнаяИнформация.ТабельныйНомер);
                    bookmarks.Add("ФИО", oVacation.Сотрудник.ОсновнаяИнформация.Фамилия + " " +
                        oVacation.Сотрудник.ОсновнаяИнформация.Имя + " " +
                        oVacation.Сотрудник.ОсновнаяИнформация.Отчество);
                    bookmarks.Add("Подразделение", oVacation.Сотрудник.ОсновнаяИнформация.Должность.Подразделение.Название);
                    bookmarks.Add("Должность", oVacation.Сотрудник.ОсновнаяИнформация.Должность.Название);

                    if (oVacation.Вид == "Ежегодный")
                    {
                        bookmarks.Add("ЕжегодныйДлительность",oVacation.КонецОтпуска.Subtract(oVacation.НачалоОтпуска).Days.ToString());
                        bookmarks.Add("ЕжегодныйНачалоЧисло", oVacation.НачалоОтпуска.Day.ToString());
                        bookmarks.Add("ЕжегодныйНачалоМесяц", oVacation.НачалоОтпуска.Month.ToString());
                        bookmarks.Add("ЕжегодныйНачалоГод", oVacation.НачалоОтпуска.Year.ToString());
                        bookmarks.Add("ЕжегодныйКонецЧисло", oVacation.КонецОтпуска.Day.ToString());
                        bookmarks.Add("ЕжегодныйКонецМесяц", oVacation.КонецОтпуска.Month.ToString());
                        bookmarks.Add("ЕжегодныйКонецГод", oVacation.КонецОтпуска.Year.ToString());
                    }
                    else if (oVacation.Вид == "Единоразовый")
                    {
                        bookmarks.Add("ЕдиноразовыйДлительность",
                            oVacation.КонецОтпуска.Subtract(oVacation.НачалоОтпуска).Days.ToString());
                        bookmarks.Add("ЕдиноразовыйНачалоЧисло", oVacation.НачалоОтпуска.Day.ToString());
                        bookmarks.Add("ЕдиноразовыйНачалоМесяц", oVacation.НачалоОтпуска.Month.ToString());
                        bookmarks.Add("ЕдиноразовыйНачалоГод", oVacation.НачалоОтпуска.Year.ToString());
                        bookmarks.Add("ЕдиноразовыйКонецЧисло", oVacation.КонецОтпуска.Day.ToString());
                        bookmarks.Add("ЕдиноразовыйКонецМесяц", oVacation.КонецОтпуска.Month.ToString());
                        bookmarks.Add("ЕдиноразовыйКонецГод", oVacation.КонецОтпуска.Year.ToString());
                    }
                    else
                    {
                        bookmarks.Add("Другое",oVacation.Вид);
                        bookmarks.Add("ДругоеДлительность",
    oVacation.КонецОтпуска.Subtract(oVacation.НачалоОтпуска).Days.ToString());
                        bookmarks.Add("ДругоеНачалоЧисло", oVacation.НачалоОтпуска.Day.ToString());
                        bookmarks.Add("ДругоеНачалоМесяц", oVacation.НачалоОтпуска.Month.ToString());
                        bookmarks.Add("ДругоеНачалоГод", oVacation.НачалоОтпуска.Year.ToString());
                        bookmarks.Add("ДругоеКонецЧисло", oVacation.КонецОтпуска.Day.ToString());
                        bookmarks.Add("ДругоеКонецМесяц", oVacation.КонецОтпуска.Month.ToString());
                        bookmarks.Add("ДругоеКонецГод", oVacation.КонецОтпуска.Year.ToString());
                    }
                    bookmarks.Add("ДолжностьРуководителя", "");// TODO Глобальный класс с настройками
                    openPath = Directory.GetCurrentDirectory() + @"\Templates\Vacation.dotx";
                    if (savePath == null)
                    {
                        savePath = Directory.GetCurrentDirectory() + @"\Documents\ПриказОтпуск.docx";

                    }
                    break;
                case OrderType.BusinessTrip:
                    var oBusinessTrip = _order as ПриказКомандировка;
                    bookmarks.Add("Номер", oBusinessTrip.Номер);
                    bookmarks.Add("Дата", oBusinessTrip.Дата.ToString());
                    bookmarks.Add("ТабельныйНомер", oBusinessTrip.Сотрудник.ОсновнаяИнформация.ТабельныйНомер);
                    bookmarks.Add("ФИО", oBusinessTrip.Сотрудник.ОсновнаяИнформация.Фамилия + " " +
                        oBusinessTrip.Сотрудник.ОсновнаяИнформация.Имя + " " +
                        oBusinessTrip.Сотрудник.ОсновнаяИнформация.Отчество);
                    bookmarks.Add("Подразделение", oBusinessTrip.Сотрудник.ОсновнаяИнформация.Должность.Подразделение.Название);
                    bookmarks.Add("Должность", oBusinessTrip.Сотрудник.ОсновнаяИнформация.Должность.Название);
                    bookmarks.Add("Срок", oBusinessTrip.КонецКомандировки.Subtract(oBusinessTrip.НачалоКомандировки).Days.ToString());
                    bookmarks.Add("НачалоЧисло", oBusinessTrip.НачалоКомандировки.Day.ToString());
                    bookmarks.Add("НачалоМесяц",oBusinessTrip.НачалоКомандировки.Month.ToString());
                    bookmarks.Add("НачалоГод",oBusinessTrip.НачалоКомандировки.Year.ToString());
                    bookmarks.Add("КонецЧисло",oBusinessTrip.КонецКомандировки.Day.ToString());
                    bookmarks.Add("КонецМесяц",oBusinessTrip.КонецКомандировки.Month.ToString());
                    bookmarks.Add("КонецГод",oBusinessTrip.КонецКомандировки.Year.ToString());
                    bookmarks.Add("Цель", oBusinessTrip.Цель);
                    bookmarks.Add("ЗаСчёт", oBusinessTrip.ЗаСчёт);
                    bookmarks.Add("Основание", oBusinessTrip.Основание);
                    bookmarks.Add("ДолжностьРуководителя", ""); // TODO Добавить должность руководителя
                    openPath = Directory.GetCurrentDirectory() + @"\Templates\BusinessTrip.dotx";
                    if (savePath == null)
                    {
                        savePath = Directory.GetCurrentDirectory() + @"\Documents\ПриказКомандировка.docx";

                    }
                    break;
                default:
                    openPath = savePath = "";
                    break;
            }

            WordDocument document = new WordDocument(openPath,savePath);
            document.SetTemplate(bookmarks,true);
        }

        public static void FillTreeView(ref TreeView tv)
        {
            Controller.OpenConnection();
            var Подразделения = Controller.Select(new Подразделение(), e => e != null);

            TreeViewItem item = new TreeViewItem();

            item.Header = "Все подразделения";
            foreach (var должность in Controller.Select(new Должность(), e => e != null))
            {
                item.Items.Add(должность.Название);
            }
            tv.Items.Add(item);

            foreach (var подразделение in Подразделения)
            {
                item = new TreeViewItem();
                item.Tag = подразделение;
                item.Header = подразделение.Название;
                foreach (var должность in Controller.Select(new Должность(), e => e.Подразделение.ПодразделениеId == подразделение.ПодразделениеId))
                {
                    item.Items.Add(должность.Название);
                }
                tv.Items.Add(item);

            }
            Controller.CloseConnection();
        }

        public static string GetRTBText(RichTextBox rtb)
        {
            var textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            return textRange.Text;
        }

        public static void SetRTBText(RichTextBox rtb, string text)
        {
            FlowDocument document = new FlowDocument();
            Paragraph paragraph = new Paragraph();
            paragraph.Inlines.Add(new Bold(new Run(text)));
            document.Blocks.Add(paragraph);
            rtb.Document = document;
        }

        //public static void OnTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        //{
        //    TreeView tv = sender as TreeView;


        //}
    }

    public enum OrderType
    {
        Recruitment,
        Dismissal,
        Vacation,
        BusinessTrip
    }

    class WordDocument
    {
        public WordDocument()
        {
            OpenPath = Directory.GetCurrentDirectory() +  @"\Templates\Order.dotx";
            SavePath = Directory.GetCurrentDirectory() + @"\Documents\Приказ.docx";
        }

        public WordDocument(string openPath, string savePath)
        {
            OpenPath = openPath;
            SavePath = savePath;
        }

        readonly Word._Application _oWord = new Word.Application();

        private string OpenPath { get; set; }
        private string SavePath { get; set; }

        private Word._Document Document;

        public void Open(string path = null)
        {
            if (path == null)
            {
                path = OpenPath;
            }
            Document = _oWord.Documents.Add(path);
        }

        public void Save(string path = null)
        {
            if (path == null)
            {
                path = SavePath;
            }
            Document.SaveAs(FileName: path);
        }

        public void Close()
        {
            Document.Close();
        }

        public void SetTemplate(Dictionary<string, string> bookmarksValuesDictionary)
        {
            foreach (var bookmark in bookmarksValuesDictionary)
            {
                Document.Bookmarks[bookmark.Key].Range.Text = bookmark.Value;
            }
        }

        public void SetTemplate(Dictionary<string, string> bookmarksValuesDictionary, bool IsDefault)
        {
            if (!IsDefault)
            {
                SetTemplate(bookmarksValuesDictionary);
            }
            else
            {
                Open();
                SetTemplate(bookmarksValuesDictionary);
                Save();
                Close();
            }
        }
    }
}
