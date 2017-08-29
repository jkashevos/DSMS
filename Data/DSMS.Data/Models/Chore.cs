using System.Collections.Generic;

namespace DSMS.Data.Models
{
    public class Chore : BaseEntity
    {
        public Chore()
        {
            this.SelectedChore = new List<SelectedChore>();
        }

        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual int Qty { get; set; }
        public virtual bool Active { get; set; }

        public virtual List<SelectedChore> SelectedChore { get; set; }
    }
}