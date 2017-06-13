using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using Hured.DataBase;
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
            Closing += Order_OnClosing;

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

            var allDocuments = DocumentsTypeDictionary.GetTypesOfAllDocuments().Select(q => q.Name.Substring(6));

            foreach (var document in allDocuments)
            {
                CbOrderType.Items.Add(document);
            }
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

        private void Order_OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
        }

        readonly List<int> _employeesId = new List<int>();

        private readonly bool _isEditMode;

        private bool IsNumberExists(int number, OrderType ordertype)
        {
            Controller.OpenConnection();

            var result = ControllerExtensions.ExistsDocumentNotGenericByNumber(number,
                DocumentsTypeDictionary.GetDocumentTypeByEnum(ordertype));

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

            string swindowType = ((OrderType) CbOrderType.SelectedIndex).ToString();

            Assembly assembly = Assembly.GetExecutingAssembly();

            var windowType = assembly.GetTypes().Where(t => String.Equals(t.Namespace, "Hured", StringComparison.Ordinal)).ToList()
                .FirstOrDefault(q => q.Name == swindowType);
            var type = DocumentsTypeDictionary.GetDocumentTypeByEnum((OrderType)CbOrderType.SelectedIndex);

       
            dynamic window;

            var paramLength = type.GetConstructors().FirstOrDefault().GetParameters().Length;
            var parameters = new object[paramLength];

            // TODO Здесь создано условие только для одного документа, что мешает концепции универсальности
            if (windowType == typeof(Dismissal))
            {
                window = Activator.CreateInstance(windowType, args: new object[] {_employeesId[CbEmployee.SelectedIndex],null});
            }
            else
            {
                window = Activator.CreateInstance(windowType,args:new object[] {null});
            }
            


            if (_isEditMode)
            {
                Controller.OpenConnection();
                var number = TbНомерПриказа.Text;

                var order = ControllerExtensions.FindDocumentByNumberNotGeneric(int.Parse(number),
                    type);
                
                Controller.CloseConnection();


                if (windowType == typeof(Dismissal))
                {
                    window = Activator.CreateInstance(windowType, args: new[] { _employeesId[CbEmployee.SelectedIndex], order });
                }
                else
                {
                    window = Activator.CreateInstance(windowType, args: new[] { order });
                }
            }

            (window as Window).ShowDialog();

            if (window.DialogResult == true)
            {

                var order = Convert.ChangeType(window.Tag,type);
                if (order != null)
                {
                    order.Номер = TbНомерПриказа.Text;
                    order.Дата = DpДатаПриказа.DisplayDate;
                    var employeeId = _employeesId[CbEmployee.SelectedIndex];
                    order.Сотрудник = Controller.Find<Сотрудник>(q => q.СотрудникId == employeeId);

                    if (_isEditMode)
                    {
                        ControllerExtensions.EditDocumentByNumberNotGeneric(order.Номер,order,type);
                    }
                    else
                    {
                        Controller.Insert(order);
                    }
                }
                Controller.CloseConnection();
            }

            DialogResult = true;
            Close();
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
