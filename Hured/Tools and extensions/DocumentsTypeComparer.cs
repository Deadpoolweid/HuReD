using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hured.Tools_and_extensions
{
    public static class DocumentsTypeComparer
    {
        private static Dictionary<Type, OrderType> Documents = new Dictionary<Type, OrderType>();

        public static void AddDocument<T>(OrderType documentType) where T : Приказ
        {
            Documents.Add(typeof(T), documentType);
        }

        private static OrderType _GetTypeOfDocument<T>() where T : Приказ
        {
            return Documents[typeof(T)];
        }

        public static Type GetDocumentType(OrderType type)
        {
            return Documents.FirstOrDefault(q => q.Value == type).Key;
        }

        public static Type GetDocumentType(string type)
        {
            return Documents.FirstOrDefault(q => q.Value.ToString().Contains(type)).Key;
        }

        public static OrderType GetTypeOfDocument<T>(T document) where T : Приказ
        {
            MethodInfo method = typeof(DocumentsTypeComparer).GetMethod("_GetTypeOfDocument");
            MethodInfo genericMethod = method.MakeGenericMethod(typeof(T));
            var result = genericMethod.Invoke(null, null);
            return (OrderType)result;
        }

        public static OrderType GetTypeOfDocument(string name)
        {
            return (OrderType)Documents.FirstOrDefault(q => q.Key.Name == name).Value;
        }

        public static List<Type> GetAllDocuments()
        {
            return Documents.Select(q => q.Key).ToList();
        }

        public static Type GetDocument(string name)
        {
            return Documents.Keys.FirstOrDefault(q => q.Name.Contains(name));
        }
    }
}
