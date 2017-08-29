using System;

using Microsoft.EntityFrameworkCore;

using DSMS.Data;

namespace DataTest
{
    class Program
    {
        static void Main(string[] args)
        {
           var context = new DSMSDBContext();

           if (!context.AllMigrationsApplied())
                context.Database.Migrate();
            context.EnsureSeedData();
        }
    }
}
