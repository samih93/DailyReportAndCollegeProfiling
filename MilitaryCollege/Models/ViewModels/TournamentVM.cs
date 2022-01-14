using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MilitaryCollege.Models.ViewModels
{
    public class TournamentVM
    {
        public int Id { get; set; }


        
        [DisplayName("اسم الدورة")]
        public string Name { get; set; }
        [DisplayName("ملاحظات")]
        public string Note { get; set; }
        [DisplayName("الصورة")]
        public IFormFile TournamentImage { get; set; }
        public string ExistingTournamentImage { get; set; }

        public bool IsActive { get; set; }
    }
}
