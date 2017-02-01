using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
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

            SyncTimeSheet();
        }

        readonly List<int> _employeesId = new List<int>();

        private bool _isGridCreated;

        private const int DaysCount = 20;

        void SyncTimeSheet(List<Сотрудник> сотрудники = null)
        {
            if (!_isGridCreated)
            {
                var name = new DataGridTextColumn { Header = "Сотрудник" };

                DgTimeSheet.Columns.Add(name);

                var now = DateTime.Now.Date;

                for (var i = 0; i < DaysCount; i++)
                {
                    var column = new DataGridTemplateColumn { Header = now.ToShortDateString() };

                    DgTimeSheet.Columns.Add(column);

                    now = now.AddDays(1);
                }
                _isGridCreated = true;
            }

            DgTimeSheet.Items.Clear();

            Controller.OpenConnection();
            if (сотрудники == null)
            {
                сотрудники = Controller.Select(new Сотрудник(), q => q != null);
            }

            foreach (var employee in сотрудники)
            {
                var entries = Controller.Select(new ТабельнаяЗапись(),
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


            DgTimeSheet.ItemContainerGenerator.StatusChanged += (sender, args) =>
            {
                if (DgTimeSheet.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                {
                    for (var row = 0; row < DgTimeSheet.Items.Count; row++)
                    {
                        var items = DgTimeSheet.Items[row] as Object[];

                        for (var column = 0; column < DgTimeSheet.Columns.Count; column++)
                        {
                            var cell = GetCell(row, column);

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
            };

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
                    списокСотрудников = Controller.Select(new Сотрудник(),
                        q => q.ОсновнаяИнформация.Должность.Подразделение.ПодразделениеId == unitId);
                }
            } // Выбрана должность
            else
            {
                if (item?.Tag != null)
                {
                    var positionId = (int)item.Tag;
                    списокСотрудников = Controller.Select(new Сотрудник(),
                        q => q.ОсновнаяИнформация.Должность.ДолжностьId == positionId);
                }
            }

            Controller.CloseConnection();
            SyncTimeSheet(списокСотрудников);
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

                var cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    DgTimeSheet.ScrollIntoView(rowContainer, DgTimeSheet.Columns[column]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
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
                    var entry = Controller.Find(new ТабельнаяЗапись(),
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
                        entry.Сотрудник = Controller.Find(new Сотрудник(),
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

            SyncTimeSheet();
        }
    }
}
