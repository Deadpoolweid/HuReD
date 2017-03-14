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
using Hured.DBModel;
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
                if (!LaunchWizard()) Application.Current.Shutdown(1);

                Controller.SetConnectionString(Functions.GetAppSettings().GetConnectionString());

                Controller.InitDb();

                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Documents");


                appSettings["IsFirstLaunch"] = false.ToString();
            }

            var settings = Functions.GetAppSettings();

            Controller.SetConnectionString(settings?.GetConnectionString());

            Functions.ChangeTheme(settings?.Theme);
            Functions.ChangeAccent(settings?.Accent);

            var buttons = this.FindChildren<Button>();
            foreach (var button in buttons)
            { 
            }

            this.Unloaded += (sender, args) => Environment.Exit(0);

            Grid.SetRowSpan(Loading, mainGrid.RowDefinitions.Count);
            if (mainGrid.ColumnDefinitions.Count < 1) return;
            Grid.SetColumnSpan(Loading, mainGrid.ColumnDefinitions.Count);


            

        }

        private bool LaunchWizard()
        {
            var w = new Wizard();
            w.ShowDialog();
            return w.IsFinished;
        }

        public void BEmployees_OnClick(object sender, RoutedEventArgs e)
        {
            //Functions.AddProgressRing(this);

            Loading.IsActive = true;

            if (!Controller.IsConnectionSucceded())
            {
                Functions.ShowPopup(BEmployees, "Не удаётся подключиться к базе данных. " +
                                               "Проверьте настройки подключения.");
                //Functions.RemoveProgressRing();
                return;
            }




            IsHitTestVisible = false;
            var w = new Employees();
            w.ShowDialog();
            IsHitTestVisible = true;

            Loading.IsActive = false;

            //Functions.RemoveProgressRing();
        }

        public void bOrders_Click(object sender, RoutedEventArgs e)
        {
            Functions.AddProgressRing(this);


            if (!Controller.IsConnectionSucceded())
            {
                Functions.ShowPopup(BEmployees, "Не удаётся подключиться к базе данных. " +
                                               "Проверьте настройки подключения.");
                return;
            }

            IsHitTestVisible = false;
            var w = new Orders();
            w.ShowDialog();
            IsHitTestVisible = true;

            Functions.RemoveProgressRing();

        }

        public void bTimesheet_Click(object sender, RoutedEventArgs e)
        {

            Functions.AddProgressRing(this);

            if (!Controller.IsConnectionSucceded())
            {
                Functions.ShowPopup(BEmployees, "Не удаётся подключиться к базе данных. " +
                                               "Проверьте настройки подключения.");
                return;
            }

            IsHitTestVisible = false;
            var w = new Timesheet();
            w.ShowDialog();
            IsHitTestVisible = true;

            Functions.RemoveProgressRing();

        }

        private void bSettings_Click(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;
            var w = new Settings();
            w.ShowDialog();
            IsHitTestVisible = true;
        }
    }
}
