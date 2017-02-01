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
using MahApps.Metro.Controls;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Position.xaml
    /// </summary>
    public partial class Position : MetroWindow
    {
        public Position(Должность position = null)
        {
            InitializeComponent();
            Functions.AddUnitsFromDB(ref cbUnit);
            cbUnit.SelectedIndex = 0;
            if (position != null)
            {
                IsEditMode = true;
                tbName.Text = oldName = position.Название;
                tbРасписание.Text = position.Расписание;
                cbUnit.SelectedItem = position.Подразделение.Название;
            }
        }

        private bool IsEditMode = false;
        private string oldName;

        private void bOk_Click(object sender, RoutedEventArgs e)
        {
            if (this.FindChildren<TextBox>().Any(Functions.IsEmpty))
            {
                return;
            }


            Controller.OpenConnection();

            var unitId = (int) (cbUnit.SelectedItem as ComboBoxItem).Tag;

            var unit = Controller.Select(new Подразделение(),
                q => q.ПодразделениеId == unitId).FirstOrDefault();

            var position = new Должность()
            {
                Название = tbName.Text,
                Расписание = tbРасписание.Text,
                Подразделение = unit
            };


            if (IsEditMode)
            {
                Controller.Edit(q => q.Название == oldName, position);
            }
            else
            {
                Controller.Insert(position);

            }
            Controller.CloseConnection();


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
