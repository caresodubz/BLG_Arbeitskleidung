using Arbeitsbekleidung.Database.Database;
using Arbeitsbekleidung.Models.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;

namespace BLG_Arbeitskleidung.ViewModels {
    partial class ArtikelBearbeitenViewModel : ObservableObject {
        public BLGBestandDbContext Database { get; set; }
        public ObservableCollection<Arbeitskleidung> Kleidung { get; set; } = [];
        public ObservableCollection<Arbeitskleidung> TempKleidung { get; set; }

        public ArtikelBearbeitenViewModel(BLGBestandDbContext database) {
            Database = database;
            foreach(Arbeitskleidung kleidung in Database.Arbeitskleidung) {
                Kleidung.Add(kleidung);
            }
        }

        [RelayCommand]
        protected void DatenbankÜberschreiben() {            
                MessageBox.Show(
                "Speichern war erfolgreich!",
                "Information",
                MessageBoxButton.OK,
                MessageBoxImage.Information
                );            

            try {                
                Database.SaveChanges();
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }        
    }
}