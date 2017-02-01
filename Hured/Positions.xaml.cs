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
    /// Логика взаимодействия для Positions.xaml
    /// </summary>
    public partial class Positions : MetroWindow
    {
        public Positions()
        {
            InitializeComponent();
            lbUnits.Items.Add("Все подразделения");
            Functions.AddUnitsFromDB(ref lbUnits);
            lbUnits.SelectedIndex = 0;
        }

        private TransactionResult tResult = new TransactionResult();

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;

            Position w = new Position();
            w.ShowDialog();

            IsHitTestVisible = true;

            tResult.RecordsAdded++;

            SyncPositions();
        }

        private void bChange_Click(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;

            Position w = new Position(lvPositions.SelectedItem as Должность);
            w.ShowDialog();

            IsHitTestVisible = true;

            tResult.RecordsChanged++;

            SyncPositions();
        }

        private void bRemove_Click(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;

            Controller.OpenConnection();

            string НазваниеДолжности = (lvPositions.SelectedItem as Должность).Название;
                

            Controller.Remove(new Должность(), 
                q => q.Название == НазваниеДолжности);
            Controller.CloseConnection();

            tResult.RecordsDeleted++;

            IsHitTestVisible = true;
            SyncPositions();
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Controller.OpenConnection();
            tResult.RecordsCount = Controller.RecordsCount<Должность>();
            Controller.CloseConnection();


            Tag = tResult;
            Close();
        }

        private void LvUnits_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SyncPositions();
        }

        private void SyncPositions()
        {
            lvPositions.Items.Clear();

            if (lbUnits.SelectedIndex == 0)
            {
                Functions.AddPositionsFromDB(ref lvPositions);
            }
            else
            {
                Functions.AddPositionsFromDB(ref lvPositions,
                    (int)(lbUnits.SelectedItem as ListBoxItem).Tag);
            }
        }
    }
}