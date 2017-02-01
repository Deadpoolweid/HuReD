using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Hured.DBModel;
using Hured.Tables_templates;
using Microsoft.Office.Interop.Word;
using Application = Microsoft.Office.Interop.Word.Application;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Drawing.Image;
using Paragraph = System.Windows.Documents.Paragraph;

namespace Hured
{
    internal static class Functions
    {
        public static void AddUnitsFromDB(ref ListView lvUnits)
        {
            Controller.OpenConnection();
            var units = Controller.Select(new Подразделение(), e => e != null);
            Controller.CloseConnection();

            foreach (var unit in units)
            {
                var newItem = new ListViewItem();
                newItem.Content = unit;
                newItem.Tag = unit.ПодразделениеId;
                lvUnits.Items.Add(newItem);
            }
        }

        public static void AddUnitsFromDB(ref ComboBox cbUnits)
        {
            Controller.OpenConnection();
            var units = Controller.Select(new Подразделение(), e => e != null);
            Controller.CloseConnection();

            foreach (var unit in units)
            {
                var newItem = new ComboBoxItem();
                newItem.Content = unit;
                newItem.Tag = unit.ПодразделениеId;
                cbUnits.Items.Add(newItem);
            }
        }

        public static void AddUnitsFromDB(ref ListBox lbUnits)
        {
            Controller.OpenConnection();
            var units = Controller.Select(new Подразделение(), e => e != null);
            Controller.CloseConnection();

            foreach (var unit in units)
            {
                var newItem = new ListBoxItem();
                newItem.Content = unit;
                newItem.Tag = unit.ПодразделениеId;
                lbUnits.Items.Add(newItem);
            }
        }

        public static List<Должность> GetPositionsForUnit(int unitId)
        {
            Controller.OpenConnection();
            var positions = Controller.Select(new Должность(), e => e.Подразделение.ПодразделениеId == unitId);
            Controller.CloseConnection();
            return positions;
        }

        public static void AddPositionsFromDB(ref ListView lvPositions, int unitId = -1)
        {
            List<Должность> positions;
            if (unitId == -1)
            {
                Controller.OpenConnection();
                positions = Controller.Select(new Должность(), e => e != null);
                Controller.CloseConnection();

            }
            else
            {
                positions = GetPositionsForUnit(unitId);
            }
            foreach (var должность in positions)
            {
                lvPositions.Items.Add(должность);
            }
        }

        public static void AddPositionsFromDB(ref ComboBox cbPositions, int unitId = -1)
        {
            List<Должность> positions;
            if (unitId == -1)
            {
                Controller.OpenConnection();
                positions = Controller.Select(new Должность(), e => e != null);
                Controller.CloseConnection();

            }
            else
            {
                positions = GetPositionsForUnit(unitId);
            }
            foreach (var должность in positions)
            {
                var newItem = new ComboBoxItem();
                newItem.Content = должность.Название;
                newItem.Tag = должность.ДолжностьId;
                cbPositions.Items.Add(newItem);
            }
        }

