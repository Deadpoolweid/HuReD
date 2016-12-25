using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.Entity;
using MySql.Data.MySqlClient;

namespace DataModel
{
    public class Color
    {
        public Color(string name)
        {
            this.Name = name;
        }

        public int ColorId { get; set; }

        public string Name { get; set; }

        //public Cat cat { get; set; }
    }

    public class Cat
    {
        public Cat(string name)
        {
            this.Name = name;
        }

        public int CatId { get; set; }

        public string Name { get; set; }

        //public virtual List<Color> Colors { get; set; }
    }

    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class Context : DbContext
    {
        public Context() : base("Hured")
        {
            Database.SetInitializer(
                new DropCreateDatabaseIfModelChanges<Context>());
        }

        public Context(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Cat>().MapToStoredProcedures();
            modelBuilder.Entity<Color>().MapToStoredProcedures();
        }

        public DbSet<Color> Colors { get; set; }
        public DbSet<Cat> Cats { get; set; }
    }
  
    // -----------------------------------------------------------------------------
}