using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
using MahApps.Metro.Controls;
using Xceed.Wpf.Toolkit.Core;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Wizard.xaml
    /// </summary>
    public partial class Wizard : MetroWindow
    {
        public Wizard()
        {
            InitializeComponent();
            MetroWindow w = new MetroWindow();
            bUnits.Click += (sender, args) =>
            {
                this.IsHitTestVisible = false;
                w = new Units();
                w.ShowDialog();
                IsRecordsAdded = HasOneRecord(w.Tag);
                IsHitTestVisible = true;
            };
            bPositions.Click += (sender, args) =>
            {
                this.IsHitTestVisible = false;
                w = new Positions();
                w.ShowDialog();
                IsRecordsAdded = HasOneRecord(w.Tag);
                IsHitTestVisible = true;
            };
            bEmployees.Click += (sender, args) =>
            {
                this.IsHitTestVisible = false;
                w = new Employees();
                w.ShowDialog();
                IsRecordsAdded = HasOneRecord(w.Tag);
                IsHitTestVisible = true;
            };
            bStatuses.Click += (sender, args) =>
            {
                this.IsHitTestVisible = false;
                w = new Statuses();
                w.ShowDialog();
                IsRecordsAdded = HasOneRecord(w.Tag);
                IsHitTestVisible = true;
            };
            bOrders.Click += (sender, args) =>
            {
                this.IsHitTestVisible = false;
                w = new Orders();
                w.ShowDialog();
                IsHitTestVisible = true;
            };
            bTimeSheet.Click += (sender, args) =>
            {
                this.IsHitTestVisible = false;
                w = new Timesheet();
                w.ShowDialog();
                IsHitTestVisible = true;
            };

            foreach (var textbox in this.FindChildren<TextBox>())
            {
                textbox.TextChanged += (sender, args) =>
                {
                    IsTextBoxesFilled = true;
                };
            }
            
        }

        private bool HasOneRecord(object windowTag)
        {
            var tResult = windowTag as TransactionResult;
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
                return (bool)this.GetValue(IsTextBoxesFilledProperty);
            }
            set
            {
                _contentLoaded = value;
                this.SetValue(IsTextBoxesFilledProperty, !this.FindChildren<TextBox>().Any(Functions.IsEmpty));
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

        private void WInit_OnNext(object sender, CancelRoutedEventArgs e)
        {
            var currentPage = wInit.CurrentPage;

            int currentNumber;
            bool isContains = false;
            // 7 - Количестов страниц
            for (currentNumber = 1; currentNumber < 7; currentNumber++)
            {
                if (currentPage.Name.Contains($"{currentNumber}"))
                {
                    isContains = true;
                    break;
                }
            }


            if (isContains)
            {
                if (currentNumber == 1)
                {
                    AppSettings s = new AppSettings()
                    {
                        ДолжностьРуководителя = tbДолжностьРуководителя.Text,
                        НазваниеОрганизации = tbНазваниеОрганизации.Text,
                        РуководительОрганизации = tbРуководитель.Text,
                        НормаРабочегоДня = tbНормаРабочегоДня.Text,
                    };

                    var formatter = new BinaryFormatter();
                    // Сохранить объект в локальном файле.
                    using (Stream fStream = new FileStream("settings.dat",
                       FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        formatter.Serialize(fStream, s);
                    }
                }
                else if (currentNumber == 7 || currentNumber == 5)
                {
                    currentPage.CanSelectNextPage = true;
                }
                else
                {

                    IsRecordsAdded = false;
                } 
            }
        }
    }
}
