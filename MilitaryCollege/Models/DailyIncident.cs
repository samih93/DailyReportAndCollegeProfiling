using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime;
using System.Runtime.InteropServices;

namespace MilitaryCollege.Models
{
    public class DailyIncident :SharedAttribute
    {
        public int Id { get; set; }
       
        [DisplayName("ملاحظات")]
        public string Explanation { get; set; }


        [DisplayName("من")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? StartDate { get; set; }
        [DisplayName("الى")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? EndDate { get; set; }
        [DisplayName("إلتحق")]
        public bool IsFinish { get; set; }


        public int CountDaysOfIncident { get; set; }

        [NotMapped]
        [DisplayName("عدم احتساب الايام")]
        // if reason is tadaroj mabhsb ayam lwouku3at
        public bool NotCountDayOfIncident { get; set; }
        [NotMapped]
        public bool WantToDelete { get; set; }

        public int? ReasonOfIncidentId { get; set; }
        public ReasonOfIncident ReasonOfIncident { get; set; }

        public int? OfficerId { get; set; }
        [NotMapped]
        [DisplayName("الاسم")]

        public string OfficerName { get; set; }
        public Officer Officer { get; set; }
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; }
    }
}