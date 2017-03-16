using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Hured.DataBase;
using Hured.Tables_templates;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для TimesheetEntry.xaml
    /// </summary>
    public partial class TimesheetEntry
    {
        public TimesheetEntry(object[] data)
        {
            InitializeComponent();

            Controller.OpenConnection();

            var statuses = Controller.Select<Статус>(q => q != null);


            foreach (var status in statuses)
            {
                CbStatus.Items.Add(status.Название);
                _statusesId.Add(status.СтатусId);
            }

            CbStatus.SelectedIndex = 0;

            LDate.Content += " " + data[0];
            LName.Content += " " + data[1];

            if (data.Length > 2)
            {
                var entry = data[2] as ТабельнаяЗапись;
                var editingEntry = Controller.Find<ТабельнаяЗапись>(
                    q => q.ТабельнаяЗаписьId == entry.ТабельнаяЗаписьId);
                CbStatus.SelectedIndex = _statusesId.IndexOf(editingEntry.Статус.СтатусId);
                TbОтработанныеЧасы.Text = editingEntry.Часы;
                Functions.SetRtbText(Rtbпримечание, editingEntry.Примечание);
                _editingEntryId = editingEntry.ТабельнаяЗаписьId;
            }

            Controller.CloseConnection();

        }


        private readonly int _editingEntryId;

        readonly List<int> _statusesId = new List<int>();

        private void bOk_Click(object sender, RoutedEventArgs e)
        {
            Controller.OpenConnection();

            var statusId = _statusesId[CbStatus.SelectedIndex];
            var timesheetEntry = new ТабельнаяЗапись
            {
                Часы = TbОтработанныеЧасы.Text,
                Примечание = Functions.GetRtbText(Rtbпримечание),
                Статус = Controller.Find<Статус>( q => q.СтатусId == statusId),
                ТабельнаяЗаписьId = _editingEntryId
            };

            Tag = timesheetEntry;
            DialogResult = true;
            Close();
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void TimesheetEntry_OnClosing(object sender, CancelEventArgs e)
        {
        }
    }
}
