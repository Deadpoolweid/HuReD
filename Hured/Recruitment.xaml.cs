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
    /// Логика взаимодействия для Recruitment.xaml
    /// </summary>
    public partial class Recruitment : MetroWindow
    {
        public Recruitment(ПриказПриём order = null)
        {
            InitializeComponent();
            Functions.AddUnitsFromDB(ref cbUnit);
            cbUnit.SelectedIndex = cbPosition.SelectedIndex = 0;


            if (order != null)
            {
                dpBegin.Text = order.НачалоРаботы.ToShortDateString();
                dpEnd.Text = order.КонецРаботы.ToShortDateString();
                chIsTraineship.IsChecked = order.ИспытательныйСрок;
                tbИспытательныйСрокДлительность.Text = order.ИспытательныйСрокДлительность ?? "";
                cbUnit.SelectedItem = order.Должность.Подразделение.Название;
                cbPosition.SelectedItem = order.Должность.Название;
                tbОклад.Text = order.Оклад;
                tbНадбавка.Text = order.Надбавка;
                Functions.SetRTBText(rtbПримечание, order.Примечания);
                tbНомерДоговора.Text = order.НомерТрудовогоДоговора;
                dpДатаДоговора.Text = order.ДатаТрудовогоДоговора.ToShortDateString();

                filePath = order.Файл;
            }

            tbИспытательныйСрокДлительность.IsEnabled = false;
        }

        private string filePath;

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void bOk_Click(object sender, RoutedEventArgs e)
        {

            Controller.OpenConnection();

            var unit = Controller.Find(new Подразделение(),
                q => q.Название == cbUnit.SelectedValue.ToString());

            var positionName = cbPosition.SelectedValue.ToString();

            var position = Controller.Find(new Должность(),
                q => q.Подразделение.ПодразделениеId == unit.ПодразделениеId
                && q.Название == positionName);

            var приём = new ПриказПриём()
            {
                НачалоРаботы = dpBegin.DisplayDate,
                КонецРаботы = dpEnd.DisplayDate,
                ИспытательныйСрок = chIsTraineship.IsChecked.Value,
                Должность = position,
                Оклад = tbОклад.Text,
                Надбавка = tbНадбавка.Text,
                Примечания = Functions.GetRTBText(rtbПримечание),
                НомерТрудовогоДоговора = tbНомерДоговора.Text,
                ДатаТрудовогоДоговора = dpДатаДоговора.DisplayDate,
                ИспытательныйСрокДлительность = tbИспытательныйСрокДлительность.Text,
                Файл = filePath
            };
            Tag = приём;

            DialogResult = true;
            Close();
        }

        private void CbUnit_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbPosition.Items.Clear();
            Functions.AddPositionsFromDB(ref cbPosition, cbUnit.SelectedValue.ToString());
            cbPosition.SelectedIndex = 0;
        }

        private void ChIsTraineship_OnChecked(object sender, RoutedEventArgs e)
        {
            tbИспытательныйСрокДлительность.IsEnabled = chIsTraineship.IsChecked.Value;
        }
    }
}
