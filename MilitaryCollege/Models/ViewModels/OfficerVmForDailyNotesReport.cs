using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MilitaryCollege.Models.ViewModels
{
    public class OfficerVmForDailyNotesReport
    {
        public string Name { get; set; }
        public ICollection<DailyNote> DailyNotesPositive { get; set; }
        public ICollection<DailyNote> DailyNotesNegative { get; set; }
        public ICollection<DailyNote> DailyNotes { get; set; }
        public int CountMaxNotes { get; set; }
    }

}
