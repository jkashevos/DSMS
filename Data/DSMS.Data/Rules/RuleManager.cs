using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using DSMS.Data.Models;

namespace DSMS.Data.Rules
{
     public static class RuleManager
    {
        public static async Task ExecuteAsync()
        {
            using (var context = new DSMSDBContext())
            {
                var lastUpdate = await context.SystemSettings.FirstOrDefaultAsync(s => s.Name == "LastRuleExecute");
                if (lastUpdate == null)
                {
                    var ce = context.CalendarEvents.ToList().LastOrDefault(e => e.CalendarEventType == (int)CalendarEventType.FirstDayOfSchool);
                    lastUpdate = new SystemSetting("LastRuleExecute", ce.CalendarDateTime.ToString());
                    context.SystemSettings.Add(lastUpdate);
                    context.SaveChanges();
                }

                AttendanceRules.UpdateSystemEvents(context);

                lastUpdate.Value = DateTime.Now.ToString();
                context.SaveChanges();
            }
        }
    }
}
