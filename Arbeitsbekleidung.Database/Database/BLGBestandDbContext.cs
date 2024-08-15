using Arbeitsbekleidung.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Arbeitsbekleidung.Database.Database {
    public class BLGBestandDbContext : DbContext {
        public DbSet<Mitarbeiter> Mitarbeiter { get; set; }
        public DbSet<Lagerplatz> Lagerplatz {  get; set; }
        public DbSet<Arbeitskleidung> Arbeitskleidung {  get; set; }
        public DbSet<Log> Log { get; set; }
        public DbSet<Bestand> Bestand { get; set; }


        private readonly string _connectionString;

        public IConfigurationRoot Configuration { get; set; } 

        public BLGBestandDbContext() {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            _connectionString = Configuration.GetConnectionString("secondConnection")!;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if(!optionsBuilder.IsConfigured) {
                optionsBuilder.UseSqlServer(_connectionString);
            }
            //optionsBuilder.UseSqlServer($@"Server=SQL2022TOM;Database=BLGBestandDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        public class AppSettings {
            public ConnectionStrings ConnectionString = new();
        }

        public class ConnectionStrings {
            public string DefaultConnection { get; set; } = string.Empty;
        }
    }
}