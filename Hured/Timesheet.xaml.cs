using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Hured.DBModel;
using Hured.Tables_templates;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Timesheet.xaml
    /// </summary>
    public partial class Timesheet
    {
        public Timesheet()
        {
            InitializeComponent();

            Functions.FillTreeView(ref TvUnits);

            DgTimeSheet.CanUserAddRows = false;
            DgTimeSheet.CanUserSortColumns = false;
            DgTimeSheet.GridLinesVisibility = DataGridGridLinesVisibility.All;
            DgTimeSheet.SelectionUnit = DataGridSelectionUnit.Cell;

            _isGridCreated = false;

            CurrentDate = DateTime.Now;

            SyncTimeSheet(firstDay:CurrentDate);
        }

        readonly List<int> _employeesId = new List<int>();

        private bool _isGridCreated;

        private DateTime _currentDate;

        private DateTime CurrentDate
        {
            get { return _currentDate; }
            set
            {
                _currentDate = value;
                CurrentDateChanged = true;
            }
        }

        private bool CurrentDateChanged = false;

        private const int DaysCount = 7;

        void SyncTimeSheet(List<Сотрудник> сотрудники = null, DateTime firstDay = default(DateTime))
        {
            if (!_isGridCreated || CurrentDateChanged)
            {
                var name = new DataGridTextColumn { Header = "Сотрудник" };

                DgTimeSheet.Columns.Clear();

                DgTimeSheet.Columns.Add(name);

                var now = firstDay;

                for (var i = 0; i < DaysCount; i++)
                {
                    var column = new DataGridTemplateColumn { Header = now.ToShortDateString() };

                    DgTimeSheet.Columns.Add(column);

                    now = now.AddDays(1);
                }
                _isGridCreated = true;
                CurrentDateChanged = false;
            }

            DgTimeSheet.Items.Clear();

            Controller.OpenConnection();
            if (сотрудники == null)
            {
                сотрудники = Controller.Select<Сотрудник>(q => q != null);
            }

            foreach (var employee in сотрудники)
            {
                var entries = Controller.Select<ТабельнаяЗапись>(
                    q => q.Сотрудник.СотрудникId == employee.СотрудникId);
                entries = entries.OrderBy(q => q.Дата).ToList();

                var sEntries = new object[DgTimeSheet.Columns.Count];

                sEntries[0] = employee.ОсновнаяИнформация.Фамилия + " " +
                employee.ОсновнаяИнформация.Имя + " " +
                employee.ОсновнаяИнформация.Отчество + " ";

                for (var i = 1; i < sEntries.Length; i++)
                {
                    foreach (var entry in entries)
                    {
                        if (DgTimeSheet.Columns[i].Header.ToString() == entry.Дата.ToShortDateString())
                        {
                            sEntries[i] = entry;
                            break;
                        }
                    }
                }

                DgTimeSheet.Items.Add(sEntries);
                _employeesId.Add(employee.СотрудникId);
            }

            Controller.CloseConnection();

            


            DgTimeSheet.ItemContainerGenerator.StatusChanged += OnItemContainerGeneratorStautsChanged;

            DgTimeSheet.UpdateLayout();

            //DgTimeSheet.ItemContainerGenerator.StatusChanged += (sender, args) =>
            //{
            //    if (DgTimeSheet.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            //    {

            //    }
            //};

        }

        private void OnItemContainerGeneratorStautsChanged(object sender, EventArgs args)
        {
            if (DgTimeSheet.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                DgTimeSheet.ItemContainerGenerator.StatusChanged -= OnItemContainerGeneratorStautsChanged;
                Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(DelayedAction));
            }
        }

        private void DelayedAction()
        {
            for (var row = 0; row < DgTimeSheet.Items.Count; row++)
            {
                var items = DgTimeSheet.Items[row] as Object[];

                for (var column = 0; column < DgTimeSheet.Columns.Count; column++)
                {
                    //var cell = GetCell(row, column);

                    var cell = DgTimeSheet.GetCellOfColumn(row, column);

                    if (items != null && items[column] == null)
                    {
                        cell.Tag = false;
                        continue;
                    }

                    var content = items?[column] as ТабельнаяЗапись;
                    if (content == null)
                    {
                        cell.Content = items?[column] as String;
                        continue;
                    }


                    cell.Tag = true;
                    cell.Background = GetColorFromString(content.Статус.Цвет);
                    cell.Content = content.Статус.Название;
                }
            }
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
                if (item != null && item.Header.ToString() != "Все подразделения")
                {
                    var unitId = (int)item.Tag;
                    списокСотрудников = Controller.Select<Сотрудник>(
                        q => q.ОсновнаяИнформация.Должность.Подразделение.ПодразделениеId == unitId);
                }
            } // Выбрана должность
            else
            {
                if (item?.Tag != null)
                {
                    var positionId = (int)item.Tag;
                    списокСотрудников = Controller.Select<Сотрудник>(
                        q => q.ОсновнаяИнформация.Должность.ДолжностьId == positionId);
                }
            }

            Controller.CloseConnection();
            SyncTimeSheet(списокСотрудников, CurrentDate);
        }

        Brush GetColorFromString(string colorString)
        {
            var r = Convert.ToByte(colorString.Split(' ')[0]);
            var g = Convert.ToByte(colorString.Split(' ')[1]);
            var b = Convert.ToByte(colorString.Split(' ')[2]);

            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        public DataGridCell GetCell(int row, int column)
        {
            var rowContainer = GetRow(row);

            if (rowContainer != null)
            {
                var presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter == null) presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);


                DgTimeSheet.ScrollIntoView(rowContainer, DgTimeSheet.Columns[column]);
                    var cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);

                return cell;
            }
            return null;
        }

        public DataGridRow GetRow(int index)
        {
            DgTimeSheet.UpdateLayout();
            DgTimeSheet.ScrollIntoView(DgTimeSheet.Items[index]);
            var row = (DataGridRow)DgTimeSheet.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                DgTimeSheet.UpdateLayout();
                DgTimeSheet.ScrollIntoView(DgTimeSheet.Items[index]);
                row = (DataGridRow)DgTimeSheet.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            var child = default(T);
            var numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < numVisuals; i++)
            {
                var v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T ?? GetVisualChild<T>(v);
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DgTimeSheet_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            var grid = sender as DataGrid;


            if (e.Column.DisplayIndex > 0)
            {
                var cell = GetCell(e.Row.GetIndex(), e.Column.DisplayIndex);
                var isEditing = (bool)cell.Tag;


                TimesheetEntry w;
                if (isEditing)
                {
                    var employeeId = _employeesId[e.Row.GetIndex()];
                    var entryDate = DateTime.Parse(e.Column.Header.ToString());
                    Controller.OpenConnection();
                    var entry = Controller.Find<ТабельнаяЗапись>(
                        q => q.Сотрудник.СотрудникId == employeeId &&
                             q.Дата == entryDate);
                    Controller.CloseConnection();
                    w = new TimesheetEntry(new[]
                    {
                        e.Column.Header,
                        (grid?.Items[e.Row.GetIndex()] as object[])?[0],
                        entry
                    });
                }
                else
                {
                    w = new TimesheetEntry(new[]
                    {
                        e.Column.Header,
                        (grid?.Items[e.Row.GetIndex()] as object[])?[0]
                    });
                }

                w.ShowDialog();
                if (w.DialogResult == true)
                {
                    var entry = w.Tag as ТабельнаяЗапись;
                    if (entry != null)
                    {
                        entry.Дата = DateTime.Parse(e.Column.Header.ToString());

                        var employeeId = _employeesId[e.Row.GetIndex()];
                        entry.Сотрудник = Controller.Find<Сотрудник>(
                            q => q.СотрудникId == employeeId);

                        if (isEditing)
                        {
                            Controller.Edit(q => q.ТабельнаяЗаписьId == entry.ТабельнаяЗаписьId, entry);
                        }
                        else
                        {
                            Controller.Insert(entry);

                        }
                    }

                    Controller.CloseConnection();
                }
            }
            e.Cancel = true;

            SyncTimeSheet(firstDay:CurrentDate);
        }

        private void Back_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentDate = CurrentDate.AddDays(-7);
            SyncTimeSheet(firstDay:CurrentDate);
        }

        private void Next_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentDate = CurrentDate.AddDays(7);
            SyncTimeSheet(firstDay:CurrentDate);
        }

    }

    public static class VisualHelper
    {
        public static T FindVisualChild<T>(this Visual parent) where T : Visual
        {
            List<T> childs = new List<T>();

            return GetVisualChild(parent, true, ref childs);
        }

        public static IEnumerable<T> FindVisualChilds<T>(this Visual parent) where T : Visual
        {
            List<T> childs = new List<T>();
            GetVisualChild(parent, false, ref childs);
            return childs;
        }

        public static T FindVisualChildByName<T>(this Visual parent, string name) where T : Visual
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            List<T> childs = new List<T>();

            return GetVisualChild(parent, true, ref childs, true, name);
        }

        private static T GetVisualChild<T>(this Visual parent, bool getOnlyOnechild, ref List<T> list, bool findByName = false, string childName = "") where T : Visual
        {
            T child = default(T);

            for (int index = 0; index < VisualTreeHelper.GetChildrenCount(parent); index++)
            {
                Visual visualChild = (Visual)VisualTreeHelper.GetChild(parent, index);
                child = visualChild as T;

                if (child == null)
                    child = GetVisualChild<T>(visualChild, getOnlyOnechild, ref list);//Find Recursively

                if (child != null)
                {
                    if (getOnlyOnechild)
                    {
                        if (findByName)
                        {
                            var element = child as FrameworkElement;
                            if (element != null && element.Name == childName)
                                break;
                        }
                        else
                            break;
                    }
                    else
                        list.Add(child);
                }
            }
            return child;
        }
    }

    public static class DataGridHelper
    {
        public static DataGridColumn GetColumnByIndices(this DataGrid dataGrid, int rowIndex, int columnIndex)
        {
            if (dataGrid == null)
                return null;

            //Validate Indices
            ValidateColumnIndex(dataGrid, columnIndex);
            ValidateRowIndex(dataGrid, rowIndex);


            var row = dataGrid.GetRowByIndex(rowIndex);


            if (row != null)//Get Column for the DataGridRow by index using GetRowColumnByIndex Extension methods
                return row.GetRowColumnByIndex(dataGrid, columnIndex);

            return null;
        }

        public static DataGridCell GetCellByIndices(this DataGrid dataGrid, int rowIndex, int columnIndex)
        {
            if (dataGrid == null)
                return null;

            //Validate Indices
            ValidateColumnIndex(dataGrid, columnIndex);
            ValidateRowIndex(dataGrid, rowIndex);

            var row = dataGrid.GetRowByIndex(rowIndex);

            if (row != null)
                return row.GetCellByColumnIndex(dataGrid, columnIndex);

            return null;
        }

        public static DataGridRow GetRowByIndex(this DataGrid dataGrid, int rowIndex)
        {
            if (dataGrid == null)
                return null;

            ValidateRowIndex(dataGrid, rowIndex);

            var container = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            var result = (DataGridRow) container;

            if (result == null)
            {
                dataGrid.UpdateLayout();
                dataGrid.ScrollIntoView(dataGrid.Items[rowIndex]);
                result = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            }
            return result;
        }

        public static DataGridColumn GetRowColumnByIndex(this DataGridRow row, DataGrid dataGrid, int columnIndex)
        {

            if (row == null || dataGrid == null)
                return null;

            ValidateColumnIndex(dataGrid, columnIndex);

            var cell = GetCellByColumnIndex(row, dataGrid, columnIndex);

            if (cell != null)
                return cell.Column;

            return null;
        }

        public static DataGridCell GetCellByColumnIndex(this DataGridRow row, DataGrid dataGrid, int columnIndex)
        {
            if (row == null || dataGrid == null)
                return null;

            ValidateColumnIndex(dataGrid, columnIndex);

            DataGridCellsPresenter cellPresenter = row.FindVisualChild<DataGridCellsPresenter>();

            if (cellPresenter != null)
                return ((DataGridCell)cellPresenter.ItemContainerGenerator.ContainerFromIndex(columnIndex));

            return null;
        }

        public static T GetUIElementOfCell<T>(this DataGrid dataGrid, int rowIndex, int columnIndex) where T : Visual
        {
            var cell = GetCellByIndices(dataGrid, rowIndex, columnIndex);

            if (cell != null)
                return cell.FindVisualChild<T>();

            return null;
        }

        public static IEnumerable<T> GetUIElementsOfCell<T>(this DataGrid dataGrid, int rowIndex, int columnIndex) where T : Visual
        {
            var cell = GetCellByIndices(dataGrid, rowIndex, columnIndex);

            if (cell != null)
                return cell.FindVisualChilds<T>();

            return null;
        }

        public static T GetUIElementOfCell<T>(this DataGridCell dataGridCell) where T : Visual
        {
            if (dataGridCell == null)
                return null;

            return dataGridCell.FindVisualChild<T>();
        }

        private static void ValidateColumnIndex(DataGrid dataGrid, int columnIndex)
        {
            if (columnIndex >= dataGrid.Columns.Count)
                throw new IndexOutOfRangeException("columnIndex out of range");
        }

        private static void ValidateRowIndex(DataGrid dataGrid, int rowIndex)
        {
            if (rowIndex >= dataGrid.Items.Count)
                throw new IndexOutOfRangeException("rowIndex out of range");
        }
    }

    public class ExtendedDataGrid : DataGrid
    {
        public DataGridRow this[int rowIndex]
        {
            get { return DataGridHelper.GetRowByIndex(this as DataGrid, rowIndex); }
        }

        public DataGridColumn this[int rowIndex, int columnIndex]
        {
            get { return DataGridHelper.GetColumnByIndices(this as DataGrid, rowIndex, columnIndex); }
        }

        public DataGridCell GetCellOfColumn(int rowIndex, int columnIndex)
        {
            return this.GetCellByIndices(rowIndex, columnIndex);
        }

        public T GetVisualElementOfCell<T>(int rowIndex, int columnIndex) where T : Visual
        {
            return this.GetCellByIndices(rowIndex, columnIndex).FindVisualChild<T>();
        }

        public IEnumerable<T> GetVisualElementsOfCell<T>(int rowIndex, int columnIndex) where T : Visual
        {
            return this.GetCellByIndices(rowIndex, columnIndex).FindVisualChilds<T>();
        }
    }
}
