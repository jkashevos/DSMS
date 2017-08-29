using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

using DSMS.Data.Models;

namespace DSMS.Data
{
    public static class DSMSDBContextExtensions
    {
        public static void EnsureSeedData(this DSMSDBContext context)
        {
            if (context.AllMigrationsApplied())
            {
                if (!context.CalendarEvents.Any())
                {
                    context.CalendarEvents.Add(new CalendarEvent() { CalendarDateTime = DateTime.Parse("08/01/" + DateTime.Now.Year), CalendarEventType = (int)CalendarEventType.FirstDayOfSchool });
                    context.CalendarEvents.Add(new CalendarEvent() { CalendarDateTime = DateTime.Parse("05/01/" + (DateTime.Now.Year + 1)), CalendarEventType = (int)CalendarEventType.LastDayOfSchool });
                }
                
                if (!context.SystemMessages.Any())
                {
                    context.SystemMessages.Add(new SystemMessage() { Title = "Test Message", Message = "This is a message for everyone" });
                }
                
                SchoolMeetingMember smm = null;
                if (!context.SchoolMeetingMembers.Any())
                {
                    smm = new SchoolMeetingMember() { FirstName = "Gary", LastName = "GoodTwoShoes" };
                    context.SchoolMeetingMembers.Add(smm);
                }
                else
                {
                    smm = context.SchoolMeetingMembers.First();
                }
                
                if (!context.Visitors.Any())
                {
                    context.Visitors.Add(new Visitor() { Name = "Ran Dumb", ResponsibleParty = smm, ReasonForVisit = "Goofing Off", TimeIn = DateTime.Now, ExpectedDuration = 2.0 });
                }
                
                context.SaveChanges();
            }
        }

        public static bool AllMigrationsApplied(this DSMSDBContext context)
        {
            var applied = context.GetService<IHistoryRepository>()
                .GetAppliedMigrations()
                .Select(m => m.MigrationId);

            var total = context.GetService<IMigrationsAssembly>()
                .Migrations
                .Select(m => m.Key);

            return !total.Except(applied).Any();
        }
    }
}