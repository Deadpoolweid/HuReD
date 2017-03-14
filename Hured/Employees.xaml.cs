using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
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
            Closing += Employees_OnClosing;

            InitializeComponent();

            SyncEmployeesList();
            Functions.FillTreeView(ref TvUnits);

            (TvUnits.Items[0] as TreeViewItem).IsSelected = true;

            Functions.AddSortingToListView(LvEmployees);

        }

        private void Employees_OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            Controller.OpenConnection();
            _tResult.RecordsCount = Controller.RecordsCount<Сотрудник>();
            Controller.CloseConnection();

            Tag = _tResult;
        }

        readonly TransactionResult _tResult = new TransactionResult();

        private readonly List<int> _employeesId = new List<int>();

        void SyncEmployeesList(List<Сотрудник> сотрудники = null)
        {

            сотрудники = FilterEmployees();
            LvEmployees.Items.Clear();
            _employeesId.Clear();

            if (сотрудники == null)
            {
                Controller.OpenConnection();
                сотрудники = Controller.Select<Сотрудник>(e => e != null);
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
            var employee = Controller.Select<Сотрудник>(
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

            var employee = Controller.Find<Сотрудник>(q => q.СотрудникId == index);

            Controller.Remove<ОсновнаяИнформация>(
                q => q.ОсновнаяИнформацияId == employee.ОсновнаяИнформация.ОсновнаяИнформацияId);
            Controller.Remove<УдостоверениеЛичности>(
                q => q.УдостоверениеЛичностиId == employee.УдостоверениеЛичности.УдостоверениеЛичностиId);
            Controller.Remove<ВоинскийУчёт>(
                q => q.ВоинскийУчётId == employee.ВоинскийУчёт.ВоинскийУчётId);

            var educationsId = employee.Образование.Select(образование => образование.ОбразованиеId).ToList();

            foreach (var id in educationsId)
            {
                Controller.Remove<Образование>(
                    q => q.ОбразованиеId == id);
            }


            Controller.Remove<Сотрудник>(
                q => q.СотрудникId == index);


            Controller.CloseConnection();

            _tResult.RecordsDeleted++;
            SyncEmployeesList();
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {

            Close();
        }

        private List<Сотрудник> FilterEmployees()
        {
            var tvUnits = TvUnits;

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
                        var unitId = (int) item.Tag;
                        списокСотрудников = Controller.Select<Сотрудник>(
                            q => q.ОсновнаяИнформация.Должность.Подразделение.ПодразделениеId == unitId);
                    }
                }
                else
                {
                    списокСотрудников = Controller.Select<Сотрудник>(q => q != null);
                }
            } // Выбрана должность
            else
            {
                if (item != null)
                {
                    var positionId = (int)item.Tag;
                    списокСотрудников = Controller.Select<Сотрудник>(
                        q => q.ОсновнаяИнформация.Должность.ДолжностьId == positionId);
                }
            }

            Controller.CloseConnection();

            if (!tbSearch.IsHavePlaceholder() && tbSearch.Text != String.Empty)
            {
                var tags = tbSearch.Text.Split(' ');

                списокСотрудников = FilterEmployeesByTags(tags, списокСотрудников);
            }

            return списокСотрудников;
        }

        private List<Сотрудник> FilterEmployeesByTags(string[] tags, List<Сотрудник> employees)
        {
            var searchResult = employees.Where(
                q => new Regex(string.Join("|", tags.Select(Regex.Escape)),RegexOptions.IgnoreCase).IsMatch(
                    q.ОсновнаяИнформация.Имя + q.ОсновнаяИнформация.Фамилия +
                    q.ОсновнаяИнформация.Отчество)
                    );
            if (searchResult != null)
            {
                employees = searchResult.ToList();
            }

            return employees;
        }

        private void TvUnits_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SyncEmployeesList();
        }

        private void TbSearch_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbSearch.IsHavePlaceholder()) return;
            SyncEmployeesList();
        }
    }
}
