using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arbeitsbekleidung.Models.Models {
    [PrimaryKey(nameof(log_id))]
    public class Log {
        public int log_id { get; set; } = 0;        
        public DateTime datum { get; set; }

        public string log_date {
            get {
                return datum.ToString("dd.MM.yyyy");
            }
        }

        public string? log_bemerkung { get; set; }
        public string? artikel { get; set; }
        public string? log_groesse { get; set; }
        public string? log_lpz { get; set; }
        public string? log_funktion { get; set; }
        public int log_stueckzahl { get; set; }
        public string? bearbeiter { get; set; }       
        public string? person_nr { get; set; }        
    }
}