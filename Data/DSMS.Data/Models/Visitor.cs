using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSMS.Data.Models
{
    public class Visitor : BaseEntity
    {
        [Display(Name="Name")]
        public virtual string Name { get; set; }
        public virtual string ReasonForVisit { get; set; }      
        public virtual System.DateTime TimeIn { get; set; }
        public virtual Nullable<System.DateTime> TimeOut { get; set; }
        public virtual Nullable<double> ExpectedDuration { get; set; }

        public virtual SchoolMeetingMember ResponsibleParty { get; set; }

        [NotMapped]
        public string ResponsiblePartyName
        {
            get { return this.ResponsibleParty.FullName; }
        }
    }
}