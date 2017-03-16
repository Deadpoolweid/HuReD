using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Hured.DataBase;
using Hured.Tables_templates;
using Control = System.Windows.Forms.Control;

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

        void SyncEmployeesList()
        {
            var сотрудники = FillEmployeesList();
            LvEmployees.Items.Clear();
            _employeesId.Clear();

            if (сотрудники == null)
            {
                return;
            }

            foreach (var сотрудник in сотрудники)
            {
                LvEmployees.Items.Add(сотрудник.ОсновнаяИнформация);
                _employeesId.Add(сотрудник.СотрудникId);
            }
        }

        private void bAdd_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                IsHitTestVisible = false;
                var w = new Employee();
                w.ShowDialog();

                if (w.DialogResult == true)
                {
                    _tResult.RecordsAdded++;
                    SyncEmployeesList();
                }
                else
                {
                    Functions.ShowPopup(sender as Button, "Не удалось добавить сотрудника.");
                }

            }
            catch (Exception ex)
            {
                Functions.ShowPopup(this, "Произошла ошибка в работе модуля. Информация: " + ex);
            }
            finally
            {
                Controller.CloseConnection(true);
                IsHitTestVisible = true;

            }
        }

        private void BChange_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                IsHitTestVisible = false;
                var index = _employeesId[LvEmployees.SelectedIndex];
                Controller.OpenConnection();
                var employee = Controller.Select<Сотрудник>(
                    q => q.СотрудникId == index).FirstOrDefault();
                Controller.CloseConnection();

                if (employee == null)
                {
                    Functions.ShowPopup(sender as Button, "Не удалось найти сотрудника с имеющимся Id");
                }
                else
                {
                    var w = new Employee(employee);

                    w.ShowDialog();

                    if (w.DialogResult == true)
                    {

                        _tResult.RecordsChanged++;

                        SyncEmployeesList();
                    }
                    else
                    {
                        Functions.ShowPopup(sender as Button, "Не удалось изменить данные о сотруднике.");
                    }
                }

            }
            catch (Exception ex)
            {
                Functions.ShowPopup(this, "Произошла ошибка в работе модуля. Информация: " + ex);
            }
            finally
            {
                Controller.CloseConnection(true);
                IsHitTestVisible = true;

            }
        }

        private void bRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IsHitTestVisible = false;

                var index = _employeesId[LvEmployees.SelectedIndex];

                Controller.OpenConnection();
                var employee = Controller.Find<Сотрудник>(q => q.СотрудникId == index);
                if (employee == null)
                {
                    Functions.ShowPopup(sender as Button, "Не удалось найти сотрудника с имеющимся Id");
                }
                else
                {
                    if (!Controller.Remove<ОсновнаяИнформация>(
                        q => q.ОсновнаяИнформацияId == employee.ОсновнаяИнформация.ОсновнаяИнформацияId))
                        throw new Exception("Не удалось удалить элемент. Информация: " + employee.ОсновнаяИнформация);
                    if (Controller.Remove<УдостоверениеЛичности>(
                        q => q.УдостоверениеЛичностиId == employee.УдостоверениеЛичности.УдостоверениеЛичностиId))
                        throw new Exception("Не удалось удалить элемент. Информация: " + employee.УдостоверениеЛичности);
                    if (Controller.Remove<ВоинскийУчёт>(
                        q => q.ВоинскийУчётId == employee.ВоинскийУчёт.ВоинскийУчётId))
                        throw new Exception("Не удалось удалить элемент. Информация: " + employee.ВоинскийУчёт);

                    var educationsId = employee.Образование.Select(образование => образование.ОбразованиеId).ToList();

                    foreach (var id in educationsId)
                    {
                        if (Controller.Remove<Образование>(q => q.ОбразованиеId == id))
                            throw new Exception("Не удалось удалить элемент. Информация: " +
                                                employee.Образование.FirstOrDefault(q => q.ОбразованиеId == id));
                    }
                    if (Controller.Remove<Сотрудник>(q => q.СотрудникId == index))
                        throw new Exception("Не удалось удалить элемент. Информация: " + employee);

                    Controller.CloseConnection();

                    _tResult.RecordsDeleted++;
                    SyncEmployeesList();
                }

            }
            catch (Exception ex)
            {
                Functions.ShowPopup(this, "Произошла ошибка в работе модуля. Информация: " + ex);
            }
            finally
            {
                Controller.CloseConnection(true);
                IsHitTestVisible = true;
            }
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private List<Сотрудник> FillEmployeesList()
        {
            List<Сотрудник> списокСотрудников = null;

            var item = TvUnits?.SelectedItem as TreeViewItem;


            //Выбрано подразделение
            if (TvUnits != null && TvUnits.Items.Contains(TvUnits.SelectedItem))
            {
                if (item?.Header.ToString() != "Все подразделения")
                {
                    if (item?.Tag != null)
                    {
                        var unitId = (int)item.Tag;
                        Controller.OpenConnection();
                        списокСотрудников = Controller.Select<Сотрудник>(
                            q => q.ОсновнаяИнформация.Должность.Подразделение.ПодразделениеId == unitId);
                        Controller.CloseConnection();
                    }
                }
                else
                {
                    Controller.OpenConnection();
                    списокСотрудников = Controller.SelectAll<Сотрудник>();
                    Controller.CloseConnection();
                }
            } // Выбрана должность
            else
            {
                if (item != null)
                {
                    var positionId = (int)item.Tag;
                    Controller.OpenConnection();
                    списокСотрудников = Controller.Select<Сотрудник>(
                        q => q.ОсновнаяИнформация.Должность.ДолжностьId == positionId);
                    Controller.CloseConnection();
                }
            }

            if (!tbSearch.IsHavePlaceholder() && tbSearch.Text != string.Empty)
            {
                var tags = tbSearch.Text.Split(' ');

                списокСотрудников = FilterEmployeesByTags(tags, списокСотрудников);
            }

            return списокСотрудников;
        }

        private List<Сотрудник> FilterEmployeesByTags(string[] tags, List<Сотрудник> employees)
        {
            var searchResult = employees.Where(
                q => new Regex(string.Join("|", tags.Select(Regex.Escape)), RegexOptions.IgnoreCase).IsMatch(
                    q.ОсновнаяИнформация.Имя + q.ОсновнаяИнформация.Фамилия +
                    q.ОсновнаяИнформация.Отчество)
                    );
            employees = searchResult.ToList();

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
