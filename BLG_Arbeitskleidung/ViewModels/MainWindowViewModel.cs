using BLG_Arbeitskleidung.Models;
using BLG_Arbeitskleidung.Views.UserControls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using Arbeitsbekleidung.Models.Models;
using Arbeitsbekleidung.Database.Database;

namespace BLG_Arbeitskleidung.ViewModels {
    partial class MainWindowViewModel : ObservableObject {

        [ObservableProperty]
        public FrameworkElement? _ContentView = null;

        [ObservableProperty]
        public ViewType _CurrentViewType;

        public BLGBestandDbContext Database { get; set; }

        public MainWindowViewModel() {
            Database = new BLGBestandDbContext();
            SwitchTab(ViewType.AusgabeView);

            Lagerplatz[] lagerplätze = Database.Lagerplatz
                .Include(x => x.Bestaende)
                    .ThenInclude(x => x.Arbeitskleidung)
                .ToArray();
        }

        [RelayCommand]
        protected void SwitchTab(ViewType viewtype) {
            switch(viewtype) {
            case ViewType.AusgabeView:
                ContentView = new AusgabeControl() {
                    DataContext = new AusgabeViewModel(Database)
                };
                break;
            case ViewType.EinlagernView:
                ContentView = new EinlagernControl() {
                    DataContext = new EinlagernViewModel(Database)
                };
                break;
            case ViewType.BestandAnzeigenView:
                ContentView = new BestandAnzeigenControl() {
                    DataContext = new BestandAnzeigenViewModel(Database)
                };
                break;
            case ViewType.LagerplatzHinzufügenView:
                ContentView = new LagerplatzAddControl() {
                    DataContext = new LagerplatzAddViewModel(Database)
                };
                break;
            case ViewType.ArbeitskleidungHinzufügen:
                ContentView = new ArbeitskleidungVerwaltenControl() {
                    DataContext = new ArbeitskleidungVerwaltenViewModel(Database)
                };
                break;
            case ViewType.Log:
                ContentView = new LogControl() {
                    DataContext = new LogViewModel(Database)
                };
                break;
            case ViewType.ArbeitskleidungVerwalten:
                ContentView = new ArtikelBearbeitenControl() {
                    DataContext = new ArtikelBearbeitenViewModel(Database)
                };
                break;
            default:
                break;
            }
            CurrentViewType = viewtype;
        }
    }
}