using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nebrangu.Models
{
    [Table("problems")]
    public class Problem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(255, ErrorMessage = "Pavadinimas negali viršyti 255 raidžių.")]
        public string Name { get; set; }
    }
}
