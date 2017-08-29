using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSMS.Data.Models
{
    public class CalendarEvent : BaseEntity
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual int CalendarEventType { get; set; }
        public virtual System.DateTime CalendarDateTime { get; set; }

          [NotMapped]
        public string EventTypeString
        {
            get
            {
                return Utils.GetSpacedString(Enum.GetName(typeof(DSMS.Data.CalendarEventType), CalendarEventType));
            }
        }
    }
}