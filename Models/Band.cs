using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShowScheduler.Models
{
    public class Band
    {
        public int ID { get; set; }

        [ForeignKey("Show")]
        public int ShowID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Band Name")]
        public string BandName { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm tt}")]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm tt}")]
        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }
        public Show Show { get; set; }

    }
}
