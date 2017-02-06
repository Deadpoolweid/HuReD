using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Hured.DBModel;
using Hured.Tables_templates;

namespace Hured
{
    public class ItemVm : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public ItemVm(string text, Color color)
        {
            Text = text;
            BackgroundColor = color;
        }

        private string _text;
        private Color _backgroundColor;

        public string Text
        {
            set { SetField(ref _text, value, "Text"); }
            get { return _text; }
        }

        public Color BackgroundColor
        {
            set { SetField(ref _backgroundColor, value, "BackgroundColor"); }
            get { return _backgroundColor; }
        }
    }

    /// <summary>
    /// Логика взаимодействия для Statuses.xaml
    /// </summary>
    public partial class Statuses
    {
        public Statuses()
        {
            InitializeComponent();
            Items = new List<ItemVm>();

            LvStatuses.ItemsSource = Items;

            SyncStatuses();
        }

        readonly TransactionResult _tResult = new TransactionResult();

        readonly List<int> _statusesId = new List<int>();

        public List<ItemVm> Items { get; set; }

        void SyncStatuses()
        {
            Items.Clear();
            _statusesId.Clear();

            Controller.OpenConnection();

            var statuses = Controller.Select<Статус>( q => q != null);

            foreach (var status in statuses)
            {
                _statusesId.Add(status.СтатусId);


                var r = Convert.ToByte(status.Цвет.Split(' ')[0]);
                var g = Convert.ToByte(status.Цвет.Split(' ')[1]);
                var b = Convert.ToByte(status.Цвет.Split(' ')[2]);


                Items.Add(new ItemVm(status.Название, Color.FromRgb(r, g, b)));

            }

            Controller.CloseConnection();

            LvStatuses.Items.Refresh();
        }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;

            var w = new Status();
            w.ShowDialog();

            _tResult.RecordsAdded++;

            SyncStatuses();
            IsHitTestVisible = true;
        }

        private void bChange_Click(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;

            var id = _statusesId[LvStatuses.SelectedIndex];
            Controller.OpenConnection();
            var w = new Status(Controller.Find<Статус>(q => q.СтатусId == id));
            Controller.CloseConnection();
            w.ShowDialog();

            _tResult.RecordsChanged++;

            SyncStatuses();
            IsHitTestVisible = true;
        }

        private void bRemove_Click(object sender, RoutedEventArgs e)
        {
            Controller.OpenConnection();
            var id = _statusesId[LvStatuses.SelectedIndex];
            Controller.Remove<Статус>(q => q.СтатусId == id);
            Controller.CloseConnection();

            _tResult.RecordsDeleted++;

            SyncStatuses();
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Controller.OpenConnection();
            _tResult.RecordsCount = Controller.RecordsCount<Статус>();
            Controller.CloseConnection();
            Tag = _tResult;
            Close();
        }
    }
}
