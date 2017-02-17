using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using Hured.DBModel;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using Xceed.Wpf.Toolkit.Core;
using System.Windows.Media;
using Microsoft.Win32;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Wizard.xaml
    /// </summary>
    public partial class Wizard
    {
        public Wizard()
        {
            InitializeComponent();
            var w = new MetroWindow();
            BUnits.Click += (sender, args) =>
            {
                IsHitTestVisible = false;
                w = new Units();
                w.ShowDialog();
                IsRecordsAdded = HasOneRecord(w.Tag);
                IsHitTestVisible = true;
            };
            BPositions.Click += (sender, args) =>
            {
                IsHitTestVisible = false;
                w = new Positions();
                w.ShowDialog();
                IsRecordsAdded = HasOneRecord(w.Tag);
                IsHitTestVisible = true;
            };
            BEmployees.Click += (sender, args) =>
            {
                IsHitTestVisible = false;
                w = new Employees();
                w.ShowDialog();
                IsRecordsAdded = HasOneRecord(w.Tag);
                IsHitTestVisible = true;
            };
            BStatuses.Click += (sender, args) =>
            {
                IsHitTestVisible = false;
                w = new Statuses();
                w.ShowDialog();
                IsRecordsAdded = HasOneRecord(w.Tag);
                IsHitTestVisible = true;
            };
            BOrders.Click += (sender, args) =>
            {
                IsHitTestVisible = false;
                w = new Orders();
                w.ShowDialog();
                IsHitTestVisible = true;
            };
            BTimeSheet.Click += (sender, args) =>
            {
                IsHitTestVisible = false;
                w = new Timesheet();
                w.ShowDialog();
                IsHitTestVisible = true;
            };

            foreach (var textbox in this.FindChildren<TextBox>())
            {
                textbox.TextChanged += (sender, args) =>
                {
                    // Запустить проверку на пустоту
                    IsTextBoxesFilled = true;
                };
            }

        }

        private bool HasOneRecord(object windowTag)
        {
            var tResult = windowTag as TransactionResult;

            Debug.Assert(tResult != null, "tResult != null");


            return tResult.RecordsCount > 0;
        }


        public static readonly DependencyProperty IsTextBoxesFilledProperty =
   DependencyProperty.Register("IsTextBoxesFilled", typeof(bool), typeof(Wizard));

        public bool IsTextBoxesFilled
        {
            get
            {
                return (bool)GetValue(IsTextBoxesFilledProperty);
            }
            set
            {
                _contentLoaded = value;
                SetValue(IsTextBoxesFilledProperty, !this.FindChildren<TextBox>().Any(Functions.IsEmpty));
            }
        }

        public static readonly DependencyProperty IsRecordsAddedProperty =
            DependencyProperty.Register("IsRecordsAdded", typeof(bool), typeof(Wizard));

        public bool IsRecordsAdded
        {
            get { return (bool)GetValue(IsRecordsAddedProperty); }
            set
            {
                _contentLoaded = value;
                SetValue(IsRecordsAddedProperty, value);
            }
        }

        private AppSettings _settings = new AppSettings();

        private void ImportDataBase(object sender, RoutedEventArgs args)
        {
            string backUpDirectory = Directory.GetCurrentDirectory() + @"\Backup";

            var ofd = new OpenFileDialog()
            {
                InitialDirectory = backUpDirectory,
                Filter = "SQL |*.sql|Все файлы (*.*)|*.*"
            };

            if (ofd.ShowDialog() == false) return;

            Functions.AddProgressRing(this);

            Controller.ImportDataBase(ofd.FileName);

            IsDatabaseImported = true;

            Functions.RemoveProgressRing();
        }

        private bool IsDatabaseImported = false;

        private void WInit_OnNext(object sender, CancelRoutedEventArgs e)
        {
            var currentPage = WInit.CurrentPage;

            if (currentPage.Name == "DbSettings")
            {
                if (currentPage.FindChildren<TextBox>().Any(Functions.IsEmpty))
                {
                    e.Cancel = true;
                    return;
                }

                var connectionStringBuilder = new MySqlConnectionStringBuilder()
                {
                    Server = TbServer.Text,
                    Port = (uint)NtbPort.Value,
                    UserID = tbUid.Text,
                    Password = PbPassword.Password,
                    PersistSecurityInfo = ChbPersistSecurityInfo.IsChecked.Value
                };
                if (Controller.IsConnectionSucceded(connectionStringBuilder.ConnectionString))
                {
                    currentPage.Background = Brushes.ForestGreen;
                    Functions.ShowPopup(currentPage, "Соединение установлено.");
                }
                else
                {
                    currentPage.Background = Brushes.Red;
                    Functions.ShowPopup(currentPage, "Соединение не установлено.");
                    e.Cancel = true;
                    return;
                }

                var builder = new MySqlConnectionStringBuilder()
                {
                    Server = TbServer.Text,
                    Port = (uint)NtbPort.Value,
                    Database = TbDatabaseName.Text,
                    UserID = tbUid.Text,
                    Password = PbPassword.Password,
                    PersistSecurityInfo = ChbPersistSecurityInfo.IsChecked.Value
                };

                Controller.SetConnectionString(builder.ConnectionString);

                _settings.SetConnectionStringBuilder(builder);

                return;
            }

            if (currentPage.Name == "SelectDb")
            {
                if (rbCreateDatabase.IsChecked == true)
                {
                    currentPage.NextPage = Page2;
                }
                else if (rbIHaveDatabase.IsChecked == true)
                {
                    currentPage.NextPage = ImportDb;
                }
            }

            if (currentPage.Name == "ImportDb")
            {
                if (!IsDatabaseImported)
                {
                    e.Cancel = true;
                    return;
                }
            }

            int currentNumber;
            var isContains = false;
            // 7 - Количестов страниц
            for (currentNumber = 1; currentNumber < 7; currentNumber++)
            {
                if (!currentPage.Name.Contains($"{currentNumber}")) continue;
                isContains = true;
                break;
            }


            if (!isContains) return;
            switch (currentNumber)
            {
                case 1:
                    if (currentPage.FindChildren<TextBox>().Any(Functions.IsEmpty))
                    {
                        e.Cancel = true;
                        return;
                    }

                    _settings.ДолжностьРуководителя = TbДолжностьРуководителя.Text;
                    _settings.НазваниеОрганизации = TbНазваниеОрганизации.Text;
                    _settings.РуководительОрганизации = TbРуководитель.Text;
                    _settings.НормаРабочегоДня = TbНормаРабочегоДня.Text;

                    break;
                case 7:
                case 5:
                    break;
                default:
                    if (!IsRecordsAdded)
                    {
                        Functions.ShowPopup(currentPage, "Добавьте хотя бы одну запись в базу данных.");
                        e.Cancel = true;
                    }
                    IsRecordsAdded = false;
                    break;
            }
        }


        private void WInit_OnFinish(object sender, CancelRoutedEventArgs e)
        {


            var formatter = new BinaryFormatter();
            // Сохранить объект в локальном файле.
            using (Stream fStream = new FileStream("settings.dat",
                FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(fStream, _settings);
            }
        }
    }
}
