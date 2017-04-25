using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Hured.Tables_templates;
using MySql.Data.MySqlClient;

namespace Hured.DataBase
{
    public static class Controller
    {
        private static string _connectionString = String.Empty;

        public static void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }

        private static MySqlConnection _connection;

        public static void OpenConnection()
        {
            if (IsConnectionOpened)
            {
                throw new Exception("Соединение уже открыто. Вы не можете открыть больше одного соединения за раз.");
            }



            _connection = new MySqlConnection(_connectionString);
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


            var countOfRows = Context.Sessions.Count();
            if (countOfRows != 0)
            {
                var lastSession = Context.Sessions.OrderBy(c => 1 == 1).Skip(countOfRows - 1).FirstOrDefault();
                if (lastSession.Status == UserStatus.Working)
                {
                    _connection.Close();
                    Context = null;

                    IsConnectionOpened = false;
                    return;
                }
            }


            IsConnectionOpened = true;

            ConnectionOpened?.Invoke();
        }

        private static MySqlTransaction _transaction;

        public static void CloseConnection(bool force = false)
        {
            if (!IsConnectionOpened) return;

            ConnectionClosing?.Invoke();

            if (!force)
            {
                Context.SaveChanges();
                _transaction.Commit();
            }

            _connection.Close();
            Context = null;

            IsConnectionOpened = false;
        }

        public static Hured Context;

        public static bool IsConnectionOpened = false;

        public static void InitDb()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                using (var contextDb = new Hured(connection, false))
                {
                    contextDb.Database.CreateIfNotExists();
                }
            }
        }

        public static bool IsConnectionSucceded(string connectionString = null)
        {
            if (connectionString == null)
            {
                connectionString = _connectionString;
            }

            try
            {
                using (var sqlConn =
                    new MySqlConnection(connectionString))
                {
                    sqlConn.Open();
                    return (sqlConn.State == ConnectionState.Open);
                }
            }
            catch (SqlException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
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

        public static bool Edit<T>(Expression<Func<T, bool>> predicate, T newItem) where T : class
        {
            var table = Context.Set<T>();

            var item = table.FirstOrDefault(predicate);

            if (item == null) return false;


            try
            {
                foreach (var property in typeof(T).GetProperties())
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

        public static bool Remove<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var table = Context.Set<T>();

            var result = table.FirstOrDefault(predicate);

            if (result == null)
            {
                return false;
            }

            table.Remove(result);
            return true;
        }

        public static List<T> Select<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var table = Context.Set<T>();

            return table.Where(predicate).ToList();
        }

        public static List<T> SelectAll<T>() where T : class
        {
            var table = Context.Set<T>();
            return table.ToList();
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

        public static void ExportDataBase(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        mb.ExportToFile(path);
                        conn.Close();
                    }
                }
            }
        }

        public static void ImportDataBase(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }

            InitDb();
            

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        mb.ImportFromFile(path);
                        conn.Close();
                    }
                }
            }
        }

        public delegate void DataAccessed();

        public static event DataAccessed ConnectionOpened;

        public static event DataAccessed ConnectionClosing;
    }
}
