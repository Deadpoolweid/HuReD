using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Hured.DBModel;
using Hured.Tables_templates;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Employees.xaml
    /// </summary>
    public partial class Employees
    {
        public Employees()
        {
            InitializeComponent();

            SyncEmployeesList();
            Functions.FillTreeView(ref TvUnits);
        }

        readonly TransactionResult _tResult = new TransactionResult();

        private readonly List<int> _employeesId = new List<int>();

        void SyncEmployeesList(List<Сотрудник> сотрудники = null)
        {
            LvEmployees.Items.Clear();
            _employeesId.Clear();

            if (сотрудники == null)
            {
                Controller.OpenConnection();
                сотрудники = Controller.Select(new Сотрудник(), e => e != null);
                Controller.CloseConnection();
            }


            foreach (var сотрудник in сотрудники)
            {
                LvEmployees.Items.Add(сотрудник.ОсновнаяИнформация);
                _employeesId.Add(сотрудник.СотрудникId);
            }
        }

        private void bAdd_OnClick(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;
            var w = new Employee();
            w.ShowDialog();

            _tResult.RecordsAdded++;

            SyncEmployeesList();

            IsHitTestVisible = true;
        }

        private void BChange_OnClick(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;
            var index = _employeesId[LvEmployees.SelectedIndex];
            Controller.OpenConnection();
            var employee = Controller.Select(new Сотрудник(),
                q => q.СотрудникId == index).FirstOrDefault();
            Controller.CloseConnection();

            var w = new Employee(employee);

            _tResult.RecordsChanged++;

            w.ShowDialog();

            IsHitTestVisible = true;
            SyncEmployeesList();
        }

        private void bRemove_Click(object sender, RoutedEventArgs e)
        {
            Controller.OpenConnection();

            var index = _employeesId[LvEmployees.SelectedIndex];

            var employee = Controller.Find(new Сотрудник(), q => q.СотрудникId == index);

            Controller.Remove(new ОсновнаяИнформация(),
                q => q.ОсновнаяИнформацияId == employee.ОсновнаяИнформация.ОсновнаяИнформацияId);
            Controller.Remove(new УдостоверениеЛичности(),
                q => q.УдостоверениеЛичностиId == employee.УдостоверениеЛичности.УдостоверениеЛичностиId);
            Controller.Remove(new ВоинскийУчёт(),
                q => q.ВоинскийУчётId == employee.ВоинскийУчёт.ВоинскийУчётId);

            var educationsId = employee.Образование.Select(образование => образование.ОбразованиеId).ToList();

            foreach (var id in educationsId)
            {
                Controller.Remove(new Образование(),
                    q => q.ОбразованиеId == id);
            }


            Controller.Remove(new Сотрудник(),
                q => q.СотрудникId == index);


            Controller.CloseConnection();

            _tResult.RecordsDeleted++;
            SyncEmployeesList();
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Controller.OpenConnection();
            _tResult.RecordsCount = Controller.RecordsCount<Сотрудник>();
            Controller.CloseConnection();

            Tag = _tResult;
            Close();
        }

        private void TvUnits_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var tvUnits = sender as TreeView;

            List<Сотрудник> списокСотрудников = null;

            var item = tvUnits?.SelectedItem as TreeViewItem;

            Controller.OpenConnection();

            //Выбрано подразделение
            if (tvUnits != null && tvUnits.Items.Contains(tvUnits.SelectedItem))
            {
                if (item?.Header.ToString() != "Все подразделения")
                {
                    if (item?.Tag != null)
                    {
                        var unitId = (int)item.Tag;
                        списокСотрудников = Controller.Select(new Сотрудник(),
                            q => q.ОсновнаяИнформация.Должность.Подразделение.ПодразделениеId == unitId);
                    }
                }
            } // Выбрана должность
            else
            {
                if (item != null)
                {
                    var positionId = (int)item.Tag;
                    списокСотрудников = Controller.Select(new Сотрудник(),
                        q => q.ОсновнаяИнформация.Должность.ДолжностьId == positionId);
                }
            }

            Controller.CloseConnection();
            SyncEmployeesList(списокСотрудников);
        }
    }
}
