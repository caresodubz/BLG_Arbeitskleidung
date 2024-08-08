using BLG_Arbeitskleidung.Database;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace BLG_Arbeitskleidung.ViewModels {
    public partial class LogViewModel : ObservableObject {

        public BLGBestandDbContext Database { get; set; }

        public ObservableCollection<Log> Logs {  get; set; }

        public LogViewModel(BLGBestandDbContext database) {
             Database = database;
        }
    }
}
