using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Hured.DataBase;
using Hured.Tables_templates;
using MahApps.Metro.Controls;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Recruitment.xaml
    /// </summary>
    public partial class Recruitment
    {
        public Recruitment(ПриказПриём order = null)
        {
            Closing += Recruitment_OnClosing;

            InitializeComponent();
            Functions.AddUnitsFromDB(ref CbUnit);
            CbUnit.SelectedIndex = CbPosition.SelectedIndex = 0;


            if (order != null)
            {
                DpBegin.Text = order.НачалоРаботы.ToShortDateString();
                DpEnd.Text = order.КонецРаботы.ToShortDateString();
                ChIsTraineship.IsChecked = order.ИспытательныйСрок;
                TbИспытательныйСрокДлительность.Text = order.ИспытательныйСрокДлительность ?? "";
                CbUnit.SelectedItem = order.Должность.Подразделение.Название;
                CbPosition.SelectedItem = order.Должность.Название;
                TbОклад.Text = order.Оклад;
                TbНадбавка.Text = order.Надбавка;
                Functions.SetRtbText(RtbПримечание, order.Примечания);
                TbНомерДоговора.Text = order.НомерТрудовогоДоговора;
                DpДатаДоговора.Text = order.ДатаТрудовогоДоговора.ToShortDateString();

            }

            TbИспытательныйСрокДлительность.IsHitTestVisible = false;
        }



        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void bOk_Click(object sender, RoutedEventArgs e)
        {
            if (this.FindChildren<TextBox>().Where(
                textbox => textbox.Name != "tbИспытательныйСрокДлительность" &&
                           ChIsTraineship.IsChecked != false).Any(Functions.IsEmpty))
            {
                return;
            }


            Controller.OpenConnection();

            var tag = (CbUnit.SelectedItem as ComboBoxItem)?.Tag;
            if (tag != null)
            {
                var unitId = (int)tag;

                var unit = Controller.Find<Подразделение>(q => q.ПодразделениеId == unitId);

                var comboboxItem = (CbPosition.SelectedItem as ComboBoxItem)?.Tag;
                if (comboboxItem != null)
                {
                    var positionId = (int)comboboxItem;

                    var position = Controller.Find<Должность>(q => q.Подразделение.ПодразделениеId == unit.ПодразделениеId
                                                                   && q.ДолжностьId == positionId);

                    var приём = new ПриказПриём
                    {
                        НачалоРаботы = DateTime.Parse(DpBegin.Text),
                        КонецРаботы = DateTime.Parse(DpEnd.Text),
                        ИспытательныйСрок = ChIsTraineship.IsChecked != null && ChIsTraineship.IsChecked.Value,
                        Должность = position,
                        Оклад = TbОклад.Text,
                        Надбавка = TbНадбавка.Text,
                        Примечания = Functions.GetRtbText(RtbПримечание),
                        НомерТрудовогоДоговора = TbНомерДоговора.Text,
                        ДатаТрудовогоДоговора = DateTime.Parse(DpДатаДоговора.Text),
                        ИспытательныйСрокДлительность = TbИспытательныйСрокДлительность.Text
                    };
                    Tag = приём;
                }
            }

            DialogResult = true;
            Close();
        }


        private void Recruitment_OnClosing(object sender, CancelEventArgs e)
        {
            
        }

        private void CbUnit_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CbPosition.Items.Clear();
            var tag = (CbUnit.SelectedItem as ComboBoxItem)?.Tag;
            if (tag != null)
                Functions.AddPositionsFromDB(ref CbPosition, (int)tag);
            CbPosition.SelectedIndex = 0;
        }

        private void ChIsTraineship_OnChecked(object sender, RoutedEventArgs e)
        {
            if (ChIsTraineship.IsChecked != null)
                TbИспытательныйСрокДлительность.IsHitTestVisible = ChIsTraineship.IsChecked.Value;
        }
    }
}
