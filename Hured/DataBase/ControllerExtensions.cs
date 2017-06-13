using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catel.Collections;

namespace Hured.DataBase
{
    public class ControllerExtensions 
    {
        public static dynamic FindDocumentNotGeneric(int id, Type type)
        {
            var table = Controller.Context.Set(type);

            dynamic result = Activator.CreateInstance(type);

            foreach (var _element in table)
            {
                var element = _element as IДокумент;
                if (element == null) return null;
                if (element.GetId() == id)
                {
                    result = _element;
                }
            }

            return result;
        }

        public static dynamic FindDocumentByNumberNotGeneric(int number, Type type)
        {
            var table = Controller.Context.Set(type);

            dynamic result = Activator.CreateInstance(type);

            if (table.Local.Count < 1) return null;

            foreach (var _element in table)
            {
                var element = _element as IДокумент;
                if (element == null) return null;
                if (int.Parse(element.Номер) == number)
                {
                    result = _element;
                    break;
                }
                else
                {
                    result = null;
                }
            }

            return result;
        }

        public static bool EditDocumentByNumberNotGeneric(int number, object newItem, Type type)
        {
            var item = FindDocumentByNumberNotGeneric(number, type);

            if (item == null) return false;

            try
            {
                foreach (var property in type.GetProperties())
                {
                    var value = newItem.GetType().GetProperty(property.Name).GetValue(newItem);

                    if (property.Name.Contains("Id") || Equals(property.GetValue(newItem), null))
                    {
                        continue;
                    }
                    property.SetValue(item, value);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool RemoveDocumentNotGeneric(int id, Type type)
        {
            var table = Controller.Context.Set(type);

            var item = FindDocumentNotGeneric(id, type);

            if (item == null) return false;

            table.Remove(item);

            return true;
        }

        public static ArrayList SelectAll(Type type)
        {
            var table = Controller.Context.Set(type);

            ArrayList result = new ArrayList();

            foreach (var element in table)
            {
                result.Add(element);
            }

            return result;
        }

        public static bool ExistsDocumentNotGenericByNumber(int number, Type type)
        {
            var result = FindDocumentByNumberNotGeneric(number, type);
            return result != null;
        }
    }
}
