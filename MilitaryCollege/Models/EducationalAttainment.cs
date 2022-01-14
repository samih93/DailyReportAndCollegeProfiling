using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MilitaryCollege.Models
{
    public class EducationalAttainment
    {
        public int Id { get; set; }
        [DisplayName("الشهادة")]

        public string Certificate { get; set; }

        [DisplayName("المصدر")]
        public string Source { get; set; }

        [DisplayName("التاريخ")]
        public string Date { get; set; }

        [DisplayName("(الدرجة(العلامة")]
        public string Grade { get; set; }
        public int OfficerId { get; set; }
        public Officer Officer { get; set; }
    }
}
