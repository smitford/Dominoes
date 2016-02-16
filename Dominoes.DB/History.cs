namespace Dominoes.DB
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class HistoryDb : DbContext
    {
        // Your context has been configured to use a 'History' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'Dominoes.DB.History' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'History' 
        // connection string in the application configuration file.
        public HistoryDb()
            : base("data source=(LocalDb)\\MSSQLLocalDB;initial catalog=Dominoes.DB.History;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework")
        {
        }
        public virtual DbSet<Result> Results { get; set; }
        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}