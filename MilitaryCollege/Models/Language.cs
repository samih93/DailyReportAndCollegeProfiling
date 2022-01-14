using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MilitaryCollege.Models
{
    public class Language 
    {
        public int Id { get; set; }
        [DisplayName("اللغة")]
        public string  Name { get; set; }

        [DisplayName("الاستماع")]
        public string Listen { get; set; }

        [DisplayName("التكلم")]
        public string Speak { get; set; }

        [DisplayName("القراءة")]
        public string Read { get; set; }

        [DisplayName("الكتابة")]
        public string Write { get; set; }

        public int OfficerId { get; set; }
        public Officer Officer { get; set; }

    }
}
