using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSMS.Data.Models
{
    public class Book : BaseEntity
    {
        public Book()
        {
            this.Authors = new List<Author>();
        }

        public virtual string Title { get; set; }
        public virtual string Summary { get; set; }
        public virtual string Isbn { get; set; }
        public virtual string Notes { get; set; }
        public virtual int PublisherId { get; set; }

        public virtual Publisher Publisher { get; set; }
        public virtual List<Author> Authors { get; set; }

        [NotMapped]
        public string ShortSummary
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Summary))
                {
                    if (this.Summary.Length > 100)
                        return string.Format("{0}...(more)", this.Summary.Substring(0, 100));
                    else
                        return this.Summary;
                }

                return null;
            }
        }
    }
}