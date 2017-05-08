using ecruise.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ecruise.Database
{
    public class ecruiseContextFactory
        : IDbContextFactory<ecruiseContext>
    {
        public static string ConnectionString { private get; set; }

        public ecruiseContext Create(DbContextFactoryOptions options)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ecruiseContext>();
            optionsBuilder.UseMySql(ConnectionString);

            //Ensure database creation
            ecruiseContext ctx = new ecruiseContext(optionsBuilder.Options);
            ctx.Database.EnsureCreated();

            return ctx;
        }
    }
}
