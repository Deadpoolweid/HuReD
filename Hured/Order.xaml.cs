using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Hured.DBModel;
using Hured.Tables_templates;
using MahApps.Metro.Controls;
using Microsoft.Windows.Controls;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Order.xaml
    /// </summary>
    public partial class Order : MetroWindow
    {
        public Order(OrderInfo orderInfo = null)
        {
            InitializeComponent();



            Controller.OpenConnection();
            var сотрудники = Controller.Select(new Сотрудник(), e => e != null);
            var имена = new List<string>();
            foreach (var сотрудник in сотрудники)
            {
                имена.Add(сотрудник.ОсновнаяИнформация.Фамилия + " " +
                    сотрудник.ОсновнаяИнформация.Имя + " " +
                    сотрудник.ОсновнаяИнформация.Отчество);
                employeesId.Add(сотрудник.СотрудникId);
            }
            cbEmployee.ItemsSource = имена;
            cbOrderType.Items.Add("Приём");
            cbOrderType.Items.Add("Увольнение");
            cbOrderType.Items.Add("Отпуск");
            cbOrderType.Items.Add("Командировка");
            cbEmployee.SelectedIndex = cbOrderType.SelectedIndex = 0;

            if (orderInfo != null)
            {
                IsEditMode = true;
                tbНомерПриказа.Text = orderInfo.Номер;

                dpДатаПриказа.Text = orderInfo.Дата.ToString();
                cbOrderType.SelectedItem = orderInfo.Тип;
                cbOrderType.IsEnabled = false;
                cbEmployee.SelectedItem = orderInfo.ФИО;
                tbНомерПриказа.IsEnabled = false;
            }

            Controller.CloseConnection();
        }

        List<int> employeesId = new List<int>();

        private bool IsEditMode = false;

        private bool IsNumberExists(int number, OrderType ordertype)
        {
            Controller.OpenConnection();

            bool result = false;

            switch (ordertype)
            {
                case OrderType.Recruitment:
                    result = Controller.Exists(new ПриказПриём(),
                        q => q.Номер == number.ToString());
                    break;
                case OrderType.Dismissal:
                    result = Controller.Exists(new ПриказУвольнение(),
                        q => q.Номер == number.ToString());
                    break;
                case OrderType.Vacation:
                    result = Controller.Exists(new ПриказОтпуск(),
                        q => q.Номер == number.ToString());
                    break;
                case OrderType.BusinessTrip:
                    result = Controller.Exists(new ПриказКомандировка(),
                        q => q.Номер == number.ToString());
                    break;
                default:
                    break;
            }

            Controller.CloseConnection();
            return result;
        }

        private void bOk_Click(object sender, RoutedEventArgs e)
        {
            if (!IsEditMode)
            {
                if (IsNumberExists(int.Parse(tbНомерПриказа.Text), (OrderType)cbOrderType.SelectedIndex))
                {
                    var popup = new Popup()
                    {
                        StaysOpen = false,
                        Placement = PlacementMode.Mouse,
                        PopupAnimation = PopupAnimation.Fade,
                        AllowsTransparency = true,
                        Child = new Label()
                        {
                            Content = "Номер приказа для своего типа должен быть уникальным."
                        },

                    };

                    popup.IsOpen = true;

                    return;
                }
            }
           

            switch (cbOrderType.SelectedIndex)
            {
                case 0:
                    Recruitment wRecruitment = new Recruitment();
                    if (IsEditMode)
                    {
                        Controller.OpenConnection();
                        string number = tbНомерПриказа.Text;
                        var order = Controller.Find(new ПриказПриём(),
                            q => q.Номер == number);
                        Controller.CloseConnection();
                        wRecruitment = new Recruitment(order);

                    }

                    wRecruitment.ShowDialog();

                    if (wRecruitment.DialogResult == true)
                    {
                        var order = wRecruitment.Tag as ПриказПриём;
                        order.Номер = tbНомерПриказа.Text;
                        order.Дата = dpДатаПриказа.DisplayDate;
                        int employeeId = employeesId[cbEmployee.SelectedIndex];
                        order.Сотрудник = Controller.Find(new Сотрудник(),
                            q => q.СотрудникId == employeeId);

                        if (IsEditMode)
                        {
                            Controller.Edit(q => q.Номер == order.Номер,
                                order);
                        }
                        else
                        {
                            Controller.Insert(order);
                        }
                        Controller.CloseConnection();
                    }
                    break;
                case 1:
                    Dismissal wDismissal = new Dismissal(employeesId[cbEmployee.SelectedIndex]);

                    if (IsEditMode)
                    {
                        Controller.OpenConnection();
                        string number = tbНомерПриказа.Text;
                        var order = Controller.Find(new ПриказУвольнение(),
                            q => q.Номер == number);
                        Controller.CloseConnection();

                        wDismissal = new Dismissal(employeesId[cbEmployee.SelectedIndex], order);
                    }

                    wDismissal.ShowDialog();

                    if (wDismissal.DialogResult == true)
                    {
                        var order = wDismissal.Tag as ПриказУвольнение;
                        order.Номер = tbНомерПриказа.Text;
                        order.Дата = dpДатаПриказа.DisplayDate;
                        int employeeId = employeesId[cbEmployee.SelectedIndex];
                        order.Сотрудник = Controller.Find(new Сотрудник(),
                            q => q.СотрудникId == employeeId);

                        if (IsEditMode)
                        {
                            Controller.Edit(q => q.Номер == order.Номер, order);
                        }
                        else
                        {
                            Controller.Insert(order);
                        }
                        Controller.CloseConnection();
                    }

                    break;
                case 2:
                    Vacation wVacation = new Vacation();

                    if (IsEditMode)
                    {
                        Controller.OpenConnection();
                        string number = tbНомерПриказа.Text;
                        var order = Controller.Find(new ПриказОтпуск(),
                            q => q.Номер == number);
                        Controller.CloseConnection();

                        wVacation = new Vacation(order);
                    }

                    wVacation.ShowDialog();

                    if (wVacation.DialogResult == true)
                    {
                        var order = wVacation.Tag as ПриказОтпуск;
                        order.Номер = tbНомерПриказа.Text;
                        order.Дата = dpДатаПриказа.DisplayDate;
                        int employeeId = employeesId[cbEmployee.SelectedIndex];
                        Controller.OpenConnection();
                        order.Сотрудник = Controller.Find(new Сотрудник(),
                            q => q.СотрудникId == employeeId);
                        if (IsEditMode)
                        {
                            Controller.Edit(q => q.Номер == order.Номер, order);

                        }
                        else
                        {
                            Controller.Insert(order);
                        }
                        Controller.CloseConnection();
                    }

                    break;
                case 3:
                    BusinessTrip wBusinessTrip = new BusinessTrip();

                    if (IsEditMode)
                    {
                        Controller.OpenConnection();
                        string number = tbНомерПриказа.Text;
                        var order = Controller.Find(new ПриказКомандировка(),
                            q => q.Номер == number);
                        Controller.CloseConnection();
                        wBusinessTrip = new BusinessTrip(order);
                    }

                    wBusinessTrip.ShowDialog();

                    if (wBusinessTrip.DialogResult == true)
                    {
                        var order = wBusinessTrip.Tag as ПриказКомандировка;
                        order.Номер = tbНомерПриказа.Text;
                        order.Дата = dpДатаПриказа.DisplayDate;
                        int employeeId = employeesId[cbEmployee.SelectedIndex];
                        Controller.OpenConnection();
                        order.Сотрудник = Controller.Find(new Сотрудник(),
                            q => q.СотрудникId == employeeId);

                        if (IsEditMode)
                        {
                            Controller.Edit(q => q.Номер == order.Номер, order);

                        }
                        else
                        {
                            Controller.Insert(order);
                        }
                        Controller.CloseConnection();
                    }
                    break;
            }



            DialogResult = true;
            Close();
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
