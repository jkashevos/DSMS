using System;

namespace DSMS.Data.Models
{
    public class SelectedChore : BaseEntity
    {
        public virtual System.DateTime SelectedDateTime { get; set; }

        public virtual Nullable<System.DateTime> TakenDateTime { get; set; }
        public virtual Nullable<System.DateTime> CheckedDateTime { get; set; }

        public virtual Chore Chore { get; set; }

        public virtual SchoolMeetingMember SelectedBySmm { get; set; }

        public virtual SchoolMeetingMember TakenBySmm { get; set; }

        public virtual SchoolMeetingMember CheckedBySmm { get; set; }
    }
}