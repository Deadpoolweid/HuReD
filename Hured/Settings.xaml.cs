using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Hured.DBModel;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings
    {
        public Settings()
        {
            InitializeComponent();

            cbTheme.ItemsSource = ThemeManager.AppThemes.ToList();
            cbAccent.ItemsSource = ThemeManager.Accents;

            _loadedSettings = Functions.GetAppSettings() ?? new AppSettings();
            TbРуководитель.Text = _loadedSettings.РуководительОрганизации;
            TbДолжностьРуководителя.Text = _loadedSettings.ДолжностьРуководителя;
            TbНазваниеОрганизации.Text = _loadedSettings.НазваниеОрганизации;
            TbНормаРабочегоДня.Text = _loadedSettings.НормаРабочегоДня;
            ChbСтрогаяПроеркаПолей.IsChecked = _loadedSettings.СтрогаяПроверкаПолей;

            cbTheme.SelectedIndex = cbAccent.SelectedIndex = 0;

            cbTheme.SelectedItem = ThemeManager.AppThemes.ToList().FirstOrDefault(
                q => q.Name == _loadedSettings.Theme);
            cbAccent.SelectedItem = ThemeManager.Accents.FirstOrDefault(q => q.Name == _loadedSettings.Accent);
            

            var mySqlConnectionStringBuilder = _loadedSettings.GetConnectionStringBuilder();
            if (mySqlConnectionStringBuilder == null) return;
            TbServer.Text = mySqlConnectionStringBuilder.Server;
            NtbPort.Value = mySqlConnectionStringBuilder.Port;
            TbDatabaseName.Text = mySqlConnectionStringBuilder.Database;
            tbUid.Text = mySqlConnectionStringBuilder.UserID;
            PbPassword.Password = mySqlConnectionStringBuilder.Password;
            ChbPersistSecurityInfo.IsChecked = mySqlConnectionStringBuilder.PersistSecurityInfo;

        }

        private readonly AppSettings _loadedSettings;

        public void bUnits_Click(object sender, RoutedEventArgs e)
        {
            if (!Controller.IsConnectionSucceded())
            {
                Functions.ShowPopup(BUnits, "Не удаётся подключиться к базе данных. " +
                                               "Проверьте настройки подключения.");
                return;
            }

            IsHitTestVisible = false;
            var w = new Units();
            w.ShowDialog();
            IsHitTestVisible = true;
        }

        public void bPositions_Click(object sender, RoutedEventArgs e)
        {

            if (!Controller.IsConnectionSucceded())
            {
                Functions.ShowPopup(BPositions, "Не удаётся подключиться к базе данных. " +
                                               "Проверьте настройки подключения.");
                return;
            }

            IsHitTestVisible = false;
            var w = new Positions();
            w.ShowDialog();
            IsHitTestVisible = true;
        }

        public void bStatuses_Click(object sender, RoutedEventArgs e)
        {

            if (!Controller.IsConnectionSucceded())
            {
                Functions.ShowPopup(BStatuses, "Не удаётся подключиться к базе данных. " +
                                               "Проверьте настройки подключения.");
                return;
            }

            IsHitTestVisible = false;
            var w = new Statuses();
            w.ShowDialog();
            IsHitTestVisible = true;
        }

        private async void bClose_Click(object sender, RoutedEventArgs e)
        {
            if (!Functions.ValidateAllTextboxes(this, ChbСтрогаяПроеркаПолей.IsChecked))
            {
                return;
            }

            if (!Regex.IsMatch(TbРуководитель.Text, "^[А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+$"))
            {
                Functions.ShowPopup(TbРуководитель, "Введите фамилию, имя и отчество через пробелы.");
                return;
            }

            var mySettings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Да",
                NegativeButtonText = "Нет",
                AnimateShow = true,
                AnimateHide = false
            };

            var s = new AppSettings
            {
                ДолжностьРуководителя = TbДолжностьРуководителя.Text,
                НазваниеОрганизации = TbНазваниеОрганизации.Text,
                РуководительОрганизации = TbРуководитель.Text,
                НормаРабочегоДня = TbНормаРабочегоДня.Text,
                СтрогаяПроверкаПолей = ChbСтрогаяПроеркаПолей.IsChecked != null && ChbСтрогаяПроеркаПолей.IsChecked.Value,
                Theme = (cbTheme.SelectedItem as AppTheme).Name,
                Accent = (cbAccent.SelectedItem as Accent).Name
            };

            s.SetConnectionStringBuilder(new MySqlConnectionStringBuilder()
            {
                Server = TbServer.Text,
                Port = (uint)NtbPort.Value,
                Database = TbDatabaseName.Text,
                UserID = tbUid.Text,
                Password = PbPassword.Password,
                PersistSecurityInfo = ChbPersistSecurityInfo.IsChecked.Value
            });

            if (!Equals(_loadedSettings, null))
            {
                if (s == _loadedSettings && s.GetConnectionString() == _loadedSettings.GetConnectionString())
                {
                    Close();
                    return;
                }
            }

            var result = await this.ShowMessageAsync("Предупреждение", "Сохранить настройки?",
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            if (result == MessageDialogResult.Affirmative)
            {
                var formatter = new BinaryFormatter();
                // Сохранить объект в локальном файле.
                using (Stream fStream = new FileStream("settings.dat",
                   FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    formatter.Serialize(fStream, s);
                }

                Functions.ChangeTheme((cbTheme.SelectedItem as AppTheme).Name);
                Functions.ChangeAccent((cbAccent.SelectedItem as Accent).Name);
            }

            Controller.SetConnectionString(s.GetConnectionString());

            Close();
        }


        private void BCheckDbConnection_OnClick(object sender, RoutedEventArgs e)
        {
            var connectionStringBuilder = new MySqlConnectionStringBuilder()
            {
                Server = TbServer.Text,
                Port = (uint) NtbPort.Value,
                UserID = tbUid.Text,
                Password = PbPassword.Password,
                PersistSecurityInfo = ChbPersistSecurityInfo.IsChecked.Value
            };
            if (Controller.IsConnectionSucceded(connectionStringBuilder.ConnectionString))
            {
                BCheckDbConnection.Background = Brushes.ForestGreen;
                Functions.ShowPopup(BCheckDbConnection,"Соединение установлено.");
            }
            else
            {
                BCheckDbConnection.Background = Brushes.Red;
                Functions.ShowPopup(BCheckDbConnection,"Соединение не установлено.");
            }
        }

        private void BExportDb_OnClick(object sender, RoutedEventArgs e)
        {
            string backUpDirectory = Directory.GetCurrentDirectory() + @"\Backup";
            Directory.CreateDirectory(backUpDirectory);

            var sfd = new SaveFileDialog
            {
                InitialDirectory = backUpDirectory,
                Filter = "SQL Query | *.sql | Все файлы (*.*)|*.*",
                FileName = "Backup"
            };

            if (sfd.ShowDialog() == false) return;

            Controller.ExportDataBase(sfd.FileName);
        }

        private void BImportDb_OnClick(object sender, RoutedEventArgs e)
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

            Functions.RemoveProgressRing();
        }

        private async void Settings_OnClosed(object sender, EventArgs e)
        {
            
        }
    }
}
