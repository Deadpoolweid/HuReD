using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Hured.DBModel
{

    internal class Controller
    {
        private static string connectionString =
            "server=localhost;port=3306;database=Hured;uid=deadpoolweid;password=HERETIC23;persistsecurityinfo=True";

        private static MySqlConnection connection;

        public static void OpenConnection()
        {
            connection = new MySqlConnection(connectionString);
            context = new Hured(connection, false);
            connection.Open();
            transaction = connection.BeginTransaction();

            // Interception/SQL logging
            //context.Database.Log = (string message) => { Console.WriteLine(message + "\n====================================\n"); };
            context.Database.Log = (string message) =>
            {
                using (System.IO.StreamWriter file = new StreamWriter(@"EFlog.txt", true))
                {
                    file.WriteLine(DateTime.Now + message + "\n====================================\n");
                }
            };

            // Passing an existing transaction to the context
            context.Database.UseTransaction(transaction);

            context.Configuration.LazyLoadingEnabled = true;
        }

        static private MySqlTransaction transaction;

        public static void CloseConnection()
        {
            context.SaveChanges();
            transaction.Commit();

            connection.Close();
            context = null;
        }


        public static Hured context;

        public static void ExecuteExample()
        {
            
        }

        public static void InitDB(string connectionString = null)
        {
            connectionString =
                "server=localhost;port=3306;database=Hured;uid=deadpoolweid;password=HERETIC23;persistsecurityinfo=True";

            using (var connection = new MySqlConnection(connectionString))
            {
                using (var contextDB = new Hured(connection, false))
                {
                    contextDB.Database.CreateIfNotExists();
                }
            }
        }


        public static void Insert<T>(T Item) where T : class
        {

            var table = context.Set<T>();

            table.Add(Item);


        }

        public static void Insert<T>(List<T> Items) where T : class
        {
            var table = context.Set<T>();

            table.AddRange(Items);
        }

        public static T Find<T>(T type, Expression<Func<T, bool>> predicate) where T : class
        {


            var table = context.Set<T>();

            T result = table.FirstOrDefault(predicate);


            return result;

        }

        public static void Edit<T>(Expression<Func<T, bool>> predicate, T NewItem) where T : class
        {
            var table = context.Set<T>();

            var item = table.FirstOrDefault(predicate);

            foreach (var property in typeof(T).GetProperties())
            {
                var value = NewItem.GetType().GetProperty(property.Name).GetValue(NewItem);

                var temp = property.GetValue(NewItem,null);
                if (property.Name.Contains("Id") || Equals(property.GetValue(NewItem),null))
                {
                    continue;
                }
                property.SetValue(item, value);
            }

        }

        public static void Remove<T>(T type, Expression<Func<T, bool>> predicate) where T : class
        {
            var table = context.Set<T>();

            T result = table.FirstOrDefault(predicate);

            table.Remove(result);
        }

        public static List<T> Select<T>(T type, Expression<Func<T, bool>> predicate) where T : class
        {
            var table = context.Set<T>();

            return table.Where(predicate)?.ToList();
        }

        public static bool Exists<T>(T type, Expression<Func<T, bool>> predicate) where  T : class
        {
            var result = Find(type, predicate);
            return result != null;
        }

        public static int RecordsCount<T>() where T : class 
        {
            var table = context.Set<T>();

            var result = table.Count();
            return result;
        }
    }
}
