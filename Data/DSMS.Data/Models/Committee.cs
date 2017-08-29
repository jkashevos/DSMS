namespace DSMS.Data.Models
{
    public class Committee : BaseEntity
    {
        public virtual string Name { get; set; }
        public virtual string Abbreviation { get; set; }
        public virtual string Charter { get; set; }

        public virtual SchoolMeetingMember Chairman { get; set; }
        public virtual SchoolMeetingMember Secretary { get; set; }
        public virtual SchoolMeetingMember Treasurer { get; set; }
    }
}