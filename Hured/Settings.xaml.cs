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

            LoadedSettings = Functions.GetAppSettings() ?? new AppSettings();
            tbРуководитель.Text = LoadedSettings.РуководительОрганизации;
            tbДолжностьРуководителя.Text = LoadedSettings.ДолжностьРуководителя;
            tbНазваниеОрганизации.Text = LoadedSettings.НазваниеОрганизации;
            tbНормаРабочегоДня.Text = LoadedSettings.НормаРабочегоДня;
                
        }

        private AppSettings LoadedSettings;

        public void bUnits_Click(object sender, RoutedEventArgs e)
        {
            this.IsHitTestVisible = false;
            Units w = new Units();
            w.ShowDialog();
            IsHitTestVisible = true;
        }

        public void bPositions_Click(object sender, RoutedEventArgs e)
        {
            this.IsHitTestVisible = false;
            Positions w = new Positions();
            w.ShowDialog();
            IsHitTestVisible = true;
        }

        public void bStatuses_Click(object sender, RoutedEventArgs e)
        {
            this.IsHitTestVisible = false;
            Statuses w = new Statuses();
            w.ShowDialog();
            IsHitTestVisible = true;
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

            AppSettings s = new AppSettings()
            {
                ДолжностьРуководителя = tbДолжностьРуководителя.Text,
                НазваниеОрганизации = tbНазваниеОрганизации.Text,
                РуководительОрганизации = tbРуководитель.Text,
                НормаРабочегоДня = tbНормаРабочегоДня.Text,
            };

            if (!Equals(LoadedSettings,null))
            {
                if (s == LoadedSettings)
                {
                    Close();
                }
            }

            MessageDialogResult result = await this.ShowMessageAsync("Предупреждение","Сохранить настройки?", 
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            if (result == MessageDialogResult.Affirmative)
            {
                if (this.FindChildren<TextBox>().Any(Functions.IsEmpty))
                {
                    return;
                }


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
