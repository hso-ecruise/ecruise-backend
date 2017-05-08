using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ecruise.Database.Models;

namespace ecruise.Database
{
    class Test
    {
        void printCustomers()
        {
            var factory = new ecruiseContextFactory();
            var ctx = factory.Create(new DbContextFactoryOptions());

            foreach (var customer in ctx.Customers)
                Console.WriteLine("customer: " + customer);
        }
    }
}
