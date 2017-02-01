using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

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

            _loadedSettings = Functions.GetAppSettings() ?? new AppSettings();
            TbРуководитель.Text = _loadedSettings.РуководительОрганизации;
            TbДолжностьРуководителя.Text = _loadedSettings.ДолжностьРуководителя;
            TbНазваниеОрганизации.Text = _loadedSettings.НазваниеОрганизации;
            TbНормаРабочегоДня.Text = _loadedSettings.НормаРабочегоДня;

        }

        private readonly AppSettings _loadedSettings;

        public void bUnits_Click(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;
            var w = new Units();
            w.ShowDialog();
            IsHitTestVisible = true;
        }

        public void bPositions_Click(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;
            var w = new Positions();
            w.ShowDialog();
            IsHitTestVisible = true;
        }

        public void bStatuses_Click(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;
            var w = new Statuses();
            w.ShowDialog();
            IsHitTestVisible = true;
        }

        private async void bClose_Click(object sender, RoutedEventArgs e)
        {
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
                НормаРабочегоДня = TbНормаРабочегоДня.Text
            };

            if (!Equals(_loadedSettings, null))
            {
                if (s == _loadedSettings)
                {
                    Close();
                }
            }

            var result = await this.ShowMessageAsync("Предупреждение", "Сохранить настройки?",
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
