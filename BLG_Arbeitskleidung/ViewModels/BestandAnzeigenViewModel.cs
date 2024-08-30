using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using Arbeitsbekleidung.Models.Models;
using Arbeitsbekleidung.Database.Database;

namespace BLG_Arbeitskleidung.ViewModels {
    public partial class BestandAnzeigenViewModel : ObservableObject {

        public ObservableCollection<Arbeitskleidung> Arbeitskleidungen { get; } = new();

        public Bestand Bestände { get; } = new();       
    

        public BLGBestandDbContext Database { get; set; }

        public BestandAnzeigenViewModel(BLGBestandDbContext database) {
            Database = database;
            foreach(Arbeitskleidung arbeitskleidung in Database.Arbeitskleidung
                .Include(x => x.Bestände)
                .ThenInclude(x => x.Lagerplatz)) {
                Arbeitskleidungen.Add(arbeitskleidung);
            }          
        }       
    }
}