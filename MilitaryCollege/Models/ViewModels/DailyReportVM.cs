using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MilitaryCollege.Models.ViewModels
{
    public class DailyReportVM
    {
        public DailyReportVM()
        {
            
            // no need cz in create http get i retreive the  dailyincidents from database
           // this.DailyIncidents = new List<DailyIncident>();
        }
        public int ReportId { get; set; }
        public int TournamentId { get; set; }
        public string TournamentName { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ReportDate { get; set; }
        public int BaseNumber { get; set; }

        public int ReadyNumber { get; set; }

        public int NotReadyNumber { get; set; }

        public bool WantToDelete { get; set; }
        public IEnumerable<SelectListItem> DDLReason { get; set; }



        public List<DailyIncident> DailyIncidents { get; set; }
        public List<DailyIncident> TodayDailyIncidents { get; set; }

        public int CountDailyNotePosted { get; set; }
        public List<DailyNote> DailyNotes { get; set; }

    }
}
