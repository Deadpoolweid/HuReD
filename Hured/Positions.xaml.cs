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
    /// Логика взаимодействия для Positions.xaml
    /// </summary>
    public partial class Positions : Window
    {
        public Positions()
        {
            InitializeComponent();
            lvUnits.Items.Add("Все должности");
            Functions.AddUnitsFromDB(ref lvUnits);
            lvUnits.SelectedIndex = 0;
            // TODO Заполнение списка подразделений
        }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            // TODO Вызвать окно добавления должности
            IsEnabled = false;

            Position w = new Position();
            w.ShowDialog();

            if (w.DialogResult == true)
            {
                var position = w.Tag as Должность;
                Controller.OpenConnection();
                Controller.Insert(position);
                Controller.CloseConnection();
            }

            IsEnabled = true;
            SyncPositions();
        }

        private void bChange_Click(object sender, RoutedEventArgs e)
        {
            // TODO Вызвать окно изменения должности
        }

        private void bRemove_Click(object sender, RoutedEventArgs e)
        {
            // TODO Удаление должности
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LvUnits_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SyncPositions();
        }

        // TODO Возможно общая функция
        private void SyncPositions()
        {
            lvPositions.Items.Clear();

            Controller.OpenConnection();
            foreach (var position in Controller.Select(new Должность(), q => q.Подразделение.Название == lvUnits.SelectedValue.ToString()))
            {
                lvPositions.Items.Add(position);
            }
            Controller.CloseConnection();
        }
    }
}
