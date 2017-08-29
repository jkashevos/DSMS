using System;

namespace DSMS.Data.Models
{
    public class Fine : BaseEntity
    {
        public virtual decimal Amount { get; set; }
        public virtual System.DateTime FineDateTime { get; set; }
        public virtual int FineReason { get; set; }
        public virtual bool Paid { get; set; }
        public virtual Nullable<System.DateTime> PaidDateTime { get; set; }
        public virtual string Note { get; set; }

        public virtual SchoolMeetingMember SchoolMeetingMember { get; set; }
    }
}