using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSMS.Data.Models
{
    public class SystemEvent : BaseEntity
    {
        public virtual System.DateTime EventDateTime { get; set; }

        public virtual int EventType { get; set; }
        public virtual string Location { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Reason { get; set; }
        public virtual Nullable<System.DateTime> EventStartDateTime { get; set; }
        public virtual Nullable<System.DateTime> EventEndDateTime { get; set; }

        public virtual SchoolMeetingMember SchoolMeetingMember { get; set; }

        [NotMapped]
        public SystemEventType SystemEventType
        {
            get
            {
                object val = Enum.Parse(typeof(SystemEventType), this.EventType.ToString());

                if (val != null)
                    return (SystemEventType)val;

                return SystemEventType.None;
            }
            set
            {
                this.EventType = (int)value;
            }
        }

        [NotMapped]
        public string SystemEventTypeName
        {
            get { return Utils.GetSpacedString(SystemEventType.ToString()); }
        }

        [NotMapped]
        public string FullName
        {
            get
            {
                if (this.SchoolMeetingMember != null)
                    return this.SchoolMeetingMember.FullName;

                return null;
            }
        }

        [NotMapped]
        public DateTime EventDisplayDate
        {
            get
            {
                if (EventStartDateTime.HasValue)
                    return EventStartDateTime.Value;

                return EventDateTime;
            }
        }
    }
}