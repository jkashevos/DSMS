using System.Collections.Generic;

namespace DSMS.Data.Models
{
    public class Complaint : BaseEntity
    {
        public Complaint()
        {
            this.Complaintants = new List<SchoolMeetingMember>();
            this.Violators = new List<SchoolMeetingMember>();
            this.Witnesses = new List<SchoolMeetingMember>();
        }

        public virtual string CaseNumber { get; set; }
        public virtual System.DateTime SubmitDate { get; set; }
        public virtual System.DateTime ViolationDate { get; set; }
        public virtual string Location { get; set; }
        public virtual string Description { get; set; }

        public virtual List<SchoolMeetingMember> Complaintants { get; set; }

        public virtual List<SchoolMeetingMember> Violators { get; set; }

        public virtual List<SchoolMeetingMember> Witnesses { get; set; }
    }
}