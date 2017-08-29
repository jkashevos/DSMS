using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using DSMS.Data.Models;
using System.Collections.Generic;

namespace DSMS.Data
{
    public class DSMSDBContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<CalendarEvent> CalendarEvents { get; set; }
        public DbSet<Certification> Certifications { get; set; }
        public DbSet<Chore> Chores { get; set; }
        public DbSet<Committee> Committees { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<Fine> Fines { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SchoolMeetingMember> SchoolMeetingMembers { get; set; }
        public DbSet<SelectedChore> SelectedChores { get; set; }
        public DbSet<SystemEvent> SystemEvents { get; set; }
        public DbSet<SystemMessage> SystemMessages { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<Visitor> Visitors { get; set; }

        public DSMSDBContext() : base(new DbContextOptions<DSMSDBContext>()) { }

        public DSMSDBContext(DbContextOptions<DSMSDBContext> options) : base(options) { }

        /// <summary>
        /// Gets the school meeting members sorted.
        /// </summary>
        /// <value>
        /// The school meeting members sorted.
        /// </value>
        public List<SchoolMeetingMember> SchoolMeetingMembersSorted
        {
            get
            {
                List<SchoolMeetingMember> smm = this.SchoolMeetingMembers.ToList();
                smm.Sort((x, y) => x.FullName.CompareTo(y.FullName));
                return smm;
            }
        }

        /// <summary>
        /// Gets the chore checkers.
        /// </summary>
        /// <value>
        /// The chore checkers.
        /// </value>
        public List<SchoolMeetingMember> ChoreCheckers
        {
            get
            {
                List<SchoolMeetingMember> smm = this.SchoolMeetingMembers.ToList().Where(m => m.SignedIn && m.IsInRole("Chore Checker")).ToList();
                smm.Sort((x, y) => x.FullName.CompareTo(y.FullName));
                return smm;
            }
        }

        /// <summary>
        /// Gets the current visitors.
        /// </summary>
        /// <value>
        /// The current visitors.
        /// </value>
        public List<Visitor> CurrentVisitors
        {
            get
            {
                return this.Visitors.Where(v => v.TimeIn >= DateTime.Today && v.TimeOut == null).ToList();
            }
        }

        /// <summary>
        /// Gets the school start.
        /// </summary>
        /// <value>
        /// The school start.
        /// </value>
        public CalendarEvent SchoolStart
        {
            get
            {
                return this.CalendarEvents.AsEnumerable().LastOrDefault(e => e.CalendarDateTime <= DateTime.Now && e.CalendarEventType == (int)CalendarEventType.FirstDayOfSchool);
            }
        }

        /// <summary>
        /// Gets the school end.
        /// </summary>
        /// <value>
        /// The school end.
        /// </value>
        public CalendarEvent SchoolEnd
        {
            get
            {
                return this.CalendarEvents.AsEnumerable().LastOrDefault(e => e.CalendarDateTime >= DateTime.Now && e.CalendarEventType == (int)CalendarEventType.LastDayOfSchool);
            }
        }

        /// <summary>
        /// Gets the current calendar events.
        /// </summary>
        /// <value>
        /// The current calendar events.
        /// </value>
        public List<CalendarEvent> CurrentCalendarEvents
        {
            get
            {
                return CalendarEvents.Where(c => c.CalendarDateTime >= SchoolStart.CalendarDateTime && c.CalendarDateTime <= SchoolEnd.CalendarDateTime).ToList();
            }
        }

        /// <summary>
        /// Determines whether the specified date to check is a school day.
        /// </summary>
        /// <param name="DateToCheck">The date to check.</param>
        /// <returns></returns>
        public bool IsSchoolDay(DateTime DateToCheck)
        {
            //Date is before school start
            CalendarEvent schoolStart = this.CurrentCalendarEvents.FirstOrDefault(e => e.CalendarDateTime <= DateToCheck && e.CalendarEventType == (int)CalendarEventType.FirstDayOfSchool);
            if (schoolStart != null && DateToCheck.Date < schoolStart.CalendarDateTime)
                return false;

            //Date is not a week day
            //TODO: Make this a variable
            if (DateToCheck.DayOfWeek == DayOfWeek.Saturday || DateToCheck.DayOfWeek == DayOfWeek.Sunday)
                return false;

            //Date is not a day off
            if (this.CurrentCalendarEvents.ToList().Count(ce => ce.CalendarEventType == (int)CalendarEventType.NoSchool && ce.CalendarDateTime.Date == DateToCheck.Date) > 0)
                return false;

            //Date is after school end
            CalendarEvent schoolEnd = this.CurrentCalendarEvents.FirstOrDefault(e => e.CalendarDateTime >= DateToCheck && e.CalendarEventType == (int)CalendarEventType.LastDayOfSchool);
            if (schoolEnd != null && DateToCheck.Date > schoolEnd.CalendarDateTime)
                return false;

            return true;
        }

        public override int SaveChanges()
        {
            updateBaseEntityFields();
            return base.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            updateBaseEntityFields();
            return await this.SaveChangesAsync(CancellationToken.None);
        }

        private void updateBaseEntityFields()
        {
            var currentTime = DateTime.Now.ToUniversalTime();

            var entities = this.ChangeTracker
                .Entries()
                .Where(x => x.State == EntityState.Modified || x.State == EntityState.Added && x.Entity != null && typeof(BaseEntity).IsAssignableFrom(x.Entity.GetType()))
                .ToList();

            // Set the create/modified date as appropriate
            foreach (var entity in entities)
            {
                var entityBase = entity.Entity as BaseEntity;
                if (entity.State == EntityState.Added)
                {
                    entityBase.Created = currentTime;
                    //entityBase.CreateUserId = userId;
                }

                entityBase.Updated = currentTime;
                entityBase.Version++;
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
            if (builder.Options.Extensions.Where(e => e is Microsoft.EntityFrameworkCore.Infrastructure.Internal.SqliteOptionsExtension).Count() == 0)
                SqliteDbContextOptionsBuilderExtensions.UseSqlite(builder, "Data Source=DSMS.db", null);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
