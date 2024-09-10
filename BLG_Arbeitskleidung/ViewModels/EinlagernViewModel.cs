using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using Arbeitsbekleidung.Models.Models;
using Arbeitsbekleidung.Database.Database;

namespace BLG_Arbeitskleidung.ViewModels {
    public partial class EinlagernViewModel : ObservableObject {

        public BLGBestandDbContext Database { get; set; }

        public Arbeitskleidung[] Arbeitskleidungen { get; }

        public Lagerplatz[] Lagerplaetze { get; }

        [ObservableProperty]
        protected Lagerplatz _AusgewaehlterLagerplatz;

        [ObservableProperty]
        protected int _Menge = 0;

        public ObservableCollection<string> Groeßen { get; } = new();

        [ObservableProperty]
        protected string _AusgwaehlteGroesse = string.Empty;

        [ObservableProperty]
        protected bool _IsMengeLeer = false;

        [ObservableProperty]
        protected string _Bemerkung;  

        public DateTime Einlagern_datum { get; set; } = DateTime.Now;

        public string Bearbeiter { get; set; } = System.Security.Principal.WindowsIdentity.GetCurrent().Name;



        public string[] Artikelnamen {
            get {
                return Arbeitskleidungen.Select(x => x.artikel_name).Distinct().ToArray();
            }
        }

        private string _AusgewaehlteArtikelnamen = string.Empty;
        public string AusgewaehlteArtikelnamen {
            get {
                return _AusgewaehlteArtikelnamen;
            }

            set {
                _AusgewaehlteArtikelnamen = value;
                Groeßen.Clear();
                foreach(string größe in Arbeitskleidungen
                    .Where(x => x.artikel_name == _AusgewaehlteArtikelnamen)
                    .Select(x => x.groesse)
                    .Distinct()) {
                    Groeßen.Add(größe);
                }

                AusgwaehlteGroesse = Groeßen.FirstOrDefault() ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public EinlagernViewModel(BLGBestandDbContext database) {
            Database = database;
            Arbeitskleidungen = Database.Arbeitskleidung
                .Include(x => x.Bestände)
                    .ThenInclude(x => x.Lagerplatz)
                .ToArray();
            Lagerplaetze = Database.Lagerplatz.OrderBy(x => x.lpz_bezeichnung).ToArray();
            AusgewaehlterLagerplatz = Lagerplaetze.FirstOrDefault()!;
            AusgewaehlteArtikelnamen = Artikelnamen.FirstOrDefault()!;
        }

        [RelayCommand]
        public void Einlagern() {
            try {
                if(Menge <= 0) {
                    IsMengeLeer = true;
                    MessageBox.Show(
                        "Es muss eine Menge eingegeben werden!",
                        "Fehler",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                Arbeitskleidung selectedArbeitskleidung = Arbeitskleidungen
                .First(x => x.artikel_name == AusgewaehlteArtikelnamen && x.groesse == AusgwaehlteGroesse);

                MessageBoxResult messageBoxResult = MessageBox.Show(
                    $"Sind Sie sich sicher, dass Sie \"{selectedArbeitskleidung.artikel_name}\" ({selectedArbeitskleidung.artikel_nr}) einlagern wollen?",
                    "Abfrage",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if(messageBoxResult != MessageBoxResult.Yes) {
                    return;
                }

                Bestand? bestand = Database.Bestand.FirstOrDefault(x => x.artikel_id == selectedArbeitskleidung.artikel_id && x.lagerp_id == AusgewaehlterLagerplatz.lagerp_id);
                if(bestand != null) {
                    bestand.menge += Menge;
                }
                else {
                    bestand = new() {
                        Arbeitskleidung = selectedArbeitskleidung,
                        Lagerplatz = AusgewaehlterLagerplatz,
                        menge = Menge,
                    };
                    Database.Bestand.Add(bestand);
                }


                Log log = new() {
                    log_bemerkung = Bemerkung,
                    datum = DateTime.Now,
                    artikel = AusgewaehlteArtikelnamen,
                    log_groesse = AusgwaehlteGroesse,
                    log_lpz = AusgewaehlterLagerplatz.lpz_bezeichnung,
                    log_funktion = "Einlagerung",
                    log_stueckzahl = Menge,
                    bearbeiter = Bearbeiter
                };

                Database.Log.Add(log);
                Database.SaveChanges();

                MessageBox.Show(
                    "Bestand erfolgreich eingelagert!",
                    "Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch(Exception ex) {
                MessageBox.Show(
                    $"Fehler mit der Datenbankverbindung ({ex.Message})!",
                    "Fehlermeldung",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}