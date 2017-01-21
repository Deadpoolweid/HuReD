using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using Hured.DBModel;
using Hured.Tables_templates;
using MahApps.Metro.Controls;

namespace Hured
{
    public class ItemVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public ItemVM(string text, Color color)
        {
            Text = text;
            BackgroundColor = color;
        }

        private string text;
        private Color backgroundColor;

        public string Text
        {
            set { SetField(ref text, value, "Text"); }
            get { return text; }
        }

        public Color BackgroundColor
        {
            set { SetField(ref backgroundColor, value, "BackgroundColor"); }
            get { return backgroundColor; }
        }
    }

    /// <summary>
    /// Логика взаимодействия для Statuses.xaml
    /// </summary>
    public partial class Statuses : MetroWindow
    {
        public Statuses()
        {
            InitializeComponent();
            Items = new List<ItemVM>();

            lvStatuses.ItemsSource = Items;

            SyncStatuses();
        }

        List<int> StatusesId = new List<int>();

        public List<ItemVM> Items { get; set; }

        void SyncStatuses()
        {
            Items.Clear();
            StatusesId.Clear();

            Controller.OpenConnection();

            var statuses = Controller.Select(new Статус(), q => q != null);

            var i = 0;

            foreach (var status in statuses)
            {
                StatusesId.Add(status.СтатусId);


                byte r = Convert.ToByte(status.Цвет.Split(' ')[0]);
                byte g = Convert.ToByte(status.Цвет.Split(' ')[1]);
                byte b = Convert.ToByte(status.Цвет.Split(' ')[2]);


                Items.Add(new ItemVM(status.Название, Color.FromRgb(r, g, b)));


                i++;
            }

            Controller.CloseConnection();

            lvStatuses.Items.Refresh();
        }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            // TODO Вызвать окно добавления статуса
            IsEnabled = false;

            var w = new Status();
            w.ShowDialog();


            SyncStatuses();
            IsEnabled = true;
        }

        private void bChange_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            // TODO Вызвать окно изменения статуса



            int id = StatusesId[lvStatuses.SelectedIndex];
            Controller.OpenConnection();
            var w = new Status(Controller.Find(new Статус(),
    q => q.СтатусId == id));
            Controller.CloseConnection();
            w.ShowDialog();

            SyncStatuses();
            IsEnabled = true;
        }

        private void bRemove_Click(object sender, RoutedEventArgs e)
        {
            // TODO Удаление статуса
            Controller.OpenConnection();
            int id = StatusesId[lvStatuses.SelectedIndex];
            Controller.Remove(new Статус(),
                q => q.СтатусId == id);
            Controller.CloseConnection();

            SyncStatuses();
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
