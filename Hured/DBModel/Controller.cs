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
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                //// Create database if not exists
                using (var contextDB = new Hured(connection, false))
                {
                    contextDB.Database.CreateIfNotExists();
                }

                connection.Open();
                MySqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // DbConnection that is already opened
                    using (Hured context = new Hured(connection, false))
                    {

                        // Interception/SQL logging
                        context.Database.Log =
                            (string message) =>
                            {
                                Console.WriteLine(message + "\n====================================\n");
                            };

                        // Passing an existing transaction to the context
                        context.Database.UseTransaction(transaction);

                        Сотрудник Пример = new Сотрудник()
                        {
                            Адрес = new Адрес()
                            {
                                НаселённыйПункт = "Тула",
                                Улица = "Николая Руднева",
                                Индекс = "300000",
                                Дом = "67",
                                Квартира = "520"
                            },
                            ОсновнаяИнформация = new ОсновнаяИнформация()
                            {
                                Фамилия = "Колесников",
                                Имя = "Александр",
                                Отчество = "Алексеевич",
                                Пол = "Мужской",
                                ДатаПриема = DateTime.Today,
                                ДомашнийТелефон = "84874652574",
                                МобильныйТелефон = "89534321546",
                                ТабельныйНомер = "0875647",
                                РегистрационныйНомерОТРР = "456456456762165456",
                                Статус = "Стажер",
                                ИНН = "4654668715771645",
                                Дополнительно = "Дополнительная Информация",
                                Должность = new Должность()
                                {
                                    Подразделение = new Подразделение()
                                    {
                                        Название = "Отдел инфраструктурных сервисов",
                                    },
                                    Название = "Практикант",
                                    Расписание = "24/7"
                                },
                            },
                            Образование = new Образование()
                            {
                                Документ = "Диплом",
                                НачалоОбучения = new DateTime(2013, 9, 1),
                                КонецОбучения = new DateTime(2017, 5, 30),
                                ИностранныеЯзыки = "Английский",
                                Квалификация = "Прикладная информатика",
                                Серия = "7009",
                                Номер = "187103",
                                Тип = "Высшее",
                                Учреждение = "ТГПУ им. Л.Н.Толстого",
                                Специальность = "Прикладная информатика в здравоохранении",
                                Дополнительно = "Дополнительная информация"
                            },
                            ВоинскийУчёт = new ВоинскийУчёт()
                            {
                                КатегорияГодности = "Г4",
                                СостоитНаУчете = "Да",
                                НаименованиеВоенкомата = "г. Новомосковск ВУС",
                                Профиль = "",
                                КодВУС = "145785644"
                            }
                        };

                        //context.Сотрудники.Add(Пример);

                        //var ex = new Подразделение()
                        //{
                        //    Название = "Отдел информационной безопасности",
                        //};

                        ////Controller.Insert(ex);
                        //context.Подразделения.Add(ex);

                        var ex = context.Подразделения.FirstOrDefault(n => n.ПодразделениеId == 15);



                        var должность = new Должность()
                        {
                            Подразделение = ex,
                            Название = "Ведущий разработчик",
                            Расписание = "3/2/2",
                        };

                        //Controller.Insert(должность);
                        context.Должности.Add(должность);

                        должность = new Должность()
                        {
                            Подразделение = ex,
                            Название = "Начальник отдела",
                            Расписание = "5/2",
                        };

                        context.Должности.Add(должность);


                        //Controller.Insert(должность);

                        context.SaveChanges();
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
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

            foreach (var property in item.GetType().GetProperties())
            {
                var value = NewItem.GetType().GetProperty(property.Name).GetValue(NewItem);
                if (property.Name.Contains("Id") || property.GetValue(NewItem) == null)
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
    }
}
