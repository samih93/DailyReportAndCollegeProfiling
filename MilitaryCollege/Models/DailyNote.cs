using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MilitaryCollege.Models
{
    public class DailyNote :SharedAttribute
    {
        public int Id { get; set; }
        public string Note { get; set; }
        public bool IsPositive { get; set; }
        public bool IsImportant { get; set; }
        public int? OfficerId { get; set; }
        [NotMapped]
        public string OfficerName { get; set; }
        [NotMapped]
        public bool WantToDelete { get; set; }
        public Officer Officer { get; set; }
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; }
    }
}
