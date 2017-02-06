using System.Collections.Generic;
using System.Windows;
using Hured.DBModel;
using Hured.Tables_templates;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Order.xaml
    /// </summary>
    public partial class Order
    {
        public Order(OrderInfo orderInfo = null)
        {
            InitializeComponent();



            Controller.OpenConnection();
            var сотрудники = Controller.Select<Сотрудник>(e => e != null);
            var имена = new List<string>();
            foreach (var сотрудник in сотрудники)
            {
                имена.Add(сотрудник.ОсновнаяИнформация.Фамилия + " " +
                    сотрудник.ОсновнаяИнформация.Имя + " " +
                    сотрудник.ОсновнаяИнформация.Отчество);
                _employeesId.Add(сотрудник.СотрудникId);
            }
            CbEmployee.ItemsSource = имена;
            CbOrderType.Items.Add("Приём");
            CbOrderType.Items.Add("Увольнение");
            CbOrderType.Items.Add("Отпуск");
            CbOrderType.Items.Add("Командировка");
            CbEmployee.SelectedIndex = CbOrderType.SelectedIndex = 0;

            if (orderInfo != null)
            {
                _isEditMode = true;
                TbНомерПриказа.Text = orderInfo.Номер;

                DpДатаПриказа.Text = orderInfo.Дата;
                CbOrderType.SelectedItem = orderInfo.Тип;
                CbOrderType.IsHitTestVisible = false;
                CbEmployee.SelectedItem = orderInfo.Фио;
                TbНомерПриказа.IsHitTestVisible = false;
            }

            Controller.CloseConnection();
        }

        readonly List<int> _employeesId = new List<int>();

        private readonly bool _isEditMode;

        private bool IsNumberExists(int number, OrderType ordertype)
        {
            Controller.OpenConnection();

            var result = false;

            switch (ordertype)
            {
                case OrderType.Recruitment:
                    result = Controller.Exists<ПриказПриём>(
                        q => q.Номер == number.ToString());
                    break;
                case OrderType.Dismissal:
                    result = Controller.Exists<ПриказУвольнение>(
                        q => q.Номер == number.ToString());
                    break;
                case OrderType.Vacation:
                    result = Controller.Exists<ПриказОтпуск>(
                        q => q.Номер == number.ToString());
                    break;
                case OrderType.BusinessTrip:
                    result = Controller.Exists<ПриказКомандировка>(
                        q => q.Номер == number.ToString());
                    break;
            }

            Controller.CloseConnection();
            return result;
        }

        private void bOk_Click(object sender, RoutedEventArgs e)
        {
            if (Functions.IsEmpty(TbНомерПриказа))
            {
                return;
            }

            if (!_isEditMode)
            {
                if (IsNumberExists(int.Parse(TbНомерПриказа.Text), (OrderType)CbOrderType.SelectedIndex))
                {
                    Functions.ShowPopup(TbНомерПриказа, "Номер приказа для своего типа должен быть уникальным.");
                    return;
                }
            }


            switch (CbOrderType.SelectedIndex)
            {
                case 0:
                    var wRecruitment = new Recruitment();
                    if (_isEditMode)
                    {
                        Controller.OpenConnection();
                        var number = TbНомерПриказа.Text;
                        var order = Controller.Find<ПриказПриём>(q => q.Номер == number);
                        Controller.CloseConnection();
                        wRecruitment = new Recruitment(order);

                    }

                    wRecruitment.ShowDialog();

                    if (wRecruitment.DialogResult == true)
                    {
                        var order = wRecruitment.Tag as ПриказПриём;
                        if (order != null)
                        {
                            order.Номер = TbНомерПриказа.Text;
                            order.Дата = DpДатаПриказа.DisplayDate;
                            var employeeId = _employeesId[CbEmployee.SelectedIndex];
                            order.Сотрудник = Controller.Find<Сотрудник>( q=> q.СотрудникId == employeeId);

                            if (_isEditMode)
                            {
                                Controller.Edit(q => q.Номер == order.Номер,
                                    order);
                            }
                            else
                            {
                                Controller.Insert(order);
                            }
                        }
                        Controller.CloseConnection();
                    }
                    break;
                case 1:
                    var wDismissal = new Dismissal(_employeesId[CbEmployee.SelectedIndex]);

                    if (_isEditMode)
                    {
                        Controller.OpenConnection();
                        var number = TbНомерПриказа.Text;
                        var order = Controller.Find<ПриказУвольнение>(q => q.Номер == number);
                        Controller.CloseConnection();

                        wDismissal = new Dismissal(_employeesId[CbEmployee.SelectedIndex], order);
                    }

                    wDismissal.ShowDialog();

                    if (wDismissal.DialogResult == true)
                    {
                        var order = wDismissal.Tag as ПриказУвольнение;
                        if (order != null)
                        {
                            order.Номер = TbНомерПриказа.Text;
                            order.Дата = DpДатаПриказа.DisplayDate;
                            var employeeId = _employeesId[CbEmployee.SelectedIndex];
                            order.Сотрудник = Controller.Find<Сотрудник>(
                                q => q.СотрудникId == employeeId);

                            if (_isEditMode)
                            {
                                Controller.Edit(q => q.Номер == order.Номер, order);
                            }
                            else
                            {
                                Controller.Insert(order);
                            }
                        }
                        Controller.CloseConnection();
                    }

                    break;
                case 2:
                    var wVacation = new Vacation();

                    if (_isEditMode)
                    {
                        Controller.OpenConnection();
                        var number = TbНомерПриказа.Text;
                        var order = Controller.Find<ПриказОтпуск>(q => q.Номер == number);
                        Controller.CloseConnection();

                        wVacation = new Vacation(order);
                    }

                    wVacation.ShowDialog();

                    if (wVacation.DialogResult == true)
                    {
                        var order = wVacation.Tag as ПриказОтпуск;
                        if (order != null)
                        {
                            order.Номер = TbНомерПриказа.Text;
                            order.Дата = DpДатаПриказа.DisplayDate;
                            var employeeId = _employeesId[CbEmployee.SelectedIndex];
                            Controller.OpenConnection();
                            order.Сотрудник = Controller.Find<Сотрудник>(
                                q => q.СотрудникId == employeeId);
                            if (_isEditMode)
                            {
                                Controller.Edit(q => q.Номер == order.Номер, order);

                            }
                            else
                            {
                                Controller.Insert(order);
                            }
                        }
                        Controller.CloseConnection();
                    }

                    break;
                case 3:
                    var wBusinessTrip = new BusinessTrip();

                    if (_isEditMode)
                    {
                        Controller.OpenConnection();
                        var number = TbНомерПриказа.Text;
                        var order = Controller.Find<ПриказКомандировка>(
                            q => q.Номер == number);
                        Controller.CloseConnection();
                        wBusinessTrip = new BusinessTrip(order);
                    }

                    wBusinessTrip.ShowDialog();

                    if (wBusinessTrip.DialogResult == true)
                    {
                        var order = wBusinessTrip.Tag as ПриказКомандировка;
                        if (order != null)
                        {
                            order.Номер = TbНомерПриказа.Text;
                            order.Дата = DpДатаПриказа.DisplayDate;
                            var employeeId = _employeesId[CbEmployee.SelectedIndex];
                            Controller.OpenConnection();
                            order.Сотрудник = Controller.Find<Сотрудник>(
                                q => q.СотрудникId == employeeId);

                            if (_isEditMode)
                            {
                                Controller.Edit(q => q.Номер == order.Номер, order);

                            }
                            else
                            {
                                Controller.Insert(order);
                            }
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
