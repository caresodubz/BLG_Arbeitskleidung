using BLG_Arbeitskleidung.Database;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;

namespace BLG_Arbeitskleidung.ViewModels {
    public partial class EinlagernViewModel : ObservableObject {
        
        public BLGBestandDbContext Database {  get; set; }
        public Arbeitskleidung[] Arbeitskleidungen { get; }
        
        public Lagerplatz[] Lagerplätze { get; }

        [ObservableProperty]
        protected Lagerplatz _SelectedLagerplatz;

        [ObservableProperty]
        protected int _Menge = 0;

        public ObservableCollection<string> Größen { get; } = new();
        
        [ObservableProperty]
        protected string _SelectedGröße = string.Empty;

        public string[] Artikelnamen {
            get {
                return Arbeitskleidungen.Select(x => x.artikel_name).Distinct().ToArray();
            }
        }

        private string _SelectedArtikelnamen = string.Empty;
        public string SelectedArtikelnamen {
            get {
                return _SelectedArtikelnamen;
            }

            set {
                _SelectedArtikelnamen = value;
                Größen.Clear();
                foreach(string größe in Arbeitskleidungen
                    .Where(x => x.artikel_name == _SelectedArtikelnamen)
                    .Select(x => x.groesse)
                    .Distinct()) {
                    Größen.Add(größe);
                }

                SelectedGröße = Größen.FirstOrDefault() ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public EinlagernViewModel(BLGBestandDbContext database) {
            Database = database;
            Arbeitskleidungen = Database.Arbeitskleidung
                .Include(x => x.Bestände)
                    .ThenInclude(x => x.Lagerplatz)
                .ToArray();
            Lagerplätze = Database.Lagerplatz.OrderBy(x => x.lpz_bezeichnung).ToArray();

            SelectedLagerplatz = Lagerplätze.First();
            SelectedArtikelnamen = Artikelnamen.First();
        }

        [RelayCommand]
        public void Einlagern() {
            try {
                if(Menge <= 0) {
                    MessageBox.Show("Es muss eine Menge eingegeben werden!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Arbeitskleidung selectedArbeitskleidung = Arbeitskleidungen
                .First(x => x.artikel_name == SelectedArtikelnamen && x.groesse == SelectedGröße);

                MessageBoxResult messageBoxResult = MessageBox.Show(
                    $"Sind Sie sich sicher, dass Sie \"{selectedArbeitskleidung.artikel_name}\" ({selectedArbeitskleidung.artikel_nr}) einlagern wollen?",
                    "Abfrage", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(messageBoxResult != MessageBoxResult.Yes) {
                    return;
                }

                Bestand? bestand = Database.Bestand.FirstOrDefault(x => x.artikel_id == selectedArbeitskleidung.artikel_id && x.lagerp_id == SelectedLagerplatz.lagerp_id);
                if(bestand != null) {
                    bestand.menge += Menge;
                } else {
                    bestand = new() {
                        Arbeitskleidung = selectedArbeitskleidung,
                        Lagerplatz = SelectedLagerplatz,
                        menge = Menge,
                    };

                    Database.Bestand.Add(bestand);
                }

                Database.SaveChanges();

                MessageBox.Show("Bestand erfolgreich eingelagert!", "Information", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            } catch(Exception) {
                MessageBox.Show("Fehler mit der Datenbankverbindung!", "Fehlermeldung", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}