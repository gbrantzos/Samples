using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EfCore
{
    /// <summary>
    /// Design time support
    /// </summary>
    public class SampleContextFactory : IDesignTimeDbContextFactory<SampleContext>
    {
        public SampleContext CreateDbContext(string[] args)
        {
            var connectionString = "Server=(local);Database=Sample;Trusted_Connection=true;";

            var optionsBuilder = new DbContextOptionsBuilder<SampleContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new SampleContext(optionsBuilder.Options);
        }
    }
}
