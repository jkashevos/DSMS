using System;
using System.ComponentModel.DataAnnotations;

namespace DSMS.Web.Models
{
    public class VisitorViewModel
    {
        public string Name { get; set; }
        public string ResponsiblePartyFullName { get; set; }
        public string ReasonForVisit { get; set; }

        [Display(Name = "Time In")]
        [Required(ErrorMessage = "Please enter a start date")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:t}")]
        public DateTime TimeIn { get; set; }
        public double ExpectedDuration { get; set; }
    }
}