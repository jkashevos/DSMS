using System.Collections.Generic;

namespace DSMS.Data.Models
{
    public class Certification : BaseEntity
    {
        public Certification()
        {
            this.CertifiedMembers = new List<SchoolMeetingMember>();
        }
        public virtual string Name { get; set; }
        public virtual string Instruction { get; set; }
        public virtual string Procedure { get; set; }

        public virtual List<SchoolMeetingMember> CertifiedMembers { get; set; }
    }
}
