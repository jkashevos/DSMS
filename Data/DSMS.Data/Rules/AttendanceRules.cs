using System;
using System.Collections.Generic;
using System.Linq;

using DSMS.Data.Models;

namespace DSMS.Data.Rules
{
    public class AttendanceRules
    {
        public static void UpdateSystemEvents(DSMSDBContext Context)
        {
            var lastUpdate = Context.SystemSettings.FirstOrDefault(s => s.Name == "LastRuleExecute");
            var checkDate = DateTime.Parse(lastUpdate.Value);
            var endDate = DateTime.Now.Date.AddDays(-1);
            string incompleteReason = null;

            while (checkDate.Date <= endDate)
            {
                if (Context.IsSchoolDay(checkDate))
                {
                    foreach (SchoolMeetingMember member in Context.SchoolMeetingMembersSorted.Where(s => !s.IsStaff).ToList())
                    {
                        if (member.IsActive(checkDate))
                        {
                            SystemEvent signInEvent = member.SystemEvent.Where(e => e.SystemEventType == SystemEventType.SignIn && (e.EventDateTime >= checkDate.Date && e.EventDateTime <= checkDate.Date.AddDays(1))).FirstOrDefault();
                            SystemEvent signOutEvent = member.SystemEvent.Where(e => e.SystemEventType == SystemEventType.SignOut && (e.EventDateTime >= checkDate.Date && e.EventDateTime <= checkDate.Date.AddDays(1))).LastOrDefault();
                            List<SystemEvent> callEvents = member.SystemEvent.Where(e => e.SystemEventType == SystemEventType.CalledIn && (e.EventStartDateTime >= checkDate.Date && e.EventStartDateTime <= checkDate.Date.AddDays(1))).ToList();

                            bool incompleteDay = false;

                            if (signInEvent == null && signOutEvent == null)
                            {
                                //Did they call?                                
                                if (callEvents.Count == 0 || callEvents.Count(e => e.Reason == "In After 10:00") == 0)
                                {
                                    //if you don't sign in at all, and didn't call, it's an unexcused absence
                                    SystemEvent unexcused = new SystemEvent();
                                    unexcused.SchoolMeetingMember = member;
                                    unexcused.EventDateTime = checkDate;
                                    unexcused.EventType = (int)SystemEventType.UnexcusedAbsence;
                                    unexcused.Reason = "No Sign In, No Call.";
                                    Context.SystemEvents.Add(unexcused);

                                    //add fine
                                    var dbFine = Context.Fines.FirstOrDefault(f => f.FineDateTime == checkDate && f.FineReason == (int)FineReasonType.UnexcusedAbsence && f.SchoolMeetingMember.Id == member.Id);
                                    if (dbFine == null)
                                    {
                                        Fine fine = new Fine();
                                        fine.FineDateTime = checkDate;
                                        fine.Amount = 1.00M;
                                        fine.FineReason = (int)FineReasonType.UnexcusedAbsence;
                                        fine.SchoolMeetingMember = member;
                                        fine.Note = unexcused.Reason;
                                        Context.Fines.Add(fine);
                                    }
                                }
                                else
                                {
                                    //if you call, it's an excused absence
                                    var excused = new SystemEvent();
                                    excused.SchoolMeetingMember = member;
                                    excused.EventDateTime = checkDate;
                                    excused.EventType = (int)SystemEventType.ExcusedAbsence;
                                    excused.Reason = string.Format("Call: {0:t}", callEvents.Where(e => e.Reason == "In After 10:00").First().EventStartDateTime);
                                    Context.SystemEvents.Add(excused);
                                }
                            }
                            //sign in with no sign out
                            else if (signInEvent != null && signOutEvent == null)
                            {
                                var incomplete = new SystemEvent();
                                incomplete.SchoolMeetingMember = member;
                                incomplete.EventDateTime = checkDate;
                                incomplete.EventType = (int)SystemEventType.IncompleteDay;
                                incompleteReason = string.Format("Arrived: {0:t}, No Sign Out", signInEvent.EventDateTime);
                                incomplete.Reason = incompleteReason;
                                Context.SystemEvents.Add(incomplete);

                                incompleteDay = true;
                            }
                            //sign out with no sign in
                            else if (signInEvent == null && signOutEvent != null)
                            {
                                var incomplete = new SystemEvent();
                                incomplete.SchoolMeetingMember = member;
                                incomplete.EventDateTime = checkDate;
                                incomplete.EventType = (int)SystemEventType.IncompleteDay;
                                incompleteReason = string.Format("Signed Out: {0:t}, No Sign In", signOutEvent.EventDateTime);
                                incomplete.Reason = incompleteReason;
                                Context.SystemEvents.Add(incomplete);

                                incompleteDay = true;
                            }
                            else
                            {
                                //if you are not there between 10 and 2 it's incomplete
                                if (signInEvent.EventDateTime.TimeOfDay > new TimeSpan(10, 01, 0) || signOutEvent.EventDateTime.TimeOfDay < new TimeSpan(14, 0, 0))
                                {
                                    var incomplete = new SystemEvent();
                                    incomplete.SchoolMeetingMember = member;
                                    incomplete.EventDateTime = checkDate;
                                    incomplete.EventType = (int)SystemEventType.IncompleteDay;
                                    incompleteReason = string.Format("Arrived: {0:t}, Departed: {1:t}", signInEvent.EventDateTime, signOutEvent.EventDateTime);
                                    incomplete.Reason = incompleteReason;
                                    Context.SystemEvents.Add(incomplete);

                                    incompleteDay = true;
                                }
                                //and if you leave under 5 hours it's incomplete
                                else if (signOutEvent.EventDateTime.Subtract(signInEvent.EventDateTime) < new TimeSpan(5, 0, 0))
                                {
                                    var incomplete = new SystemEvent();
                                    incomplete.SchoolMeetingMember = member;
                                    incomplete.EventDateTime = checkDate;
                                    incomplete.EventType = (int)SystemEventType.IncompleteDay;
                                    incompleteReason = string.Format("Arrived: {0:t}, Departed: {1:t} ({2:hh\\:mm})", signInEvent.EventDateTime, signOutEvent.EventDateTime, signOutEvent.EventDateTime.Subtract(signInEvent.EventDateTime));
                                    incomplete.Reason = incompleteReason;
                                    Context.SystemEvents.Add(incomplete);

                                    incompleteDay = true;
                                }
                            }

                            if (incompleteDay)
                            {
                                if (signInEvent.EventDateTime.TimeOfDay > new TimeSpan(10, 01, 0) && (callEvents.Count == 0 || callEvents.Where(e => e.Reason == "In After 10:00").First().EventDateTime.TimeOfDay > new TimeSpan(10, 01, 0)))
                                {
                                    var dbFine = Context.Fines.FirstOrDefault(f => f.FineDateTime == checkDate && f.FineReason == (int)FineReasonType.IncompleteDay && f.SchoolMeetingMember.Id == member.Id);
                                    if (dbFine == null)
                                    {
                                        //add fine
                                        var fine = new Fine();
                                        fine.FineDateTime = checkDate;
                                        fine.Amount = 1.00M;
                                        fine.FineReason = (int)FineReasonType.IncompleteDay;
                                        fine.SchoolMeetingMember = member;
                                        fine.Note = incompleteReason;
                                        Context.Fines.Add(fine);
                                    }
                                }
                            }
                        }
                    }
                }

                //Update DB with current progress and increment the date.
                lastUpdate.Value = checkDate.ToString();
                Context.SaveChanges();
                checkDate = checkDate.AddDays(1);
            }
        }
    }
}