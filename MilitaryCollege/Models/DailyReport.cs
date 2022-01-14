using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MilitaryCollege.Models
{
    public class DailyReport : SharedAttribute
    {
        public int Id { get; set; }
        public DateTime ReportDate { get; set; }
        [Required(ErrorMessage="العدد الاساسي مطلوب")]
        public int BaseNumber { get; set; }

        [Required(ErrorMessage = "العدد الجاهز مطلوب")]
        public int ReadyNumber { get; set; }

        [Required(ErrorMessage = "العدد الغير جاهز مطلوب")]
        public int NotReadyNumber { get; set; }

        [NotMapped]
        public bool WantToDelete { get; set; }
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; }


    }
}
