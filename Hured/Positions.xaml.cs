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
            lvUnits.Items.Add("Все должности");
            Functions.AddUnitsFromDB(ref lvUnits);
            lvUnits.SelectedIndex = 0;
        }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            Position w = new Position();
            w.ShowDialog();

            IsEnabled = true;
            SyncPositions();
        }

        private void bChange_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            Position w = new Position(lvPositions.SelectedItem as Должность);
            w.ShowDialog();

            IsEnabled = true;
            SyncPositions();
        }

        private void bRemove_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            Controller.OpenConnection();

            string НазваниеДолжности = (lvPositions.SelectedItem as Должность).Название;
                

            Controller.Remove(new Должность(), 
                q => q.Название == НазваниеДолжности);
            Controller.CloseConnection();

            IsEnabled = true;
            SyncPositions();
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LvUnits_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SyncPositions();
        }

        private void SyncPositions()
        {
            lvPositions.Items.Clear();

            if (lvUnits.SelectedIndex == 0)
            {
                Functions.AddPositionsFromDB(ref lvPositions);
            }
            else
            {
                Functions.AddPositionsFromDB(ref lvPositions,
                    lvUnits.SelectedValue.ToString());
            }
        }
    }
}
