using System.ComponentModel.DataAnnotations;

namespace nebrangu.Models
{
    public class Attribute
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Pavadinimas yra privalomas")]
        [StringLength(255, ErrorMessage = "Pavadinimas negali viršyti 255 raidžių")]
        public string Name { get; set; }

        [Range(0.0, 1.0, ErrorMessage = "Nuotaikos koeficientas turi būti tarp 0.0 ir 1.0")]
        public double MoodCoefficient { get; set; }
    }
}
