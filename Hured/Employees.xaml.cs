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
    /// Логика взаимодействия для Employees.xaml
    /// </summary>
    public partial class Employees : Window
    {
        public Employees()
        {
            InitializeComponent();

            Controller.OpenConnection();


            //List<Сотрудник> Сотрудники = Controller.Select(new Сотрудник(), сотрудник => сотрудник != null);

            List<ОсновнаяИнформация> GeneralInfo = Controller.Select(new ОсновнаяИнформация(), e => e != null);

            lvEmployees.ItemsSource = GeneralInfo;

            Functions.FillTreeView(ref tvUnits);

            //tvUnits.SelectedItemChanged += (sender, args) =>
            //{
            //    Controller.OpenConnection();
            //    lvEmployees.ItemsSource = Controller.Select(new ОсновнаяИнформация(),
            //        e => e.Должность.Подразделение.Название == tvUnits.SelectedItem.ToString());
            //    Controller.CloseConnection();
            //};

            // TODO Заполнение списков
            Controller.CloseConnection();
        }

        private void bAdd_OnClick(object sender, RoutedEventArgs e)
        {
            // TODO Добавление сотрудника
            

            IsManipulationEnabled = false;
            Employee w = new Employee();
            w.ShowDialog();

            if (w.DialogResult == true)
            {
                var employeeToAdd = w.Tag as Сотрудник;
                Controller.OpenConnection();
                Controller.Insert(employeeToAdd);
                Controller.CloseConnection();
            }
            IsManipulationEnabled = true;
        }

        private void BChange_OnClick(object sender, RoutedEventArgs e)
        {
            // TODO Изменение сотрудника
            IsManipulationEnabled = false;
            Employee w = new Employee();
            w.ShowDialog();
            IsManipulationEnabled = true;
        }

        private void bRemove_Click(object sender, RoutedEventArgs e)
        {
            // TODO Удаление сотрудника
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
