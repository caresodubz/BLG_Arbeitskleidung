using Microsoft.EntityFrameworkCore;

namespace Arbeitsbekleidung.Models.Models {
    [PrimaryKey(nameof(person_nr))]
    public class Mitarbeiter {
        public string person_nr { get; set; } = string.Empty;
        public string vorname {  get; set; } = string.Empty;
        public string nachname { get; set; } = string.Empty;
    }
}