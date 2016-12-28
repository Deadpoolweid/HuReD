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

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Units.xaml
    /// </summary>
    public partial class Units : Window
    {
        public Units()
        {
            InitializeComponent();

            Functions.AddUnitsFromDB(ref lvUnits);
        }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            lvUnits.Items.Add(tbNewUnit.Text);
            Controller.OpenConnection();
            Controller.Insert(new Подразделение() {Название = tbNewUnit.Text});
            Controller.CloseConnection();
        }

        private bool EditModeEnabled = false;

        private string oldValue;

        private void bChange_Click(object sender, RoutedEventArgs e)
        {
            if (!EditModeEnabled)
            {
                tbNewUnit.Text = lvUnits.SelectedValue.ToString();
                lvUnits.IsEnabled = bAdd.IsEnabled = bRemove.IsEnabled =
                    bClose.IsEnabled = false;
                oldValue = tbNewUnit.Text;
                bChange.Content = "Ok";
            }
            else
            {
                Controller.OpenConnection();
                Controller.Edit(q => q.Название == oldValue,new Подразделение() {Название = tbNewUnit.Text});
                Controller.CloseConnection();
                lvUnits.Items[lvUnits.SelectedIndex] = tbNewUnit.Text;
                lvUnits.IsEnabled = bAdd.IsEnabled = bRemove.IsEnabled =
                    bClose.IsEnabled = true;
                bChange.Content = "Изменить";
            }
            EditModeEnabled = !EditModeEnabled;
        }

        private void bRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lvUnits.SelectedIndex == -1)
            {
                return;
            }
            Controller.OpenConnection();
            Controller.Remove(new Подразделение(), q => q.Название == lvUnits.SelectedValue.ToString());
            Controller.CloseConnection();
            lvUnits.Items.RemoveAt(lvUnits.SelectedIndex);

        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
