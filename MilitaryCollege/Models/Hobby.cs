using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MilitaryCollege.Models
{
    public class Hobby 
    {
        public int Id { get; set; }
        [DisplayName("الهواية")]
        public string Name { get; set; }

        [DisplayName("شرحها")]
        public string Explanation{ get; set; }

        [DisplayName("المستوى")]
        public string  Level { get; set; }

        public int OfficerId { get; set; }
        public Officer Officer { get; set; }
    }
}
