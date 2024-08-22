using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arbeitsbekleidung.Models.Models {
    [PrimaryKey(nameof(lagerp_id))]
    public class Lagerplatz {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int lagerp_id {  get; set; } = 0;
        public string lpz_bezeichnung { get; set; } = string.Empty;
        public int fuellstand { get; set; } = 0;

        public int Füllmenge {
            get { return Bestaende.Sum(x => x.menge); }
        }
        
        [ForeignKey(nameof(Bestand.lagerp_id))]
        public List<Bestand> Bestaende { get; set; } = new();
        
    }
}