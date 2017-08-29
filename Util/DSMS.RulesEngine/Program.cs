using System;
using System.Timers;

using Microsoft.EntityFrameworkCore;

using DSMS.Data;
using DSMS.Data.Rules;
using System.Threading;
using System.Threading.Tasks;

namespace DSMS.RulesEngine
{
    class Program
    {
        private static System.Timers.Timer timer;
        static void Main(string[] args)
        {
            var context = new DSMSDBContext();

            if (!context.AllMigrationsApplied())
                context.Database.Migrate();
            context.EnsureSeedData();

            new Program().RunAsync().Wait();
        }

        private async Task RunAsync()
        {
            while (true)
            {
                await RuleManager.ExecuteAsync();
                Thread.Sleep(5 * 60 * 1000); //5 minutes
            }
        }
    }
}
