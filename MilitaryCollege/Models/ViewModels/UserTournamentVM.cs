﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MilitaryCollege.Models.ViewModels
{
    public class UserTournamentVM
    {
        public string  UserId { get; set; }
        public int TournamentId { get; set; }
        public string UserName { get; set; }
        public bool IsSelected { get; set; }
    }
}
