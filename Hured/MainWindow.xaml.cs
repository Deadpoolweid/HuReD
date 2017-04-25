using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Hured.DataBase;
using Hured.Modules;
using Hured.Modules.Module_Auth;
using Hured.Tables_templates;
using MahApps.Metro.Controls;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var appSettings = ConfigurationManager.AppSettings;

            if (bool.Parse(appSettings["IsFirstLaunch"]))
            {
                // Если был произведён выход из мастера настройки
                if (!LaunchWizard()) Application.Current.Shutdown(1);

                Controller.SetConnectionString(Functions.GetAppSettings().GetConnectionString());

                Controller.InitDb();

                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Documents");

                AuthController.AddUser(new УчётнаяЗапись()
                {
                    Login = "admin",
                    Password = "",
                    IsAdmin = true
                });

                appSettings["IsFirstLaunch"] = false.ToString();
            }

            var settings = Functions.GetAppSettings();

            Controller.SetConnectionString(settings?.GetConnectionString());

            Functions.ChangeTheme(settings?.Theme);
            Functions.ChangeAccent(settings?.Accent);

            var w = new Auth();
            w.ShowDialog();

            if (w.DialogResult != true)
            {
                Environment.Exit(0);
            }

            Controller.ConnectionOpened += () =>
            {
                Controller.Insert(new Session(AuthController.CurrentUser, DateTime.Now, UserStatus.Working));
            };

            Controller.ConnectionClosing += () =>
            {
                Controller.Insert(new Session(AuthController.CurrentUser, DateTime.Now, UserStatus.Free));
            };

            this.Unloaded += (sender, args) => Environment.Exit(0);

            DocumentsTypeDictionary.AddDocumentType<ПриказПриём>(OrderType.Recruitment);
            DocumentsTypeDictionary.AddDocumentType<ПриказУвольнение>(OrderType.Dismissal);
            DocumentsTypeDictionary.AddDocumentType<ПриказОтпуск>(OrderType.Vacation);
            DocumentsTypeDictionary.AddDocumentType<ПриказКомандировка>(OrderType.BusinessTrip);

            Grid.SetRowSpan(Loading, mainGrid.RowDefinitions.Count);
            if (mainGrid.ColumnDefinitions.Count < 1) return;
            Grid.SetColumnSpan(Loading, mainGrid.ColumnDefinitions.Count);
        }

        private static bool LaunchWizard()
        {
            var w = new Wizard();
            w.ShowDialog();
            return w.IsFinished;
        }

        private bool CheckConnection()
        {
            if (!Controller.IsConnectionSucceded())
            {
                Functions.ShowPopup(BEmployees, "Не удаётся подключиться к базе данных. " +
                                               "Проверьте настройки подключения.");
                return false;
            }

            return true;
        }

        private void BEmployees_OnClick(object sender, RoutedEventArgs e)
        {
            Loading.IsActive = true;

            if (!CheckConnection()) return;

            IsHitTestVisible = false;
            var w = new Employees();
            w.ShowDialog();
            IsHitTestVisible = true;

            Loading.IsActive = false;
        }

        private void bOrders_Click(object sender, RoutedEventArgs e)
        {
            Loading.IsActive = true;

            if (!CheckConnection()) return;

            IsHitTestVisible = false;
            var w = new Orders();
            w.ShowDialog();
            IsHitTestVisible = true;

            Loading.IsActive = false;
        }

        private void bTimesheet_Click(object sender, RoutedEventArgs e)
        {

            Loading.IsActive = true;

            if (!CheckConnection()) return;

            IsHitTestVisible = false;
            var w = new Timesheet();
            w.ShowDialog();
            IsHitTestVisible = true;

            Loading.IsActive = false;
        }

        private void bSettings_Click(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;
            var w = new Settings();
            w.ShowDialog();
            IsHitTestVisible = true;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
