using System.Collections.Generic;

namespace DSMS.Data.Models
{
    public class Role : BaseEntity
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
    }
}