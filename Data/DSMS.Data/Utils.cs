using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DSMS.Data
{
    public sealed class Utils
    {
        public static string GetSpacedString(string StringVal)
        {
            Regex regex = new Regex("(?<!(^|[A-Z]))(?=[A-Z])|(?<!^)(?=[A-Z][a-z])");
            string[] values = regex.Split(StringVal);
            return string.Join(" ", values);
        }

        public static List<BindableEnum> GetBindableEnumList(Type EnumType)
        {
            List<BindableEnum> retList = new List<BindableEnum>();
            List<string> enumList = Enum.GetNames(EnumType).ToList();
            foreach (string enumVal in enumList)
                retList.Add(new BindableEnum(GetSpacedString(enumVal), (int)Enum.Parse(EnumType, enumVal)));

            return retList;
        }

        public static List<string> GetTimeList(int Interval)
        {
            List<string> times = new List<string>();

            DateTime startTime = RoundUp(DateTime.Now, Interval);
            int i = 0;
            while (startTime.AddMinutes(Interval * i).TimeOfDay.Hours < 17)
            {
                times.Add(startTime.AddMinutes(Interval * i).ToShortTimeString());
                i++;
            }

            return times;
        }

        public static DateTime RoundUp(DateTime dt, int Interval)
        {
            TimeSpan d = TimeSpan.FromMinutes(Interval);
            return new DateTime(((dt.Ticks + d.Ticks - 1) / d.Ticks) * d.Ticks);
        }

        public class BindableEnum
        {
            public BindableEnum(string Name, int Value)
            {
                this.Name = Name;
                this.Value = Value;
            }

            public string Name { get; set; }
            public int Value { get; set; }
        }

        private static int normalSchoolDayCount = -1;

        /// <summary>
        /// Returns the number of school days in a given year.
        /// </summary>
        /// <param name="StartDate">The start date.</param>
        /// <returns></returns>
        public static int GetSchoolDayCount(DateTime? StartDate = null, DateTime? EndDate = null)
        {
            int retVal = 1;
            DateTime startDate;
            DateTime endDate;

            var context = new DSMSDBContext();

            if (!StartDate.HasValue && !EndDate.HasValue && normalSchoolDayCount > 0)
                return normalSchoolDayCount;

            if (StartDate.HasValue && StartDate.Value > context.SchoolStart.CalendarDateTime)
                startDate = StartDate.Value;
            else
                startDate = context.CurrentCalendarEvents.FirstOrDefault(e => e.CalendarDateTime <= DateTime.Now && e.CalendarEventType == (int)CalendarEventType.FirstDayOfSchool).CalendarDateTime;

            if (EndDate.HasValue && StartDate.Value > context.SchoolEnd.CalendarDateTime)
                endDate = EndDate.Value;
            else
                endDate = context.CurrentCalendarEvents.FirstOrDefault(e => e.CalendarDateTime >= DateTime.Now && e.CalendarEventType == (int)CalendarEventType.LastDayOfSchool).CalendarDateTime;

            while (startDate < endDate)
            {
                if (context.IsSchoolDay(startDate))
                    retVal++;

                startDate = startDate.AddDays(1);
            }

            //Store it if it's a normal year count
            if (!StartDate.HasValue && !EndDate.HasValue)
                normalSchoolDayCount = retVal;

            return retVal;
        }
    }
}
