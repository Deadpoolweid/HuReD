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
using ListBox = System.Windows.Forms.ListBox;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Units.xaml
    /// </summary>
    public partial class Units : MetroWindow
    {
        public Units()
        {
            InitializeComponent();

            Functions.AddUnitsFromDB(ref lbUnits);

            tResult = new TransactionResult();
        }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            lbUnits.Items.Add(tbNewUnit.Text);
            Controller.OpenConnection();
            Controller.Insert(new Подразделение() {Название = tbNewUnit.Text});
            Controller.CloseConnection();
            tResult.RecordsAdded++;
        }

        private TransactionResult tResult;

        private bool EditModeEnabled = false;

        private string oldValue;

        private void bChange_Click(object sender, RoutedEventArgs e)
        {
            if (!EditModeEnabled)
            {
                tbNewUnit.Text = (lbUnits.SelectedItem as ListBoxItem).Content.ToString();
                lbUnits.IsHitTestVisible = bAdd.IsHitTestVisible = bRemove.IsHitTestVisible =
                    bClose.IsHitTestVisible = false;
                oldValue = tbNewUnit.Text;
                bChange.Content = "Ok";
            }
            else
            {
                oldValue = (lbUnits.Items[lbUnits.SelectedIndex] as ListBoxItem).Content.ToString();
                Controller.OpenConnection();
                Controller.Edit(q => q.Название == oldValue,new Подразделение() {Название = tbNewUnit.Text});
                Controller.CloseConnection();
                lbUnits.Items[lbUnits.SelectedIndex] = new ListBoxItem()
                {
                    Content = tbNewUnit.Text,
                    Tag = (lbUnits.Items[lbUnits.SelectedIndex] as ListBoxItem).Tag
                };
                lbUnits.IsHitTestVisible = bAdd.IsHitTestVisible = bRemove.IsHitTestVisible =
                    bClose.IsHitTestVisible = true;
                bChange.Content = "Изменить";

                tResult.RecordsChanged++;
            }
            EditModeEnabled = !EditModeEnabled;
        }

        private void bRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lbUnits.SelectedIndex == -1)
            {
                return;
            }
            Controller.OpenConnection();
            Controller.Remove(new Подразделение(), q => q.Название == lbUnits.SelectedValue.ToString());
            Controller.CloseConnection();
            lbUnits.Items.RemoveAt(lbUnits.SelectedIndex);

            tResult.RecordsDeleted++;
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Close();

            Controller.OpenConnection();
            tResult.RecordsCount = Controller.RecordsCount<Подразделение>();
            Controller.CloseConnection();

            Tag = tResult;
        }
    }
}
