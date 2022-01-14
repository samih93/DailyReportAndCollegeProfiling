using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MilitaryCollege.Models
{
    public class Tournament :SharedAttribute
    {
        public int Id { get; set; }
        [DisplayName("اسم الدورة")]
        public string Name { get; set; }
        [DisplayName("ملاحظات")]
        public string Note { get; set; }
        [DisplayName("الصورة")]
        public string TournamentImage { get; set; }

        public ICollection<DailyReport> DailyReports { get; set; }
        public ICollection<DailyNote> DailyNotes { get; set; }
        public ICollection<DailyIncident> DailyIncidentents { get; set; }




    }
}
