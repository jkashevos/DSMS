using System;

namespace DSMS.Data.Models
{
    public class SystemMessage : BaseEntity
    {
        public virtual string Title { get; set; }
        public virtual string Message { get; set; }
        public virtual Nullable<System.DateTime> StartDate { get; set; }
        public virtual Nullable<System.DateTime> EndDate { get; set; }
    }
}