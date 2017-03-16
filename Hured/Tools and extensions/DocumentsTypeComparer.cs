using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hured
{
    public static class DocumentsTypeDictionary
    {
        private static Dictionary<Type, OrderType> Documents = new Dictionary<Type, OrderType>();

        public static void AddDocumentType<T>(OrderType documentType) where T : IДокумент
        {
            Documents.Add(typeof(T), documentType);
        }

        private static OrderType _GetTypeOfDocument<T>() where T : IДокумент
        {
            return Documents[typeof(T)];
        }

        public static Type GetDocumentTypeByEnum(OrderType type)
        {
            return Documents.FirstOrDefault(q => q.Value == type).Key;
        }

        public static Type GetDocumentTypeByEnumName(string type)
        {
            return Documents.FirstOrDefault(q => q.Value.ToString().Contains(type)).Key;
        }

        public static OrderType GetEnumTypeOfDocument<T>(T document) where T : IДокумент
        {
            MethodInfo method = typeof(DocumentsTypeDictionary).GetMethod("_GetTypeOfDocument");
            MethodInfo genericMethod = method.MakeGenericMethod(typeof(T));
            var result = genericMethod.Invoke(null, null);
            return (OrderType)result;
        }

        public static OrderType GetEnumTypeOfDocumentByName(string name)
        {
            return (OrderType)Documents.FirstOrDefault(q => q.Key.Name == name).Value;
        }

        public static List<Type> GetTypesOfAllDocuments()
        {
            return Documents.Select(q => q.Key).ToList();
        }

        public static Type GetDocumentTypeByName(string name)
        {
            return Documents.Keys.FirstOrDefault(q => q.Name.Contains(name));
        }
    }
}
