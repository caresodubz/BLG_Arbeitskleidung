using Microsoft.EntityFrameworkCore;

namespace BLG_Arbeitskleidung.Database {
    [PrimaryKey("bestand_id")]
    public class Bestand {
        public int bestand_id {  get; set; } = 0;
        public int artikel_id { get; set; } = 0;
        public int? lagerp_id { get; set; } = null;
        public string? person_nr { get; set; } = null;
        public int menge { get; set; } = 0;

        public Arbeitskleidung? Arbeitskleidung { get; set; } = null;
        public Lagerplatz? Lagerplatz { get; set; } = null;
    }
}