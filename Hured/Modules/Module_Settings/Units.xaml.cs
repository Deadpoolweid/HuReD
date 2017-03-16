using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Hured.DataBase;
using Hured.Tables_templates;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Units.xaml
    /// </summary>
    public partial class Units
    {
        public Units()
        {
            InitializeComponent();

            Functions.AddUnitsFromDB(ref LbUnits);

            _editModeEnabled = false;

            _tResult = new TransactionResult();
        }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Controller.OpenConnection();
                Controller.Insert(new Подразделение {Название = TbNewUnit.Text});
                Controller.CloseConnection();

                Functions.AddUnitsFromDB(ref LbUnits);

                _tResult.RecordsAdded++;
            }
            catch (System.Exception ex)
            {
                Functions.ShowPopup(sender as Button, "Не удалось добавить новое подразделение.");
            }
            finally
            {
                Controller.CloseConnection(true);
            }
        }

        private readonly TransactionResult _tResult;

        private bool _editModeEnabled;

        private string _oldValue;

        private void bChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_editModeEnabled)
                {
                    TbNewUnit.Text = (LbUnits.SelectedItem as ListBoxItem)?.Content.ToString();
                    LbUnits.IsHitTestVisible = BAdd.IsHitTestVisible = BRemove.IsHitTestVisible =
                        BClose.IsHitTestVisible = false;
                    _oldValue = TbNewUnit.Text;
                    BChange.Content = "Ok";
                }
                else
                {
                    _oldValue = (LbUnits.Items[LbUnits.SelectedIndex] as ListBoxItem)?.Content.ToString();
                    Controller.OpenConnection();
                    Controller.Edit(q => q.Название == _oldValue, new Подразделение {Название = TbNewUnit.Text});
                    Controller.CloseConnection();
                    LbUnits.Items[LbUnits.SelectedIndex] = new ListBoxItem
                    {
                        Content = TbNewUnit.Text,
                        Tag = (LbUnits.Items[LbUnits.SelectedIndex] as ListBoxItem)?.Tag
                    };
                    LbUnits.IsHitTestVisible = BAdd.IsHitTestVisible = BRemove.IsHitTestVisible =
                        BClose.IsHitTestVisible = true;
                    BChange.Content = "Изменить";

                    _tResult.RecordsChanged++;
                }
                _editModeEnabled = !_editModeEnabled;
            }
            catch (System.Exception ex)
            {
                Functions.ShowPopup(sender as Button, "Не удалось изменить подразделение. Окно будет закрыто.");
                Close();

            }
            finally
            {
                Controller.CloseConnection(true);
            }
        }

        private void bRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LbUnits.SelectedIndex == -1)
                {
                    return;
                }

                var listBoxItem = LbUnits.SelectedItem as ListBoxItem;
                if (listBoxItem != null)
                {
                    int index = (int) listBoxItem.Tag;

                    Controller.OpenConnection();
                    Controller.Remove<Подразделение>(q => q.ПодразделениеId == index);
                    Controller.CloseConnection();

                    LbUnits.Items.RemoveAt(LbUnits.SelectedIndex);
                }
                _tResult.RecordsDeleted++;
            }
            catch (System.Exception ex)
            {
                Functions.ShowPopup(sender as Button, "Не удалось удалить подразделение");
            }
            finally
            {
                Controller.CloseConnection(true);
            }
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Units_OnClosing(object sender, CancelEventArgs e)
        {
                Controller.OpenConnection();
                _tResult.RecordsCount = Controller.RecordsCount<Подразделение>();
                Controller.CloseConnection();

                Tag = _tResult;
        }
    }
}
