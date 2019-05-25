namespace %(ProjectName)%.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class %(SQLDatabaseName)%Entities : DbContext
    {
        public %(SQLDatabaseName)%Entities()
            : base("name=%(SQLDatabaseName)%")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<%(ProjectName)%>().ToTable("%(ProjectName)%");
        }
    
        public DbSet<%(ProjectName)%> %(SQLMainTableName)% { get; set; }
    }
}
