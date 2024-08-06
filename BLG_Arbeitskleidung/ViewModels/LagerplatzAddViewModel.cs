using BLG_Arbeitskleidung.Database;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;

namespace BLG_Arbeitskleidung.ViewModels {
    public partial class LagerplatzAddViewModel : ObservableObject {

        [ObservableProperty]
        protected string _LagerplatzName = string.Empty;

        [ObservableProperty]
        protected ObservableCollection<Lagerplatz> _Lagerplätze = [];

        public BLGBestandDbContext Database { get; set; }

        public LagerplatzAddViewModel(BLGBestandDbContext database) {
            Database = database;
            foreach(Lagerplatz lagerplatz in Database.Lagerplatz.Include(x => x.Bestände).ToList().OrderBy(x => x.lpz_bezeichnung)) {
                Lagerplätze.Add(lagerplatz);
            }
        }

        [RelayCommand]
        protected void LagerplatzLöschen(Lagerplatz lagerplatz) {
            lagerplatz = Database.Lagerplatz.Include(x => x.Bestände).First(x => x.lagerp_id == lagerplatz.lagerp_id);
             
            if(lagerplatz.Bestände.Count > 0) {
                MessageBox.Show(
                    $"Der Lagerplatz ist nicht leer, bitte leeren Sie zuvor alle Bestände!",
                    "Warnung", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult messageBoxResult = MessageBox.Show(
                $"Sind Sie sich sicher, dass Sie den Lagerplatz \"{lagerplatz.lpz_bezeichnung}\" aus der Datenbank löschen wollen?",
                "Abfrage", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Warning);
            
            if(messageBoxResult != MessageBoxResult.Yes) {
                return;
            }

            try {
                Database.Remove(lagerplatz);
                Database.SaveChanges();

                Lagerplätze.Remove(lagerplatz);
            } catch(Exception) {
                MessageBox.Show(
                    "Der Lagerplatz konnte nicht gelöscht werden! Überprüfen Sie die Datenbankverbindung!",
                    "Fehlermeldung", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        protected void LagerplatzAnlegen() {
            if(string.IsNullOrWhiteSpace(LagerplatzName)) {
                MessageBox.Show("Der angegebene Lagerplatzname darf nicht leer sein!",
                    "Fehlermeldung", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try {
                if(Database.Lagerplatz.Any(x => x.lpz_bezeichnung.ToLower() == LagerplatzName.ToLower())) {
                    MessageBox.Show("Der angegebene Lagerplatzname ist bereits vergeben!",
                        "Fehlermeldung", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBoxResult messageResult = MessageBox.Show(
                    $"Sind Sie sicher das sie den Lagerplatz '{LagerplatzName}' anlegen wollen?",
                    "Information", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if(messageResult == MessageBoxResult.No) {
                    return;
                }

                Lagerplatz lagerplatz = new() {
                    fuellstand = 0,
                    lpz_bezeichnung = LagerplatzName,
                    Bestände = []
                };

                Database.Lagerplatz.Add(lagerplatz);
                Database.SaveChanges();

                Lagerplätze.Add(lagerplatz);
                Lagerplätze = Lagerplätze.OrderBy(x => x.lpz_bezeichnung).ToObservableCollection();
                

                MessageBox.Show("Lagerplatz wurde erfolgreich angelegt.",
                "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(Exception) {
                MessageBox.Show("Fehler beim Anlegen in der Datenbank!",
                    "Fehlermeldung", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    public static class IEnumerableExtensions {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerable) {
            return [.. enumerable];
        }
    }
}