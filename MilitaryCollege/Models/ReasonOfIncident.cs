
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MilitaryCollege.Models
{
    public class ReasonOfIncident
    {
        public int Id { get; set; }
        [DisplayName("السبب")]
        public string Name { get; set; }
    }
}
