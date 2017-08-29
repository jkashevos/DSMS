using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace DSMS.Data.Models
{
    public class SchoolMeetingMember : IdentityUser
    {
        public SchoolMeetingMember()
        {
            this.Fines = new List<Fine>();
            this.SystemEvent = new List<SystemEvent>();
            this.SchoolMeetingMemberRoles = new List<Role>();
        }

        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual System.DateTime DateOfBirth { get; set; }
        public virtual bool IsStaff { get; set; }
        public virtual string Phone { get; set; }

        public virtual List<Fine> Fines { get; set; }
        public virtual List<SystemEvent> SystemEvent { get; set; }
        public virtual List<Role> SchoolMeetingMemberRoles { get; set; }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        [NotMapped]
        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", this.FirstName, this.LastName);
            }
        }

        /// <summary>
        /// Gets the fine total.
        /// </summary>
        /// <value>
        /// The fine total.
        /// </value>
        [NotMapped]
        public decimal FineTotal
        {
            get
            {
                return ActiveFines.Sum(f => f.Amount);
            }
        }

        /// <summary>
        /// Gets the active fines.
        /// </summary>
        /// <value>
        /// The active fines.
        /// </value>
        [NotMapped]
        public List<Fine> ActiveFines
        {
            get { return this.Fines.Where(f => !f.Paid).ToList(); }
        }

        /// <summary>
        /// Gets a value indicating whether the smm is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [active]; otherwise, <c>false</c>.
        /// </value>
        [NotMapped]
        public bool Active
        {
            get
            {
                int withdrawEvent = this.SystemEvent.Count(e => e.SystemEventType == SystemEventType.Withdraw);
                return withdrawEvent == 0;
            }
        }

        [NotMapped]
        public int TotalSchoolDays
        {
            get
            {
                var context = new DSMSDBContext();

                var enrollEvent = this.SystemEvent.FirstOrDefault(e => e.SystemEventType == SystemEventType.Enroll);
                var withDrawEvent = this.SystemEvent.LastOrDefault(e => e.SystemEventType == SystemEventType.Withdraw);

                if (enrollEvent != null && withDrawEvent == null)
                    return Utils.GetSchoolDayCount(enrollEvent.EventDateTime);
                else if (enrollEvent == null && withDrawEvent != null)
                    return Utils.GetSchoolDayCount(null, withDrawEvent.EventDateTime);

                return Utils.GetSchoolDayCount();
            }
        }

        [NotMapped]
        public double YearTotalPersonalDays
        {
            get
            {
                return Math.Ceiling(TotalSchoolDays * .106);
            }
        }

        [NotMapped]
        public double PersonalDaysUsed
        {
            get
            {
                return Math.Round(this.ExcusedAbsenceCount + this.UnexcusedAbsenceCount + (this.IncompleteDayCount * .3333333), 2);
            }
        }

        [NotMapped]
        public double PersonalDaysRemaining
        {
            get
            {
                return Math.Round(YearTotalPersonalDays - PersonalDaysUsed, 2);
            }
        }

        /// <summary>
        /// Gets a value indicating whether [signed in].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [signed in]; otherwise, <c>false</c>.
        /// </value>
        [NotMapped]
        public bool SignedIn
        {
            get
            {
                int signInEvents = this.SystemEvent.Count(e => e.SystemEventType == SystemEventType.SignIn && (e.EventDateTime >= DateTime.Today && e.EventDateTime <= DateTime.Today.AddDays(1)));
                int signOutEvents = this.SystemEvent.Count(e => e.SystemEventType == SystemEventType.SignOut && (e.EventDateTime >= DateTime.Today && e.EventDateTime <= DateTime.Today.AddDays(1)));

                return signInEvents > signOutEvents;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [off campus].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [off campus]; otherwise, <c>false</c>.
        /// </value>
        [NotMapped]
        public bool OffCampus
        {
            get
            {
                int signInEvents = this.SystemEvent.Count(e => e.SystemEventType == SystemEventType.SignIn && (e.EventDateTime >= DateTime.Today && e.EventDateTime <= DateTime.Today.AddDays(1)));
                int signOutEvents = this.SystemEvent.Count(e => e.SystemEventType == SystemEventType.SignOut && (e.EventDateTime >= DateTime.Today && e.EventDateTime <= DateTime.Today.AddDays(1)));
                int offCampusEvents = this.SystemEvent.Count(e => e.SystemEventType == SystemEventType.OffCampus && (e.EventDateTime >= DateTime.Today && e.EventDateTime <= DateTime.Today.AddDays(1)));
                int onCampusEvents = this.SystemEvent.Count(e => e.SystemEventType == SystemEventType.OnCampus && (e.EventDateTime >= DateTime.Today && e.EventDateTime <= DateTime.Today.AddDays(1)));

                return (signInEvents > signOutEvents) && (offCampusEvents > onCampusEvents);
            }
        }

        private SystemEvent calledInRecord
        {
            get { return this.SystemEvent.LastOrDefault(e => e.SystemEventType == SystemEventType.CalledIn && e.EventStartDateTime.HasValue && e.EventStartDateTime.Value.Date == DateTime.Today); }
        }

        /// <summary>
        /// Gets a value indicating whether [called in].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [called in]; otherwise, <c>false</c>.
        /// </value>
        [NotMapped]
        public bool CalledIn
        {
            get
            {
                if (calledInRecord != null)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Gets the called in reason.
        /// </summary>
        /// <value>
        /// The called in reason.
        /// </value>
        [NotMapped]
        public string CalledInReason
        {
            get
            {
                if (calledInRecord != null)
                    return calledInRecord.Reason;

                return null;
            }
        }

        /// <summary>
        /// Gets the called in time.
        /// </summary>
        /// <value>
        /// The called in time.
        /// </value>
        [NotMapped]
        public DateTime? CalledInTime
        {
            get
            {
                if (calledInRecord != null)
                    return calledInRecord.EventStartDateTime;

                return null;
            }
        }

        /// <summary>
        /// Gets the called in time & reason.
        /// </summary>
        /// <value>
        /// The called in time & reason.
        /// </value>
        [NotMapped]
        public string CalledInTimeReason
        {
            get
            {
                if (calledInRecord != null)
                    return string.Format("{0:t} - {1}", calledInRecord.EventStartDateTime, calledInRecord.Reason);

                return null;
            }
        }

        /// <summary>
        /// Gets the sign in time.
        /// </summary>
        /// <value>
        /// The sign in time.
        /// </value>
        [NotMapped]
        public DateTime? SignInTime
        {
            get
            {
                var signInEvent = this.SystemEvent.Where(e => e.SystemEventType == SystemEventType.SignIn && (e.EventDateTime >= DateTime.Today && e.EventDateTime <= DateTime.Today.AddDays(1))).LastOrDefault();
                if (signInEvent != null)
                    return signInEvent.EventDateTime;

                return null;
            }
        }

        /// <summary>
        /// Gets the sign out time.
        /// </summary>
        /// <value>
        /// The sign out time.
        /// </value>
        [NotMapped]
        public DateTime? SignOutTime
        {
            get
            {
                var signOutEvent = this.SystemEvent.Where(e => e.SystemEventType == SystemEventType.SignOut && (e.EventDateTime >= DateTime.Today && e.EventDateTime <= DateTime.Today.AddDays(1))).LastOrDefault();
                if (signOutEvent != null)
                    return signOutEvent.EventDateTime;

                return null;
            }
        }

        [NotMapped]
        private SystemEvent offCampusRecord { get { return this.SystemEvent.Where(e => e.SystemEventType == SystemEventType.OffCampus).LastOrDefault(); } }

        /// <summary>
        /// Gets the off campus location.
        /// </summary>
        /// <value>
        /// The off campus location.
        /// </value>
        [NotMapped]
        public string OffCampusLocation { get { return offCampusRecord.Location; } }

        /// <summary>
        /// Gets the duration of the off campus.
        /// </summary>
        /// <value>
        /// The duration of the off campus.
        /// </value>
        [NotMapped]
        public double? OffCampusDuration { get { return (offCampusRecord.EventDateTime - (DateTime)offCampusRecord.EventEndDateTime).TotalMinutes; } }

        /// <summary>
        /// Gets the off campus phone.
        /// </summary>
        /// <value>
        /// The off campus phone.
        /// </value>
        [NotMapped]
        public string OffCampusPhone { get { return offCampusRecord.Phone; } }

        /// <summary>
        /// Gets the off campus record identifier.
        /// </summary>
        /// <value>
        /// The off campus record identifier.
        /// </value>
        [NotMapped]
        public int OffCampusRecordId { get { return offCampusRecord.Id; } }

        /// <summary>
        /// Gets the off campus time.
        /// </summary>
        /// <value>
        /// The off campus time.
        /// </value>
        [NotMapped]
        public DateTime OffCampusTime { get { return offCampusRecord.EventDateTime; } }

        /// <summary>
        /// Gets the est return time.
        /// </summary>
        /// <value>
        /// The est return time.
        /// </value>
        [NotMapped]
        public DateTime EstReturnTime
        {
            get { return offCampusRecord.EventEndDateTime.Value; }
        }

        /// <summary>
        /// Gets the excused absences.
        /// </summary>
        /// <value>
        /// The excused absences.
        /// </value>
        [NotMapped]
        public List<SystemEvent> ExcusedAbsences
        {
            get
            {
                //TODO: make it for this year
                return this.SystemEvent.Where(e => e.SystemEventType == SystemEventType.ExcusedAbsence).ToList();
            }
        }

        /// <summary>
        /// Gets the excused absence count.
        /// </summary>
        /// <value>
        /// The excused absence count.
        /// </value>
        [NotMapped]
        public int ExcusedAbsenceCount { get { return ExcusedAbsences.Count; } }

        /// <summary>
        /// Gets the unexcused absences.
        /// </summary>
        /// <value>
        /// The unexcused absences.
        /// </value>
        [NotMapped]
        public List<SystemEvent> UnexcusedAbsences
        {
            get
            {
                return this.SystemEvent.Where(e => e.SystemEventType == SystemEventType.UnexcusedAbsence).ToList();
            }
        }

        /// <summary>
        /// Gets the unexcused absence count.
        /// </summary>
        /// <value>
        /// The unexcused absence count.
        /// </value>
        [NotMapped]
        public int UnexcusedAbsenceCount { get { return UnexcusedAbsences.Count; } }

        /// <summary>
        /// Gets the incomplete.
        /// </summary>
        /// <value>
        /// The incomplete.
        /// </value>
        [NotMapped]
        public List<SystemEvent> IncompleteDays
        {
            get
            {
                return this.SystemEvent.Where(e => e.SystemEventType == SystemEventType.IncompleteDay).ToList();
            }
        }

        /// <summary>
        /// Gets the incomplete count.
        /// </summary>
        /// <value>
        /// The incomplete count.
        /// </value>
        [NotMapped]
        public int IncompleteDayCount { get { return IncompleteDays.Count; } }


        /// <summary>
        /// Determines whether the specified date is active.
        /// </summary>
        /// <param name="Date">The date.</param>
        /// <returns></returns>
        public bool IsActive(DateTime Date)
        {
            var enrollEvent = this.SystemEvent.FirstOrDefault(e => e.EventType == (int)SystemEventType.Enroll);
            var withdrawEvent = this.SystemEvent.LastOrDefault(e => e.EventType == (int)SystemEventType.Withdraw);
            return (enrollEvent == null || enrollEvent.EventDateTime < Date) && (withdrawEvent == null || withdrawEvent.EventDateTime > Date);
        }

        [NotMapped]
        public bool AbsenceLetter
        {
            get
            {
                var letterEvent = this.SystemEvent.FirstOrDefault(e => e.EventType == (int)SystemEventType.AbsenceLetterSent);
                return (letterEvent != null);
            }
        }

        /// <summary>
        /// Determines whether [is in role] [the specified role name].
        /// </summary>
        /// <param name="RoleName">Name of the role.</param>
        /// <returns></returns>
        public bool IsInRole(string RoleName)
        {
            return this.SchoolMeetingMemberRoles.Count(r => r.Name == RoleName) == 1;
        }

        [NotMapped]
        public int SundayHours { get; set; }
        [NotMapped]
        public int MondayHours { get; set; }
        [NotMapped]
        public int TuesdayHours { get; set; }
        [NotMapped]
        public int WednesdayHours { get; set; }
        [NotMapped]
        public int ThursdayHours { get; set; }
        [NotMapped]
        public int FridayHours { get; set; }
        [NotMapped]
        public int SaturdayHours { get; set; }
        [NotMapped]
        public int TotalHours { get { return SundayHours + MondayHours + TuesdayHours + WednesdayHours + ThursdayHours + FridayHours + SaturdayHours; } }
    }
}