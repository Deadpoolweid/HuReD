using System.IO;
using System.Windows;
using Hured.DBModel;

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

            if (Properties.Settings.Default.IsFirstLaunch)
            {
                var w = new Wizard();
                w.ShowDialog();

                Controller.SetConnectionString(Functions.GetAppSettings().GetConnectionString());

                Controller.InitDb();

                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Documents");


                Properties.Settings.Default.IsFirstLaunch = false;
            }

            var settings = Functions.GetAppSettings();

            Controller.SetConnectionString(settings?.GetConnectionString());

            Functions.ChangeTheme(settings?.Theme);
            Functions.ChangeAccent(settings?.Accent);
        }

        public void BEmployees_OnClick(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;
            var w = new Employees();
            w.ShowDialog();
            IsHitTestVisible = true;
        }

        public void bOrders_Click(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;
            var w = new Orders();
            w.ShowDialog();
            IsHitTestVisible = true;
        }

        public void bTimesheet_Click(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;
            var w = new Timesheet();
            w.ShowDialog();
            IsHitTestVisible = true;
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
