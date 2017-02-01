using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using Hured.Tables_templates;
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

        public DbSet<УдостоверениеЛичности> УдостоверенияЛичности { get; set; }

        public DbSet<ПриказПриём> ПриказыПриём { get; set; }

        public DbSet<ПриказУвольнение> ПриказыУвольнение { get; set; }

        public DbSet<ПриказОтпуск> ПриказыОтпуск { get; set; }

        public DbSet<ПриказКомандировка> ПриказыКомандировка { get; set; }

        public DbSet<ТабельнаяЗапись> ТабельныеЗаписи { get; set; }

        public DbSet<Статус> Статусы { get; set; }

        public DbSet<ДополнительнаяИнформация> СписокДополнительнойИнформации { get; set; }

        public Hured()
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
                    $"SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = '{"helpcontext"}' AND table_name = '__MigrationHistory'");

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
