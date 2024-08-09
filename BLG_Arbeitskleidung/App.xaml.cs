using BLG_Arbeitskleidung.ViewModels;
using BLG_Arbeitskleidung.Views;
using System.Windows;

namespace BLG_Arbeitskleidung {

    public partial class App : Application {
        private void OnStartUp(object sender, StartupEventArgs e) {

            try {
                MainWindowView mainWindowView = new MainWindowView();
                MainWindowViewModel viewModel = new MainWindowViewModel();
                mainWindowView.DataContext = viewModel;
                mainWindowView.ShowDialog();
            }
            catch(Exception) {
                MessageBox.Show(
                    "Fehler beim Verbinden mit der Datenbank!",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void OnExit(object sender, ExitEventArgs e) {

        }
    }
}