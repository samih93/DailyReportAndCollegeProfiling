using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace MilitaryCollege.Models
{
    public class MilitaryContext : IdentityDbContext
    {
        public MilitaryContext(DbContextOptions<MilitaryContext> options)
      : base(options)
        { }

        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<Hobby> Hobbies { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Officer> Officers { get; set; }
        public DbSet<EducationalAttainment> EducationalAttainments { get; set; }
        public DbSet<DailyReport> DailyReports { get; set; }
        public DbSet<DailyNote> DailyNotes { get; set; }
        public DbSet<DailyIncident> DailyIncidents { get; set; }
        public DbSet<ReasonOfIncident> ReasonOfIncidents { get; set; }
        public DbSet<UserTournament> UserTournaments { get; set; }
        







    }
}
