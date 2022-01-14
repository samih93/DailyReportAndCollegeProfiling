using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MilitaryCollege.Models.ViewModels
{
    public class OfficerVM
    {
        public OfficerVM()
        {
            this.EducationalAttainments = new List<EducationalAttainment>();
            this.Hobbies = new List<Hobby>();
            this.Languages = new List<Language>();
            this.DailyIncidentents = new HashSet<DailyIncident>();
            this.DailyNotes = new HashSet<DailyNote>();
        }
        public int Id { get; set; }


        [DisplayName("الصورة")]
        public IFormFile Profileimage { get; set; }
        public string  ExistingImage { get; set; }
        [Required(ErrorMessage = "الاسم مطلوب")]
        [DisplayName("الرتبة،الاسم الثلاثي ")]
        public string Name { get; set; }
        [DisplayName("الرقم المالي")]
        public int? MilitaryNumber { get; set; }
        [DisplayName("رقم الهاتف")]
        public string PhoneNumber { get; set; }

        [EmailAddress]

        [DisplayName("البريد الالكتروني")]
        public string Email { get; set; }

        [DisplayName(" تاريخ الولادة")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        

        public DateTime BirthDate { get; set; }

        [DisplayName(" مكان ورقم السجل")]
        public string BirthAddress { get; set; }
        [DisplayName("عنوان السكن")]
        public string Address { get; set; }

        [DisplayName("جنسية غير لبنانية")]
        public string Nationality { get; set; }

        [DisplayName("فئة الدم")]
        public string BloodType { get; set; }

        [DisplayName("الطول ")]
        public string Height { get; set; }

        [DisplayName("وظيفة سابقة")]
        public string PreviousJob { get; set; }

        [DisplayName("مشاكل صحية")]
        public string HealthProblem { get; set; }
        [DisplayName("عام")]
        public string RatingEnteringYear { get; set; }

        [DisplayName("قوى أمن")]
        public string RatingEnteringNumber { get; set; }
        [DisplayName("الاختصاص ")]
        public string Specialization { get; set; }



        [DisplayName("عام")]
        public string RatingGradutionYear { get; set; }
        [DisplayName("قوى أمن")]

        public string RatingGradutionNumber { get; set; }

        public string Notes { get; set; }
        public int TournamentId { get; set; }
        public string TournamentName { get; set; }


        public List<EducationalAttainment> EducationalAttainments { get; set; }
        public List<Language> Languages { get; set; }
        public List<Hobby> Hobbies { get; set; }
        public ICollection<DailyNote> DailyNotes { get; set; }
        public ICollection<DailyIncident> DailyIncidentents { get; set; }
        
        // count of incidents for an officer 
        public int TotalCountOfIncidentsForOfficer { get; set; }
        // sum of total days of incidents
        public int SumOfDaysInIncidentsForOfficer { get; set; }
    }
}
