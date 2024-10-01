using Arbeitsbekleidung.Database.Database;
using Arbeitsbekleidung.Models.Models;
using BLG_Arbeitskleidung.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BLG_Arbeitskleidung.ViewModels {
    partial class ArtikelBearbeitenViewModel : ObservableObject {
        public BLGBestandDbContext Database { get; set; }
        public ObservableCollection<Arbeitskleidung> Kleidung { get; set; } = [];


        public ArtikelBearbeitenViewModel(BLGBestandDbContext database) {
            Database = database;
            foreach(Arbeitskleidung kleidung in Database.Arbeitskleidung) {
                Kleidung.Add(kleidung);
            }
        }        
    }
}