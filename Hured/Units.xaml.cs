using System.Windows;
using System.Windows.Controls;
using Hured.DBModel;
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
            LbUnits.Items.Add(TbNewUnit.Text);
            Controller.OpenConnection();
            Controller.Insert(new Подразделение { Название = TbNewUnit.Text });
            Controller.CloseConnection();
            _tResult.RecordsAdded++;
        }

        private readonly TransactionResult _tResult;

        private bool _editModeEnabled;

        private string _oldValue;

        private void bChange_Click(object sender, RoutedEventArgs e)
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
                Controller.Edit(q => q.Название == _oldValue, new Подразделение { Название = TbNewUnit.Text });
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

        private void bRemove_Click(object sender, RoutedEventArgs e)
        {
            if (LbUnits.SelectedIndex == -1)
            {
                return;
            }
            Controller.OpenConnection();
            Controller.Remove(new Подразделение(), q => q.Название == LbUnits.SelectedValue.ToString());
            Controller.CloseConnection();
            LbUnits.Items.RemoveAt(LbUnits.SelectedIndex);

            _tResult.RecordsDeleted++;
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Close();

            Controller.OpenConnection();
            _tResult.RecordsCount = Controller.RecordsCount<Подразделение>();
            Controller.CloseConnection();

            Tag = _tResult;
        }
    }
}
