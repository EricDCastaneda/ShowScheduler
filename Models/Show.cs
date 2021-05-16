using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShowScheduler.Models
{
    public class Show
    {
        public int ID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Show Name")]
        public string ShowName { get; set; }

        [Required]
        [StringLength(50)]
        public string Venue { get; set; }
        public ICollection<Band> Bands { get; set; }
    }
}
