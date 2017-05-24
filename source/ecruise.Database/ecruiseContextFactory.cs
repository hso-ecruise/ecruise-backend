using ecruise.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ecruise.Database
{
    public class ecruiseContextFactory
        : IDbContextFactory<EcruiseContext>
    {
        public static string ConnectionString { private get; set; }

        public EcruiseContext Create(DbContextFactoryOptions options)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EcruiseContext>();
            optionsBuilder.UseMySql(ConnectionString);

            //Ensure database creation
            EcruiseContext ctx = new EcruiseContext(optionsBuilder.Options);
            ctx.Database.EnsureCreated();

            return ctx;
        }
    }
}