        /// <summary>
        /// Создаёт приказ с заданными параметрами
        /// </summary>
        /// <typeparam name="T">Тип приказа</typeparam>
        /// <param name="orderType">Тип приказа</param>
        /// <param name="_order">Приказ</param>
        /// <returns></returns>
        public static WordDocument CreateOrder<T>(OrderType orderType, T _order)
        {
            if (_order == null) throw new ArgumentNullException(nameof(_order));
            // TODO Перенести в инициализацию
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Documents");

            var bookmarks = new Dictionary<string, string>();
            string openPath;

            var расшифровкаПодписи = GetAppSettings().РуководительОрганизации;
            расшифровкаПодписи = расшифровкаПодписи.Split(' ')[0] + " " +
                                 расшифровкаПодписи.Split(' ')[1][0] + "." +
                                 расшифровкаПодписи.Split(' ')[2][0] + ".";

            switch (orderType)
            {
                case OrderType.Recruitment:
                    var order = _order as ПриказПриём;
                    bookmarks.Add("Номер", order.Номер);
                    bookmarks.Add("Дата", order.Дата.ToShortDateString());
                    bookmarks.Add("НачалоРаботы", order.НачалоРаботы.ToShortDateString());
                    bookmarks.Add("КонецРаботы", order.КонецРаботы.ToShortDateString());
                    bookmarks.Add("ТабельныйНомер", order.Сотрудник.ОсновнаяИнформация.ТабельныйНомер);
                    bookmarks.Add("ФИО", order.Сотрудник.ОсновнаяИнформация.Фамилия + " " +
                                         order.Сотрудник.ОсновнаяИнформация.Имя + " " +
                                         order.Сотрудник.ОсновнаяИнформация.Отчество);
                    bookmarks.Add("Подразделение", order.Должность.Подразделение.Название);
                    bookmarks.Add("Должность", order.Должность.Название);
                    bookmarks.Add("Примечание", order.Примечания);

                    var оклад = order.Оклад.Split(new[] { '.', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    bookmarks.Add("ОкладРубли", оклад[0]);
                    bookmarks.Add("ОкладКопейки", оклад.Length > 1 ? оклад[1] : "00");

                    var надбавка = order.Надбавка.Split(new[] { '.', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    bookmarks.Add("НадбавкаРубли", надбавка[0]);
                    bookmarks.Add("НадбавкаКопейки", надбавка.Length > 1 ? надбавка[1] : "00");
                    bookmarks.Add("ДатаТрудовогоДоговораЧисло", order.ДатаТрудовогоДоговора.Day.ToString());
                    bookmarks.Add("ДатаТрудовогоДоговораМесяц", order.ДатаТрудовогоДоговора.Month.ToString());
                    bookmarks.Add("ДатаТрудовогоДоговораГод", order.ДатаТрудовогоДоговора.Year.ToString().Substring(2));
                    bookmarks.Add("НомерТрудовогоДоговора", order.НомерТрудовогоДоговора);
                    bookmarks.Add("ДолжностьРуководителя", GetAppSettings().ДолжностьРуководителя);
                    bookmarks.Add("НазваниеОрганизации", GetAppSettings().НазваниеОрганизации);
                    bookmarks.Add("РасшифровкаПодписи", расшифровкаПодписи);
                    if (order.ИспытательныйСрок)
                    {
                        bookmarks.Add("ИспытательныйСрокДлительность", order.ИспытательныйСрокДлительность);
                    }
                    openPath = Directory.GetCurrentDirectory() + @"\Templates\Recruitment.dotx";
                    break;
                case OrderType.Dismissal:
                    var oDismissal = _order as ПриказУвольнение;
                    bookmarks.Add("Номер", oDismissal.Номер);
                    bookmarks.Add("Дата", oDismissal.Дата.ToShortDateString());
                    bookmarks.Add("ТабельныйНомер", oDismissal.Сотрудник.ОсновнаяИнформация.ТабельныйНомер);
                    bookmarks.Add("ФИО", oDismissal.Сотрудник.ОсновнаяИнформация.Фамилия + " " +
                                         oDismissal.Сотрудник.ОсновнаяИнформация.Имя + " " +
                                         oDismissal.Сотрудник.ОсновнаяИнформация.Отчество);
                    bookmarks.Add("Подразделение",
                        oDismissal.Сотрудник.ОсновнаяИнформация.Должность.Подразделение.Название);
                    bookmarks.Add("Должность", oDismissal.Сотрудник.ОсновнаяИнформация.Должность.Название);
                    bookmarks.Add("ОсновнаиеДокумент", oDismissal.ОснованиеДокумент);
                    bookmarks.Add("ДатаУвольненияЧисло", oDismissal.ДатаУвольнения.Day.ToString());
                    bookmarks.Add("ДатаУвольненияМесяц", oDismissal.ДатаУвольнения.Month.ToString());
                    bookmarks.Add("ДатаУвольненияГод", oDismissal.ДатаУвольнения.Year.ToString().Substring(2));
                    bookmarks.Add("ДатаТрудовогоДоговораЧисло", oDismissal.ДатаТрудовогоДоговора.Day.ToString());
                    bookmarks.Add("ДатаТрудовогоДоговораМесяц", oDismissal.ДатаТрудовогоДоговора.Month.ToString());
                    bookmarks.Add("ДатаТрудовогоДоговораГод",
                        oDismissal.ДатаТрудовогоДоговора.Year.ToString().Substring(2));
                    bookmarks.Add("НомерТрудовогоДоговора", oDismissal.НомерТрудовогоДоговора);
                    bookmarks.Add("ДолжностьРуководителя", GetAppSettings().ДолжностьРуководителя);
                    bookmarks.Add("НазваниеОрганизации", GetAppSettings().НазваниеОрганизации);
                    bookmarks.Add("РасшифровкаПодписи", расшифровкаПодписи);
                    openPath = Directory.GetCurrentDirectory() + @"\Templates\Dismissal.dotx";
                    break;
                case OrderType.Vacation:
                    var oVacation = _order as ПриказОтпуск;
                    bookmarks.Add("Номер", oVacation.Номер);
                    bookmarks.Add("Дата", oVacation.Дата.ToShortDateString());
                    bookmarks.Add("ТабельныйНомер", oVacation.Сотрудник.ОсновнаяИнформация.ТабельныйНомер);
                    bookmarks.Add("ФИО", oVacation.Сотрудник.ОсновнаяИнформация.Фамилия + " " +
                                         oVacation.Сотрудник.ОсновнаяИнформация.Имя + " " +
                                         oVacation.Сотрудник.ОсновнаяИнформация.Отчество);
                    bookmarks.Add("Подразделение",
                        oVacation.Сотрудник.ОсновнаяИнформация.Должность.Подразделение.Название);
                    bookmarks.Add("Должность", oVacation.Сотрудник.ОсновнаяИнформация.Должность.Название);

                    bookmarks.Add("ПериодРаботыНачалоДень", oVacation.ПериодРаботыНачало.Day.ToString());
                    bookmarks.Add("ПериодРаботыНачалоМесяц", oVacation.ПериодРаботыНачало.Month.ToString());
                    bookmarks.Add("ПериодРаботыНачалоГод", oVacation.ПериодРаботыНачало.Year.ToString());
                    bookmarks.Add("ПериодРаботыКонецДень", oVacation.ПериодРаботыНачало.Day.ToString());
                    bookmarks.Add("ПериодРаботыКонецМесяц", oVacation.ПериодРаботыНачало.Month.ToString());
                    bookmarks.Add("ПериодРаботыКонецГод", oVacation.ПериодРаботыНачало.Year.ToString());


                    if (oVacation.Вид == "Ежегодный")
                    {
                        bookmarks.Add("ЕжегодныйДлительность",
                            oVacation.КонецОтпуска.Subtract(oVacation.НачалоОтпуска).Days.ToString());
                        bookmarks.Add("ЕжегодныйНачалоЧисло", oVacation.НачалоОтпуска.Day.ToString());
                        bookmarks.Add("ЕжегодныйНачалоМесяц", oVacation.НачалоОтпуска.Month.ToString());
                        bookmarks.Add("ЕжегодныйНачалоГод", oVacation.НачалоОтпуска.Year.ToString().Substring(2));
                        bookmarks.Add("ЕжегодныйКонецЧисло", oVacation.КонецОтпуска.Day.ToString());
                        bookmarks.Add("ЕжегодныйКонецМесяц", oVacation.КонецОтпуска.Month.ToString());
                        bookmarks.Add("ЕжегодныйКонецГод", oVacation.КонецОтпуска.Year.ToString().Substring(2));
                    }
                    else if (oVacation.Вид == "Единоразовый")
                    {
                        bookmarks.Add("ЕдиноразовыйДлительность",
                            oVacation.КонецОтпуска.Subtract(oVacation.НачалоОтпуска).Days.ToString());
                        bookmarks.Add("ЕдиноразовыйНачалоЧисло", oVacation.НачалоОтпуска.Day.ToString());
                        bookmarks.Add("ЕдиноразовыйНачалоМесяц", oVacation.НачалоОтпуска.Month.ToString());
                        bookmarks.Add("ЕдиноразовыйНачалоГод", oVacation.НачалоОтпуска.Year.ToString().Substring(2));
                        bookmarks.Add("ЕдиноразовыйКонецЧисло", oVacation.КонецОтпуска.Day.ToString());
                        bookmarks.Add("ЕдиноразовыйКонецМесяц", oVacation.КонецОтпуска.Month.ToString());
                        bookmarks.Add("ЕдиноразовыйКонецГод", oVacation.КонецОтпуска.Year.ToString().Substring(2));
                    }
                    else
                    {
                        bookmarks.Add("Другое", oVacation.Вид);
                        bookmarks.Add("ДругоеДлительность",
                            oVacation.КонецОтпуска.Subtract(oVacation.НачалоОтпуска).Days.ToString());
                        bookmarks.Add("ДругоеНачалоЧисло", oVacation.НачалоОтпуска.Day.ToString());
                        bookmarks.Add("ДругоеНачалоМесяц", oVacation.НачалоОтпуска.Month.ToString());
                        bookmarks.Add("ДругоеНачалоГод", oVacation.НачалоОтпуска.Year.ToString().Substring(2));
                        bookmarks.Add("ДругоеКонецЧисло", oVacation.КонецОтпуска.Day.ToString());
                        bookmarks.Add("ДругоеКонецМесяц", oVacation.КонецОтпуска.Month.ToString());
                        bookmarks.Add("ДругоеКонецГод", oVacation.КонецОтпуска.Year.ToString().Substring(2));
                    }
                    bookmarks.Add("ДолжностьРуководителя", GetAppSettings().ДолжностьРуководителя);
                    bookmarks.Add("НазваниеОрганизации", GetAppSettings().НазваниеОрганизации);
                    bookmarks.Add("РасшифровкаПодписи", расшифровкаПодписи);
                    openPath = Directory.GetCurrentDirectory() + @"\Templates\Vacation.dotx";

                    break;
                case OrderType.BusinessTrip:
                    var oBusinessTrip = _order as ПриказКомандировка;
                    bookmarks.Add("Номер", oBusinessTrip.Номер);
                    bookmarks.Add("Дата", oBusinessTrip.Дата.ToShortDateString());
                    bookmarks.Add("ТабельныйНомер", oBusinessTrip.Сотрудник.ОсновнаяИнформация.ТабельныйНомер);
                    bookmarks.Add("ФИО", oBusinessTrip.Сотрудник.ОсновнаяИнформация.Фамилия + " " +
                                         oBusinessTrip.Сотрудник.ОсновнаяИнформация.Имя + " " +
                                         oBusinessTrip.Сотрудник.ОсновнаяИнформация.Отчество);
                    bookmarks.Add("Подразделение",
                        oBusinessTrip.Сотрудник.ОсновнаяИнформация.Должность.Подразделение.Название);
                    bookmarks.Add("Должность", oBusinessTrip.Сотрудник.ОсновнаяИнформация.Должность.Название);
                    bookmarks.Add("Срок",
                        oBusinessTrip.КонецКомандировки.Subtract(oBusinessTrip.НачалоКомандировки).Days.ToString());
                    bookmarks.Add("НачалоЧисло", oBusinessTrip.НачалоКомандировки.Day.ToString());
                    bookmarks.Add("НачалоМесяц", oBusinessTrip.НачалоКомандировки.Month.ToString());
                    bookmarks.Add("НачалоГод", oBusinessTrip.НачалоКомандировки.Year.ToString().Substring(2));
                    bookmarks.Add("КонецЧисло", oBusinessTrip.КонецКомандировки.Day.ToString());
                    bookmarks.Add("КонецМесяц", oBusinessTrip.КонецКомандировки.Month.ToString());
                    bookmarks.Add("КонецГод", oBusinessTrip.КонецКомандировки.Year.ToString().Substring(2));
                    bookmarks.Add("Цель", oBusinessTrip.Цель);
                    bookmarks.Add("ЗаСчёт", oBusinessTrip.ЗаСчёт);
                    bookmarks.Add("Основание", oBusinessTrip.Основание);
                    bookmarks.Add("ДолжностьРуководителя", GetAppSettings().ДолжностьРуководителя);
                    bookmarks.Add("НазваниеОрганизации", GetAppSettings().НазваниеОрганизации);
                    bookmarks.Add("РасшифровкаПодписи", расшифровкаПодписи);
                    openPath = Directory.GetCurrentDirectory() + @"\Templates\BusinessTrip.dotx";

                    break;
                default:
                    openPath = null;
                    break;
            }

            var document = new WordDocument(openPath);
            document.Open();
            document.SetTemplate(bookmarks);
            return document;
        }

        public static void FillTreeView(ref TreeView tv)
        {

            Controller.OpenConnection();
            var подразделения = Controller.Select(new Подразделение(), e => e != null);

            var item = new TreeViewItem();

            item.Header = "Все подразделения";
            foreach (var должность in Controller.Select(new Должность(), e => e != null))
            {
                item.Items.Add(new TreeViewItem
                {
                    Header = должность.Название,
                    Tag = должность.ДолжностьId
                });
            }
            tv.Items.Add(item);


            foreach (var подразделение in подразделения)
            {
                item = new TreeViewItem();
                item.Tag = подразделение.ПодразделениеId;
                item.Header = подразделение.Название;
                var subItems = Controller.Select(new Должность(),
                    e => e.Подразделение.ПодразделениеId == подразделение.ПодразделениеId).Select(должность => new TreeViewItem
                    {
                        Header = должность.Название,
                        Tag = должность.ДолжностьId
                    }).ToList();
                item.ItemsSource = subItems;
                tv.Items.Add(item);

            }
            Controller.CloseConnection();
        }

        public static bool IsEmpty(TextBox textbox)
        {
            var text = textbox.Text;
            if (text == "")
            {
                ShowPopup(textbox,"Заполните это поле.");
                textbox.BorderBrush = Brushes.Red;
                return true;
            }
            textbox.BorderBrush = Brushes.Black;
            return false;
        }

        public static bool IsNumber(TextBox textbox)
        {
            var text = textbox.Text;
            if (Regex.IsMatch(text,"\\D"))
            {
                ShowPopup(textbox,"Допускаются только цифры.");
                textbox.BorderBrush = Brushes.Red;
                return false;
            }
            textbox.BorderBrush = Brushes.Black;
            return true;
        }

        public static string GetRtbText(RichTextBox rtb)
        {
            var textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            return textRange.Text;
        }

        public static void SetRtbText(RichTextBox rtb, string text)
        {
            var document = new FlowDocument();
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(new Bold(new Run(text)));
            document.Blocks.Add(paragraph);
            rtb.Document = document;
        }

        public static AppSettings GetAppSettings()
        {
            if (File.Exists(Directory.GetCurrentDirectory() + "/settings.dat"))
            {
                var formatter = new BinaryFormatter();

                using (Stream fStream = File.OpenRead("settings.dat"))
                {
                    return formatter.Deserialize(fStream) as AppSettings;
                }
            }
            return null;
        }

        static public byte[] ImageToByteArray(Image imageIn)
        {
            var ms = new MemoryStream();
            imageIn.Save(ms, ImageFormat.Gif);
            return ms.ToArray();
        }

        static public Image ByteArrayToImage(byte[] byteArrayIn)
        {
            var ms = new MemoryStream(byteArrayIn);
            var returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public static BitmapSource GetImageStream(Image myImage)
        {
            var bitmap = new Bitmap(myImage);
            var bmpPt = bitmap.GetHbitmap();
            var bitmapSource =
             Imaging.CreateBitmapSourceFromHBitmap(
                   bmpPt,
                   IntPtr.Zero,
                   Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());

            //freeze bitmapSource and clear memory to avoid memory leaks
            bitmapSource.Freeze();
            DeleteObject(bmpPt);

            return bitmapSource;
        }

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr value);

        static public byte[] ImageSourceToBytes(BitmapEncoder encoder, ImageSource imageSource)
        {
            byte[] bytes = null;
            var bitmapSource = imageSource as BitmapSource;

            if (bitmapSource != null)
            {
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (var stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    bytes = stream.ToArray();
                }
            }

            return bytes;
        }

        public static void ShowPopup(UIElement element, string message)
        {
            var popup = new Popup
            {
                StaysOpen = false,
                PlacementTarget = element,
                PopupAnimation = PopupAnimation.Fade,
                AllowsTransparency = true,
                Child = new Label
                {
                    Background = Brushes.AliceBlue,
                    Content = "Заполните это поле.",
                    BorderBrush = Brushes.Red
                }
            };
            popup.Opened += (o, args) =>
            {
                var timer = new Timer(2000);
                timer.Elapsed += (sender1, eventArgs) =>
                {

                    popup.Dispatcher.Invoke(() =>
                    {
                        popup.IsOpen = false;
                    });

                };
                timer.Start();
            };
            popup.IsOpen = true;
        }
    }

    public enum OrderType
    {
        Recruitment,
        Dismissal,
        Vacation,
        BusinessTrip
    }

    /// <summary>
    /// Класс для управления файлом Word
    /// </summary>
    class WordDocument
    {
        public string Path { get; set; }

        /// <summary>
        /// Создаёт интерфейс управления документом, находящемся по указанному пути
        /// </summary>
        /// <param name="path">Местонахождение документа</param>
        public WordDocument(string path)
        {
            Path = path;
        }

        private _Application _wordApp;

        private _Document _document;

        /// <summary>
        /// Открывает фоновое приложение MS Word и документ
        /// </summary>
        public void Open()
        {
            _wordApp = new Application();
            _document = _wordApp.Documents.Add(Path);
        }

        /// <summary>
        /// Открывает документ с помощью MS Word
        /// </summary>
        public void OpenWithWord()
        {
            Process.Start(Path);
        }

        /// <summary>
        /// Сохраняет документ в указанном месте
        /// </summary>
        /// <param name="path">Место для сохранения документа</param>
        /// <param name="withAlert">Показывать предупреждение, если файл уже существует</param>
        public void Save(string path, bool withAlert = true)
        {
            if (withAlert == false)
            {
                _wordApp.DisplayAlerts = WdAlertLevel.wdAlertsNone;
            }
            _document.SaveAs(path);

            _wordApp.DisplayAlerts = WdAlertLevel.wdAlertsAll;
        }

        /// <summary>
        /// Выводит документ на печать
        /// </summary>
        public void Print()
        {

            object nullobj = Missing.Value;
            object path = Path;
            object _readonly = true;
            var doc = _wordApp.Documents.Open(ref path,
                                         ref nullobj, ref _readonly, ref nullobj,
                                         ref nullobj, ref nullobj, ref nullobj,
                                         ref nullobj, ref nullobj, ref nullobj,
                                         ref nullobj, ref nullobj, ref nullobj,
                                         ref nullobj, ref nullobj, ref nullobj);

            doc.Activate();
            _wordApp.Visible = false;
            var dialogResult = _wordApp.Dialogs[WdWordDialog.wdDialogFilePrint].Show(ref nullobj);

            if (dialogResult == 1)
            {
                doc.PrintOut(ref nullobj, ref nullobj, ref nullobj, ref nullobj,
                             ref nullobj, ref nullobj, ref nullobj, ref nullobj,
                             ref nullobj, ref nullobj, ref nullobj, ref nullobj,
                             ref nullobj, ref nullobj, ref nullobj, ref nullobj,
                             ref nullobj, ref nullobj);
            }
        }

        /// <summary>
        /// Закрывает документ и закрывает фоновое приложение
        /// </summary>
        public void Close()
        {
            _wordApp.Quit();
        }

        /// <summary>
        /// Устанавливает текст в закладках документа в соответствии с указанным словарём
        /// </summary>
        /// <param name="bookmarksValuesDictionary">Словарь закладок формата (Имя закладки, Текст в месте закладки)</param>
        public void SetTemplate(Dictionary<string, string> bookmarksValuesDictionary)
        {
            foreach (var bookmark in bookmarksValuesDictionary)
            {
                _document.Bookmarks[bookmark.Key].Range.Text = bookmark.Value;
            }
        }
    }
}
