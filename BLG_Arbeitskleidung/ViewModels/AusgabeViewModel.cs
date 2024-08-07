using BLG_Arbeitskleidung.Database;
using BLG_Arbeitskleidung.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace BLG_Arbeitskleidung.ViewModels {
    public partial class AusgabeViewModel : ObservableObject {

        public BLGBestandDbContext Database { get; set; }

        [ObservableProperty]
        protected ObservableCollection<Bestand> _Bestände = [];

        [ObservableProperty]
        protected ObservableCollection<Bestand> _SelectedBestände = [];

        [ObservableProperty]
        protected int _AddMenge = 0;

        [ObservableProperty]
        protected string _Vorname = string.Empty;

        [ObservableProperty]
        protected string _Nachname = string.Empty;

        [ObservableProperty]
        protected string _PersNummer = string.Empty;

        //für die Fehlerumrandung
        [ObservableProperty]
        protected bool _VornameTextBox = false;

        [ObservableProperty]
        protected bool _NachnameTextBox = false;

        [ObservableProperty]
        protected bool _PersNummerTextBox = false;  
        
        public AusgabeViewModel(BLGBestandDbContext database) {
            Database = database;
            foreach(Bestand bestand in Database.Bestand
                .Include(x => x.Arbeitskleidung)
                .Include(x => x.Lagerplatz)
                .Where(x => x.menge > 0 && x.Arbeitskleidung != null && x.Lagerplatz != null)
                .ToList()
                .OrderBy(x => x.Arbeitskleidung!.DisplayArtikelName)) {
                Bestände.Add(bestand);
            }
            database.ChangeTracker.Clear();
        }

        [RelayCommand]
        protected void AddItem(Bestand? bestand) {
            if(bestand == null) {
                return;
            }

            int differenz = bestand.menge < AddMenge ? bestand.menge : AddMenge;
            if(differenz == 0) {
                return;
            }

            bestand.menge -= differenz;

            Bestand? tempBestand = SelectedBestände.FirstOrDefault(x => x.bestand_id == bestand.bestand_id);
            if(tempBestand != null) {
                tempBestand.menge += differenz;
            } else {
                tempBestand = new() {
                    Arbeitskleidung = bestand.Arbeitskleidung,
                    Lagerplatz = bestand.Lagerplatz,
                    artikel_id = bestand.artikel_id,
                    bestand_id = bestand.bestand_id,
                    lagerp_id = bestand.lagerp_id,
                    person_nr = bestand.person_nr,
                    menge = differenz,
                };

                SelectedBestände.Add(tempBestand);
            }

            SelectedBestände = [.. SelectedBestände];
            Bestände = [.. Bestände];
        }

        [RelayCommand]
        protected void RemoveItem(Bestand? tempBestand) {
            if(tempBestand == null) {
                return;
            }

            int differenz = tempBestand.menge < AddMenge ? tempBestand.menge : AddMenge;
            if(differenz == 0) {
                return;
            }

            Bestand bestand = Bestände.First(x => x.bestand_id == tempBestand.bestand_id);
            bestand.menge += differenz;
            tempBestand.menge -= differenz;

            if(tempBestand.menge == 0) {
                SelectedBestände.Remove(tempBestand);
            }

            SelectedBestände = [.. SelectedBestände];
            Bestände = [.. Bestände];
        }

        [RelayCommand]
        protected void Drucken() {
            if(SelectedBestände.Count == 0) {
                MessageBox.Show("Es wurden keine Artikel ausgewählt!", 
                    "Fehlermeldung",
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                return;
            }


            //ifs damit die jeweiligen textboxen rot werden wenn sie nicht ausgefüllt wurden
            if(string.IsNullOrWhiteSpace(Vorname)) {
                VornameTextBox = true;
            }

            if(string.IsNullOrWhiteSpace(Nachname)) {
                NachnameTextBox = true;
            }

            if(string.IsNullOrWhiteSpace(PersNummer)) {
                PersNummerTextBox = true;
            }


            if(string.IsNullOrWhiteSpace(Vorname) || string.IsNullOrWhiteSpace(Nachname) || string.IsNullOrWhiteSpace(PersNummer)) {
                MessageBox.Show(
                    "Es wurden nicht alle Daten angegeben!", 
                    "Fehlermeldung", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                return;
            }        

            MessageBoxResult promptResult = MessageBox.Show(
                "Sind Sie sich sicher, ob die Angaben korrekt sind?", 
                "Abfrage", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question);

            if(promptResult != MessageBoxResult.Yes) {
                return;
            }

            string filePath = BLGWordAusgabeFactory.CreateAusgabeFile($"{Vorname} {Nachname}", PersNummer, SelectedBestände.ToArray());
            try {
                ProcessStartInfo startInfo = new(filePath) {
                    UseShellExecute = true,
                };

                Process.Start(startInfo);
            } catch(Exception) {
                MessageBoxResult messageBoxResult = MessageBox.Show(
                    "Datei konnte nicht geöffnet werden, wollen Sie sie speichern?",
                    "Fehlermeldung", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Error);

                if(messageBoxResult == MessageBoxResult.Yes) {
                    SaveFileDialog saveFileDialog = new() {
                        AddExtension = true,
                        DefaultExt = ".docx",
                        OverwritePrompt = true
                    };

                    if(saveFileDialog.ShowDialog() ?? false) {
                        try {
                            File.Copy(filePath, saveFileDialog.FileName, overwrite: true);
                        } catch(Exception) {
                            MessageBox.Show(
                                "Datei konnte nicht gespeichert werden!", 
                                "Fehlermeldung", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Error);
                            return;
                        }
                    }
                }
            }

            try {
                Bestand[] dbBestände = Database.Bestand.Include(x => x.Lagerplatz).Include(x => x.Arbeitskleidung).ToArray();
                Bestand[] veränderteBestände = dbBestände.Where(x => SelectedBestände.Any(y => y.bestand_id == x.bestand_id)).ToArray();

                foreach(Bestand bestand in veränderteBestände) {
                    bestand.menge -= SelectedBestände.First(x => x.bestand_id == bestand.bestand_id).menge;
                    if(bestand.menge <= 0) {
                        Database.Remove(bestand);
                    }
                }

                Database.SaveChanges();
                SelectedBestände.Clear();
                Bestände = [.. dbBestände.Where(x => x.menge > 0)];
            } catch(Exception) {
                MessageBox.Show(
                    "Bestandsänderung kann nicht gespeichert werden!", 
                    "Fehlermeldung", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                return;
            }
        }
    }
}