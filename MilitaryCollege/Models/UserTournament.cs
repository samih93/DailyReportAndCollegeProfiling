using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


namespace MilitaryCollege.Models
{
    public class UserTournament
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int TournamentId { get; set; }

        [NotMapped]
        public List<IdentityUser> Users { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }

    }
}
