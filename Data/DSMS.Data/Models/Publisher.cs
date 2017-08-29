using System.Collections.Generic;

namespace DSMS.Data.Models
{
    public class Publisher : BaseEntity
    {
        public Publisher()
        {
            this.Books = new List<Book>();
        }

        public virtual string Name { get; set; }

        public virtual List<Book> Books { get; set; }
    }
}