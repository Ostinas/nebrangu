using System.ComponentModel.DataAnnotations;

namespace nebrangu.Models
{
    public class Manufacturer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(255, ErrorMessage = "Pavadinimas negali viršyti 255 raidžių.")]
        public string Name { get; set; }
    }
}
