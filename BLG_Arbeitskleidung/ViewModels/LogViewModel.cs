using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Arbeitsbekleidung.Models.Models;
using Arbeitsbekleidung.Database.Database;

namespace BLG_Arbeitskleidung.ViewModels {
    public partial class LogViewModel : ObservableObject {

        public BLGBestandDbContext Database { get; set; }

        public ObservableCollection<Log> Logs { get; } = [];

        public LogViewModel(BLGBestandDbContext database) {
            Database = database;
            foreach(Log logs in Database.Log) {
                Logs.Add(logs);
            }
        }        
    }
}
