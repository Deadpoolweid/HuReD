using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Hured.DBModel;
using Hured.Tables_templates;
using MahApps.Metro.Controls;
using DataGridRowEventArgs = System.Windows.Controls.DataGridRowEventArgs;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Timesheet.xaml
    /// </summary>
    public partial class Timesheet : MetroWindow
    {
        public Timesheet()
        {
            InitializeComponent();
            // TODO Заполнение списка должностей
            Functions.FillTreeView(ref tvUnits);

            dgTimeSheet.CanUserAddRows = false;
            dgTimeSheet.CanUserSortColumns = false;
            dgTimeSheet.GridLinesVisibility = DataGridGridLinesVisibility.All;
            dgTimeSheet.SelectionUnit = DataGridSelectionUnit.Cell;


            SyncTimeSheet();
        }

        List<int> EmployeesId = new List<int>();

        private bool IsGridCreated = false;

        int daysCount = 20;

        void SyncTimeSheet()
        {
            if (!IsGridCreated)
            {
                var Name = new DataGridTextColumn();
                //Name.Binding = new Binding("Name");
                Name.Header = "Сотрудник";
                //Name.ColumnName = Name.Caption;

                dgTimeSheet.Columns.Add(Name);
                //dgTimeSheet.Columns.Add(Name);

                var now = DateTime.Now.Date;

                for (int i = 0; i < daysCount; i++)
                {
                    var column = new DataGridTemplateColumn();
                    //column.Binding = new Binding(now.ToShortDateString().Replace(".",""));
                    column.Header = now.ToShortDateString();
                    //column.ColumnName = column.Caption;
                    dgTimeSheet.Columns.Add(column);

                    now = now.AddDays(1);
                }
                IsGridCreated = true;
            }

            dgTimeSheet.Items.Clear();

            Controller.OpenConnection();
            var employees = Controller.Select(new Сотрудник(), q => q != null);

            foreach (var employee in employees)
            {
                var entries = Controller.Select(new ТабельнаяЗапись(),
                    q => q.Сотрудник.СотрудникId == employee.СотрудникId);
                entries = entries.OrderBy(q => q.Дата).ToList();

                var sEntries = new object[dgTimeSheet.Columns.Count];

                sEntries[0] = employee.ОсновнаяИнформация.Фамилия + " " +
                employee.ОсновнаяИнформация.Имя + " " +
                employee.ОсновнаяИнформация.Отчество + " ";

                for (int i = 1; i < sEntries.Length; i++)
                {
                    bool hasMatch = false;
                    foreach (var entry in entries)
                    {
                        if (dgTimeSheet.Columns[i].Header.ToString() == entry.Дата.ToShortDateString())
                        {
                            sEntries[i] = entry;
                            break;
                        }
                    }
                }

                dgTimeSheet.Items.Add(sEntries);
                EmployeesId.Add(employee.СотрудникId);
            }

            Controller.CloseConnection();


            dgTimeSheet.ItemContainerGenerator.StatusChanged += (sender, args) =>
            {
                if (dgTimeSheet.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                {
                    for (int row = 0; row < dgTimeSheet.Items.Count; row++)
                    {
                        var items = dgTimeSheet.Items[row] as Object[];

                        for (int column = 0; column < dgTimeSheet.Columns.Count; column++)
                        {
                            var cell = GetCell(row, column);

                            if (items[column] == null)
                            {
                                cell.Tag = false;
                                continue;
                            }

                            var content = items[column] as ТабельнаяЗапись;
                            if (content == null)
                            {
                                cell.Content = items[column] as String;
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

        Brush GetColorFromString(string colorString)
        {
            byte r = Convert.ToByte(colorString.Split(' ')[0]);
            byte g = Convert.ToByte(colorString.Split(' ')[1]);
            byte b = Convert.ToByte(colorString.Split(' ')[2]);

            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        public DataGridCell GetCell(int row, int column)
        {
            DataGridRow rowContainer = GetRow(row);

            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);

                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    dgTimeSheet.ScrollIntoView(rowContainer, dgTimeSheet.Columns[column]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
                return cell;
            }
            return null;
        }

        public DataGridRow GetRow(int index)
        {
            dgTimeSheet.UpdateLayout();
            dgTimeSheet.ScrollIntoView(dgTimeSheet.Items[index]);
            DataGridRow row = (DataGridRow)dgTimeSheet.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                dgTimeSheet.UpdateLayout();
                dgTimeSheet.ScrollIntoView(dgTimeSheet.Items[index]);
                row = (DataGridRow)dgTimeSheet.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
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
                    var employeeId = EmployeesId[e.Row.GetIndex()];
                    var entryDate = DateTime.Parse(e.Column.Header.ToString());
                    Controller.OpenConnection();
                    var entry = Controller.Find(new ТабельнаяЗапись(),
                        q => q.Сотрудник.СотрудникId == employeeId &&
                             q.Дата == entryDate);
                    Controller.CloseConnection();
                    w = new TimesheetEntry(new object[]
                    {
                        e.Column.Header,
                        (grid.Items[e.Row.GetIndex()] as Object[])[0],
                        entry
                    });
                }
                else
                {
                    w = new TimesheetEntry(new object[]
                    {
                        e.Column.Header,
                        (grid.Items[e.Row.GetIndex()] as Object[])[0]
                    });
                }

                w.ShowDialog();
                if (w.DialogResult == true)
                {
                    var entry = w.Tag as ТабельнаяЗапись;
                    entry.Дата = DateTime.Parse(e.Column.Header.ToString());

                    var employeeId = EmployeesId[e.Row.GetIndex()];
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

                    Controller.CloseConnection();
                }
            }
            e.Cancel = true;

            SyncTimeSheet();
        }
    }
}
