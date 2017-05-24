using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ecruise.Database
{
    class Test
    {
        void printCustomers()
        {
            var factory = new EcruiseContextFactory();
            var ctx = factory.Create(new DbContextFactoryOptions());

            foreach (var customer in ctx.Customers)
                System.Console.WriteLine("customer: " + customer);
        }
    }
}
