using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using Xceed.Wpf.Toolkit.Core;

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

            if (tResult.RecordsCount < 1)
            {
                Functions.ShowPopup(this,"Добавьте хотя бы одну запись.");
            }
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
            get { return (bool) GetValue(IsRecordsAddedProperty); }
            set
            {
                _contentLoaded = value;
                SetValue(IsRecordsAddedProperty,value);
            }
        }

        private AppSettings _settings = new AppSettings();

        private void WInit_OnNext(object sender, CancelRoutedEventArgs e)
        {
            var currentPage = WInit.CurrentPage;

            if (currentPage.Name == "DbSettings")
            {
                _settings.SetConnectionStringBuilder(new MySqlConnectionStringBuilder()
                {
                    Server = TbServer.Text,
                    Port = (uint)NtbPort.Value,
                    Database = TbDatabaseName.Text,
                    UserID = tbUid.Text,
                    Password = PbPassword.Password,
                    PersistSecurityInfo = ChbPersistSecurityInfo.IsChecked.Value
                });

                var formatter = new BinaryFormatter();
                // Сохранить объект в локальном файле.
                using (Stream fStream = new FileStream("settings.dat",
                    FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    formatter.Serialize(fStream, _settings);
                }
                return;
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

                    _settings.ДолжностьРуководителя = TbДолжностьРуководителя.Text;
                    _settings.НазваниеОрганизации = TbНазваниеОрганизации.Text;
                    _settings.РуководительОрганизации = TbРуководитель.Text;
                    _settings.НормаРабочегоДня = TbНормаРабочегоДня.Text;
                    
                    break;
                case 7:
                case 5:
                    currentPage.CanSelectNextPage = true;
                    break;
                default:
                    IsRecordsAdded = false;
                    break;
            }
        }
    }
}
