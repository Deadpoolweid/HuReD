using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.Entity;

namespace Hured.DBModel
{
        [DbConfigurationType(typeof(MySqlEFConfiguration))]
        public class Hured : DbContext
        {
            public DbSet<Сотрудник> Сотрудники { get; set; }

            public DbSet<ОсновнаяИнформация> СписокОсновнойИнформации { get; set; }

            public DbSet<Адрес> Адреса { get; set; }

            public DbSet<Образование> СписокОбразований { get; set; }

            public DbSet<ВоинскийУчёт> СписокВоинскогоУчёта { get; set; }

            public DbSet<Должность> Должности { get; set; }

            public DbSet<Подразделение> Подразделения { get; set; }


            public Hured()
              : base()
            {
                Database.SetInitializer(new MySqlInitializer());
            }

            // Constructor to use on a DbConnection that is already opened
            public Hured(DbConnection existingConnection, bool contextOwnsConnection)
              : base(existingConnection, contextOwnsConnection)
            {

            }
        }

        public class MySqlInitializer : IDatabaseInitializer<Hured>
        {
            public void InitializeDatabase(Hured context)
            {
                if (!context.Database.Exists())
                {
                    // if database did not exist before - create it
                    context.Database.Create();
                }
                else
                {
                    // query to check if MigrationHistory table is present in the database 
                    var migrationHistoryTableExists = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<int>(
                    string.Format(
                      "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = '{0}' AND table_name = '__MigrationHistory'",
                      "helpcontext"));

                    // if MigrationHistory table is not there (which is the case first time we run) - create it
                    if (migrationHistoryTableExists.FirstOrDefault() == 0)
                    {
                        context.Database.Delete();
                        context.Database.Create();
                    }
                }
            }
        }

        public class ProjectInitializer : MigrateDatabaseToLatestVersion<Hured, Configuration>
        {
        }

        public sealed class Configuration : DbMigrationsConfiguration<Hured>
        {
            public Configuration()
            {
                AutomaticMigrationsEnabled = true;
            }

            protected override void Seed(Hured context)
            {

            }
        }
}
