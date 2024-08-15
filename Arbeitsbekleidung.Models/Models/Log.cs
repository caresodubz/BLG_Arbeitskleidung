using Microsoft.EntityFrameworkCore;

namespace Arbeitsbekleidung.Models.Models {
    [PrimaryKey(nameof(log_id))]
    public class Log {
        public int log_id {  get; set; } = 0;
        public DateTime Datum { get; set; } 
        public string? log_bezeichnung { get; set; } = null;
        public string? buchungsart {  get; set; } = null;
    }
}