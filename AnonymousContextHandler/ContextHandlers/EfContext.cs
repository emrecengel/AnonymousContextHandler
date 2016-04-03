using System.Data.Entity;

namespace AnonymousContextHandler.ContextHandlers
{
    internal class EfContext<T> : DbContext where T : class
    {
        public EfContext(string connectionStringName) : base(connectionStringName)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<T>();
            Database.SetInitializer(new NullDatabaseInitializer<EfContext<T>>());
        }
    }
}