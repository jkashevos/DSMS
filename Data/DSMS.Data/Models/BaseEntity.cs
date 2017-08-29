using System;
using System.ComponentModel.DataAnnotations;

namespace DSMS.Data.Models
{
    public class BaseEntity
    {
        [Key]
        public virtual int Id { get; set; }

        public DateTime? Created { get; set; }
		
        public DateTime? Updated { get; set; }

        public int Version { get; set; }
    }
}
