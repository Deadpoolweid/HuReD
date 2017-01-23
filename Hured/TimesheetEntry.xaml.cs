using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Логика взаимодействия для TimesheetEntry.xaml
    /// </summary>
    public partial class TimesheetEntry : MetroWindow
    {
        public TimesheetEntry(object[] data)
        {
            InitializeComponent();

            Controller.OpenConnection();

            var statuses = Controller.Select(new Статус(), q => q != null);


            foreach (var status in statuses)
            {
                cbStatus.Items.Add(status.Название);
                StatusesId.Add(status.СтатусId);
            }

            cbStatus.SelectedIndex = 0;

            lDate.Content += " " + data[0];
            lName.Content += " " + data[1];

            if (data.Count()>2)
            {
                var entry = data[2] as ТабельнаяЗапись;
                var editingEntry = Controller.Find(new ТабельнаяЗапись(),
                    q => q.ТабельнаяЗаписьId == entry.ТабельнаяЗаписьId);
                cbStatus.SelectedIndex = StatusesId.IndexOf(editingEntry.Статус.СтатусId);
                tbОтработанныеЧасы.Text = editingEntry.Часы;
                Functions.SetRTBText(rtbпримечание,editingEntry.Примечание);
                editingEntryId = editingEntry.ТабельнаяЗаписьId;
            }

            Controller.CloseConnection();

        }


        private int editingEntryId;

        List<int> StatusesId = new List<int>(); 

        private void bOk_Click(object sender, RoutedEventArgs e)
        {
            Controller.OpenConnection();

            int statusId = StatusesId[cbStatus.SelectedIndex];
            var timesheetEntry = new ТабельнаяЗапись()
            {
                Часы = tbОтработанныеЧасы.Text,
                Примечание = Functions.GetRTBText(rtbпримечание),
                Статус = Controller.Find(new Статус(), q => q.СтатусId ==statusId),
                ТабельнаяЗаписьId = editingEntryId
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
    }
}
