using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.PerformanceData;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
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

        public static dynamic FindDocumentByNumberNotGeneric(int number, Type type, Expression<Func<dynamic,bool>> predicate = null)
        {
            dynamic result;
            if (predicate != null)
            {
                result = FindDocument(Activator.CreateInstance(type), predicate);
            }
            else
            {
                result = findDocument(Activator.CreateInstance(type), number);

            }
            return result;
        }

        public static dynamic FindDocument<T>(T instance, Expression<Func<T, bool>> predicate) where T:class
        {
            var t = instance.GetType();

            Controller.Context.Set(instance.GetType()).Load();
            var table = Controller.Context.Set(instance.GetType()).Local;

            var documents = new List<T>();
            foreach (var element in table)
            {
                documents.Add(element as T);
            }
            var result = documents.AsQueryable().FirstOrDefault(predicate);

            return result as T;
        }

        private static T findDocument<T>(T instance, int number) where T:class
        {
            var t = instance.GetType();

            Controller.Context.Set(instance.GetType()).Load();
            var table = Controller.Context.Set(instance.GetType()).Local;

            var documents = new List<IДокумент>();
            foreach (var element in table)
            {
                documents.Add(element as IДокумент);
            }
            var result = documents.FirstOrDefault(q => q.Номер == number.ToString());

            return result as T;
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

        public static bool RemoveDocumentNotGeneric(Type type, Expression<Func<dynamic, bool>> predicate)
        {
            var table = Controller.Context.Set(type);

            var item = FindDocumentByNumberNotGeneric(0, type, predicate);
            if (Equals(item, null)) return false;

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

        public static dynamic SelectAllDynamic(Type type)
        {
            return selectAll(Activator.CreateInstance(type));
        }

        private static List<T> selectAll<T>(T instance) where T:class 
        {
            var table = Controller.Context.Set(instance.GetType());

            var result = new List<T>();
            foreach (var element in table)
            {
                result.Add(element as T);
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
