using System;
using System.Collections;
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
using Hured.Tables_templates;
using MahApps.Metro.Controls;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Employees.xaml
    /// </summary>
    public partial class Employees : MetroWindow
    {
        public Employees()
        {
            InitializeComponent();

            SyncEmployeesList();
            Functions.FillTreeView(ref tvUnits);
            // TODO Фильтрация сотрудников по должностям и подразделениям
        }


        private List<int> employeesId = new List<int>();

        void SyncEmployeesList(List<Сотрудник> Сотрудники = null)
        {
            lvEmployees.Items.Clear();
            employeesId.Clear();

            if (Сотрудники == null)
            {
                Controller.OpenConnection();
                Сотрудники = Controller.Select(new Сотрудник(), e => e != null);
                Controller.CloseConnection();
            }


            foreach (var сотрудник in Сотрудники)
            {
                lvEmployees.Items.Add(сотрудник.ОсновнаяИнформация);
                employeesId.Add(сотрудник.СотрудникId);
            }
        }

        private void bAdd_OnClick(object sender, RoutedEventArgs e)
        {
            IsManipulationEnabled = false;
            Employee w = new Employee();
            w.ShowDialog();

            SyncEmployeesList();

            IsManipulationEnabled = true;
        }

        private void BChange_OnClick(object sender, RoutedEventArgs e)
        {
            IsManipulationEnabled = false;
            int index = employeesId[lvEmployees.SelectedIndex];
            Controller.OpenConnection();
            var employee = Controller.Select(new Сотрудник(),
                q => q.СотрудникId == index).FirstOrDefault();
            Controller.CloseConnection();

            Employee w = new Employee(employee);

            w.ShowDialog();

            IsManipulationEnabled = true;
            SyncEmployeesList();
        }

        private void bRemove_Click(object sender, RoutedEventArgs e)
        {
            Controller.OpenConnection();

            int index = employeesId[lvEmployees.SelectedIndex];

            var employee = Controller.Find(new Сотрудник(), q => q.СотрудникId == index);

            Controller.Remove(new ОсновнаяИнформация(), 
                q => q.ОсновнаяИнформацияId == employee.ОсновнаяИнформация.ОсновнаяИнформацияId);
            Controller.Remove(new УдостоверениеЛичности(), 
                q => q.УдостоверениеЛичностиId == employee.УдостоверениеЛичности.УдостоверениеЛичностиId);
            Controller.Remove(new ВоинскийУчёт(), 
                q => q.ВоинскийУчётId == employee.ВоинскийУчёт.ВоинскийУчётId);

            var educationsId = new List<int>();
            foreach (var образование in employee.Образование)
            {
                educationsId.Add(образование.ОбразованиеId);
            }

            foreach (var id in educationsId)
            {
                Controller.Remove(new Образование(), 
                    q => q.ОбразованиеId == id);
            }


            Controller.Remove(new Сотрудник(), 
                q => q.СотрудникId == index);


            Controller.CloseConnection();
            SyncEmployeesList();
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TvUnits_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var tvUnits = sender as TreeView;

            List<Сотрудник> СписокСотрудников = null;

            TreeViewItem item = tvUnits.SelectedItem as TreeViewItem;

            Controller.OpenConnection();
            
            //Выбрано подразделение
            if (tvUnits.Items.Contains(tvUnits.SelectedItem))
            {
                if (item.Header.ToString() != "Все подразделения")
                {
                    var unitId = (int) item.Tag;
                    СписокСотрудников = Controller.Select(new Сотрудник(),
                        q => q.ОсновнаяИнформация.Должность.Подразделение.ПодразделениеId == unitId);
                }
            } // Выбрана должность
            else
            {
                var positionId = (int)item.Tag;
                СписокСотрудников = Controller.Select(new Сотрудник(),
                    q => q.ОсновнаяИнформация.Должность.ДолжностьId == positionId);
            }

            Controller.CloseConnection();
            SyncEmployeesList(СписокСотрудников);
        }
    }
}
