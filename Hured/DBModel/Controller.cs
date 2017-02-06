using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MySql.Data.MySqlClient;

namespace Hured.DBModel
{

    static internal class Controller
    {
        private static string ConnectionString =String.Empty;

        public static void SetConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
        }

        private static MySqlConnection _connection;

        public static void OpenConnection()
        {
            _connection = new MySqlConnection(ConnectionString);
            Context = new Hured(_connection, false);
            _connection.Open();
            _transaction = _connection.BeginTransaction();

            // Interception/SQL logging
            //context.Database.Log = (string message) => { Console.WriteLine(message + "\n====================================\n"); };
            Context.Database.Log = message =>
            {
                using (var file = new StreamWriter(@"EFlog.txt", true))
                {
                    file.WriteLine(DateTime.Now + message + "\n====================================\n");
                }
            };

            // Passing an existing transaction to the context
            Context.Database.UseTransaction(_transaction);

            Context.Configuration.LazyLoadingEnabled = true;
        }

        static private MySqlTransaction _transaction;

        public static void CloseConnection()
        {
            Context.SaveChanges();
            _transaction.Commit();

            _connection.Close();
            Context = null;
        }


        public static Hured Context;

        public static void ExecuteExample()
        {

        }

        public static void InitDb()
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                using (var contextDb = new Hured(connection, false))
                {
                    contextDb.Database.CreateIfNotExists();
                }
            }
        }


        public static void Insert<T>(T item) where T : class
        {
            var table = Context.Set<T>();
            table.Add(item);
        }

        public static void Insert<T>(List<T> items) where T : class
        {
            var table = Context.Set<T>();

            table.AddRange(items);
        }

        public static T Find<T>(Expression<Func<T, bool>> predicate) where T : class
        {


            var table = Context.Set<T>();

            var result = table.FirstOrDefault(predicate);

            return result;

        }

        public static void Edit<T>(Expression<Func<T, bool>> predicate, T newItem) where T : class
        {
            var table = Context.Set<T>();

            var item = table.FirstOrDefault(predicate);

            foreach (var property in typeof(T).GetProperties())
            {
                var value = newItem.GetType().GetProperty(property.Name).GetValue(newItem);

                if (property.Name.Contains("Id") || Equals(property.GetValue(newItem), null))
                {
                    continue;
                }
                property.SetValue(item, value);
            }

        }

        public static void Remove<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var table = Context.Set<T>();

            var result = table.FirstOrDefault(predicate);

            table.Remove(result);
        }

        public static List<T> Select<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var table = Context.Set<T>();

            return table.Where(predicate).ToList();
        }

        public static bool Exists<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var result = Find(predicate);
            return result != null;
        }

        public static int RecordsCount<T>() where T : class
        {
            var table = Context.Set<T>();

            var result = table.Count();
            return result;
        }
    }
}
