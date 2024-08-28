using Microsoft.EntityFrameworkCore;

namespace Arbeitsbekleidung.Models.Models {
    [PrimaryKey(nameof(log_id))]
    public class Log {
        public int log_id { get; set; } = 0;
        public DateTime datum { get; set; } 
         public string log_date {
            get {
                return datum.ToString("dd.MM.yyyy HH:mm");
            }           
         } 
        public string? log_bemerkung { get; set; }
    }
}