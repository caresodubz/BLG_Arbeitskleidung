using BLG_Arbeitskleidung.Database;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace BLG_Arbeitskleidung.ViewModels {
    public partial class ArbeitskleidungVerwaltenViewModel : ObservableObject {
        public BLGBestandDbContext Database { get; set; }

        [ObservableProperty]
        protected string _Artikel_Nr = string.Empty;

        [ObservableProperty]
        protected string _Artikel_Name = string.Empty;

        [ObservableProperty]
        protected string _Groesse = string.Empty;

        [ObservableProperty]
        protected bool _av_artikelnr = false;
        [ObservableProperty]
        protected bool _av_artikelname = false;

        [ObservableProperty]
        private bool _av_groesse = false;

       


    public Arbeitskleidung arbeitskleidung { get; set; }


    public ArbeitskleidungVerwaltenViewModel(BLGBestandDbContext database) {
        Database = database;
    }

    [RelayCommand]
    protected void ArtikelHinzufügen() {
        if(string.IsNullOrWhiteSpace(Artikel_Nr)) {
            Av_artikelnr = true;
        }
        else {
            Av_artikelnr = false;
        }

        if(string.IsNullOrWhiteSpace(Artikel_Name)) {
            Av_artikelname = true;
        }
        else {
            Av_artikelname = false;
        }

        if(string.IsNullOrWhiteSpace(Groesse)) {
            Av_groesse = true;
        }
        else {
            Av_groesse = false;
        }

        if(string.IsNullOrWhiteSpace(Artikel_Nr) || string.IsNullOrWhiteSpace(Artikel_Name) || string.IsNullOrWhiteSpace(Groesse)) {
            MessageBox.Show(
                "Einige Pflichtfelder wurden nicht korrekt ausgefüllt!",
                "Warnung",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }

        MessageBoxResult messageBoxResult = MessageBox.Show(
            $"Sind sie sicher, dass Sie diesen Artikel ({Artikel_Name}, {Artikel_Nr}) anlegen möchten?",
            "Information",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if(messageBoxResult != MessageBoxResult.Yes) {
            return;
        }

        try {
            if(Database.Arbeitskleidung.Any(x => x.artikel_nr.ToLower() == Artikel_Nr.ToLower())) {
                MessageBox.Show(
                    $"Die eingegebene Artikelnummer ist nicht eindeutig! Es gibt bereits einen Artikel mit dieser Artikelnummer ({Artikel_Nr})",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            Arbeitskleidung arbeitskleidung = new() {
                artikel_name = Artikel_Name,
                artikel_nr = Artikel_Nr,
                groesse = Groesse
            };

            Database.Arbeitskleidung.Add(arbeitskleidung);
            Database.SaveChanges();


            MessageBox.Show(
                "Artikel wurde erfolgreich angelegt!",
                "Information",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch(Exception) {
            MessageBox.Show(
                "Fehler beim Anlegen in der Datenbank!",
                "Fehlermeldung",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
}
