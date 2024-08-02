using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLG_Arbeitskleidung.Database {
    [PrimaryKey(nameof(artikel_id))]
    public class Arbeitskleidung {
        public int artikel_id { get; set; } = 0;
        public string artikel_nr { get; set; } = string.Empty;
        public string artikel_name { get; set; } = string.Empty;
        public string kleidungsart {  get; set; } = string.Empty;
        public string groesse {  get; set; } = string.Empty;

        public string DisplayArtikelName {
            get {
                return $"{artikel_name} ({artikel_nr})";
            }
        }

        public string DisplayArtikelNameSize {
            get {
                return $"{artikel_name} ({artikel_nr}) - {groesse}";
            }
        }

        public int GesamtBestand {
            get {
                return Bestände.Sum(x => x.menge);
            }
        }

        [ForeignKey(nameof(Bestand.artikel_id))]
        public List<Bestand> Bestände { get; set; } = new();
    }
}