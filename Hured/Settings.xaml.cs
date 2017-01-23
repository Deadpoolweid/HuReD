using System;
using System.Collections.Generic;
using System.Drawing.Printing;
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
using MahApps.Metro.Controls.Dialogs;
using Xceed.Wpf.Toolkit;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : MetroWindow
    {
        public Settings()
        {
            InitializeComponent();

            
            // Сохранить объект в локальном файле.

            if (File.Exists(Directory.GetCurrentDirectory() + "/settings.dat"))
            {
                var formatter = new BinaryFormatter();

                using (Stream fStream = File.OpenRead("settings.dat"))
                {
                    var s = formatter.Deserialize(fStream) as AppSettings;
                    tbРуководитель.Text = s.РуководительОрганизации;
                    tbДолжностьРуководителя.Text = s.ДолжностьРуководителя;
                    tbНазваниеОрганизации.Text = s.НазваниеОрганизации;
                    tbИнтервалДокументов.Text = s.ИнтервалХраненияДокументов.ToString();
                    tbИнтервалОтчётов.Text = s.ИнтервалХраненияОтчётов.ToString();
                    tbНормаРабочегоДня.Text = s.НормаРабочегоДня;
                }
            }
        }

        private void bUnits_Click(object sender, RoutedEventArgs e)
        {
            this.IsManipulationEnabled = false;
            Units w = new Units();
            w.ShowDialog();
            IsManipulationEnabled = true;
        }

        private void bPositions_Click(object sender, RoutedEventArgs e)
        {
            this.IsManipulationEnabled = false;
            Positions w = new Positions();
            w.ShowDialog();
            IsManipulationEnabled = true;
        }

        private void bStatuses_Click(object sender, RoutedEventArgs e)
        {
            this.IsManipulationEnabled = false;
            Statuses w = new Statuses();
            w.ShowDialog();
            IsManipulationEnabled = true;
        }

        private void bPrintSettings_Click(object sender, RoutedEventArgs e)
        {
            // TODO Окно настройки печати
            
        }

        private async void bClose_Click(object sender, RoutedEventArgs e)
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Да",
                NegativeButtonText = "Нет",
                AnimateShow = true,
                AnimateHide = false
            };

            

            MessageDialogResult result = await this.ShowMessageAsync("Предупреждение","Сохранить настройки?", 
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            if (result == MessageDialogResult.Affirmative)
            {
                AppSettings s = new AppSettings()
                {
                    ДолжностьРуководителя = tbДолжностьРуководителя.Text,
                    НазваниеОрганизации = tbНазваниеОрганизации.Text,
                    РуководительОрганизации = tbРуководитель.Text,
                    НормаРабочегоДня = tbНормаРабочегоДня.Text,
                    ИнтервалХраненияДокументов = tbИнтервалДокументов.Text,
                    ИнтервалХраненияОтчётов = tbИнтервалОтчётов.Text
                };

                var formatter = new BinaryFormatter();
                // Сохранить объект в локальном файле.
                using (Stream fStream = new FileStream("settings.dat",
                   FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    formatter.Serialize(fStream, s);
                }
            }
            
            Close();
        }
    }
}
