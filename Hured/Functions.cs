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

        public static void AddUnitsFromDB(ref ComboBox cbUnits)
        {
            Controller.OpenConnection();
            var units = Controller.Select(new Подразделение(), e => e != null);
            Controller.CloseConnection();

            var stringUnits = units.Select((подразделение) => подразделение.Название);

            foreach (var unit in stringUnits)
            {
                cbUnits.Items.Add(unit);
            }
        }

        public static List<Должность> GetPositionsForUnit(string unit)
        {
            Controller.OpenConnection();
            var positions = Controller.Select(new Должность(), e => e.Подразделение.Название == unit);
            Controller.CloseConnection();
            return positions;
        }

        public static void AddPositionsFromDB(ref ListView lvPositions, string unit = "")
        {
            List<Должность> positions = new List<Должность>();
            if (unit == "")
            {
                Controller.OpenConnection();
                positions = Controller.Select(new Должность(), e => e != null);
                Controller.CloseConnection();

            }
            else
            {
                positions = GetPositionsForUnit(unit);
            }
            foreach (var должность in positions)
            {
                lvPositions.Items.Add(должность);
            }
        }

        public static void AddPositionsFromDB(ref ComboBox cbPositions, string unit = "")
        {
            List<Должность> positions = new List<Должность>();
            if (unit == "")
            {
                Controller.OpenConnection();
                positions = Controller.Select(new Должность(), e => e != null);
                Controller.CloseConnection();

            }
            else
            {
                positions = GetPositionsForUnit(unit);
            }
            foreach (var должность in positions)
            {
                cbPositions.Items.Add(должность.Название);
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
            Controller.OpenConnection();
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
            Controller.CloseConnection();
        }

        //public static void OnTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        //{
        //    TreeView tv = sender as TreeView;


        //}
    }
}
