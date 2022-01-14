using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MilitaryCollege.Models
{
    public class Officer : SharedAttribute
    {
        public int Id { get; set; }
        
        [DisplayName("الصورة")]
        public string ProfileImage { get; set; }
        [Required(ErrorMessage ="الاسم مطلوب")]
        [DisplayName("الرتبة / الاسم والشهرة")]
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
        public DateTime BirthDate { get; set; }

        [DisplayName(" مكان القيد")]
        public string BirthAddress { get; set; }
        [DisplayName("مكان السكن")]
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
        public string RatingEnteringYear { get; set; }
        public string RatingEnteringNumber { get; set; }

        [DisplayName("الاختصاص")]
        public string Specialization { get; set; }
        public string RatingGradutionYear { get; set; }
        public string RatingGradutionNumber { get; set; }

        public string Notes { get; set; }
        public int TournamentId { get; set; }

     

        public ICollection<EducationalAttainment> EducationalAttainments { get; set; }
        public ICollection<Language> Languages { get; set; }
        public ICollection<Hobby> Hobbies { get; set; }
        public ICollection<DailyNote> DailyNotes { get; set; }
        public ICollection<DailyIncident> DailyIncidentents { get; set; }

    }
}
