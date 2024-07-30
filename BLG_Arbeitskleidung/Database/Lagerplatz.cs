using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLG_Arbeitskleidung.Database {
    [PrimaryKey("lagerp_id")]
    public class Lagerplatz {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int lagerp_id {  get; set; } = 0;
        public string lpz_bezeichnung { get; set; } = string.Empty;
        public int fuellstand { get; set; } = 0;
        
        [ForeignKey(nameof(Bestand.lagerp_id))]
        public List<Bestand> Bestände { get; set; } = new();
        
    }
}