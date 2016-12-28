using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Hured.DBModel;

namespace Hured
{
    class Functions
    {
        public static void AddUnitsFromDB(ref ListView lvUnits)
        {
            Controller.OpenConnection();
            var units = Controller.Select(new Подразделение(), e => e != null);
            Controller.CloseConnection();

            var stringUnits = units.Select((подразделение) => подразделение.Название);

            foreach (var unit in stringUnits)
            {
                lvUnits.Items.Add(unit);
            }
        }

        // Вызывает стандартный диалог печати
        public static void Print()
        {
            
        }

        // Создаёт приказ с заданными параметрами
        public static void CreateOrder()
        {
            
        }

        public static void FillTreeView(ref TreeView tv)
        {
            var Подразделения = Controller.Select(new Подразделение(), e => e != null);

            TreeViewItem item = new TreeViewItem();

            item.Header = "Все подразделения";
            foreach (var должность in Controller.Select(new Должность(), e => e!=null))
            {
                item.Items.Add(должность.Название);
            }
            tv.Items.Add(item);

            foreach (var подразделение in Подразделения)
            {
                item = new TreeViewItem();
                item.Tag = подразделение;
                item.Header = подразделение.Название;
                foreach (var должность in Controller.Select(new Должность(), e => e.Подразделение.ПодразделениеId == подразделение.ПодразделениеId))
                {
                    item.Items.Add(должность.Название);
                }
                tv.Items.Add(item);

            }
        }

        //public static void OnTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        //{
        //    TreeView tv = sender as TreeView;


        //}
    }
}
