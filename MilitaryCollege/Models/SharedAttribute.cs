using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MilitaryCollege.Models
{
    public class SharedAttribute
    {
        [DisplayName("نشط")]
        public bool IsActive { get; set; }

        [DisplayName("تاريخ الإنشاء")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime CreationDate { set; get; }
    }
}
