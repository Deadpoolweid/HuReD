using System.Windows;
using System.Windows.Controls;
using Hured.DBModel;
using Hured.Tables_templates;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Positions.xaml
    /// </summary>
    public partial class Positions
    {
        public Positions()
        {
            InitializeComponent();
            LbUnits.Items.Add("Все подразделения");
            Functions.AddUnitsFromDB(ref LbUnits);
            LbUnits.SelectedIndex = 0;
        }

        private readonly TransactionResult _tResult = new TransactionResult();

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;

            var w = new Position();
            w.ShowDialog();

            IsHitTestVisible = true;

            _tResult.RecordsAdded++;

            SyncPositions();
        }

        private void bChange_Click(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;

            var w = new Position(LvPositions.SelectedItem as Должность);
            w.ShowDialog();

            IsHitTestVisible = true;

            _tResult.RecordsChanged++;

            SyncPositions();
        }

        private void bRemove_Click(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;

            Controller.OpenConnection();

            var названиеДолжности = (LvPositions.SelectedItem as Должность)?.Название;


            Controller.Remove(new Должность(),
                q => q.Название == названиеДолжности);
            Controller.CloseConnection();

            _tResult.RecordsDeleted++;

            IsHitTestVisible = true;
            SyncPositions();
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Controller.OpenConnection();
            _tResult.RecordsCount = Controller.RecordsCount<Должность>();
            Controller.CloseConnection();


            Tag = _tResult;
            Close();
        }

        private void SyncPositions()
        {
            LvPositions.Items.Clear();

            if (LbUnits.SelectedIndex == 0)
            {
                Functions.AddPositionsFromDB(ref LvPositions);
            }
            else
            {
                var tag = (LbUnits.SelectedItem as ListBoxItem)?.Tag;
                if (tag != null)
                    Functions.AddPositionsFromDB(ref LvPositions,
                        (int)tag);
            }
        }
    }
}