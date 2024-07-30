using Microsoft.EntityFrameworkCore;

namespace BLG_Arbeitskleidung.Database {
    public class BLGBestandDbContext : DbContext {
        public DbSet<Mitarbeiter> Mitarbeiter { get; set; }
        public DbSet<Lagerplatz> Lagerplatz {  get; set; }
        public DbSet<Arbeitskleidung> Arbeitskleidung {  get; set; }
        public DbSet<Log> Log { get; set; }
        public DbSet<Bestand> Bestand { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer($@"Server=SQL2022TOM;Database=BLGBestandDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }
}