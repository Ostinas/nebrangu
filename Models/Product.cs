using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace nebrangu.Models
{
    public class Product
    {        
        public int Id { get; set; }

        [Required(ErrorMessage = "Pavadinimas yra privalomas")]
        [StringLength(255, ErrorMessage = "Pavadinimas negali viršyti 255 raidžių")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Aprašymas yra privalomas")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Kaina yra privaloma")]
        [Range(0.01, 10000, ErrorMessage = "Kaina privalo būti tarp 0.01 ir 10000.00")]
        public decimal Price { get; set; }

        [Range(0.0, 5.0, ErrorMessage = "Įvertinimas privalo būti tarp 0.0 ir 5.0")]
        public double Rating { get; set; }

        public int RatingCount { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Manufacturer")]
        public int ManufacturerId { get; set; }

        [Display(Name = "Weather")]
        public int? WeatherId { get; set; }

        [Display(Name = "Season")]
        public int? SeasonId { get; set; }

        [StringLength(255, ErrorMessage = "Šalies pavadinimas negali viršyti 255 raidžių")]
        public string OriginCountry { get; set; }

        public virtual Category Category { get; set; }

        public virtual Manufacturer Manufacturer { get; set; }

        public virtual Weather Weather { get; set; }

        public virtual Season Season { get; set; }
    }
}
