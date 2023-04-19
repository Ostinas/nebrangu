using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace nebrangu.Models
{
    public class Product
    {        
        public int Id { get; set; }

        [Required(ErrorMessage = "Pavadinimas yra privalomas")]
        [StringLength(255, ErrorMessage = "Pavadinimas negali viršyti 255 raidžių")]
        [Display(Name = "Pavadinimas")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Aprašymas yra privalomas")]
        [Display(Name = "Aprašymas")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Kaina yra privaloma")]
        [Range(0.01, 10000, ErrorMessage = "Kaina privalo būti tarp 0.01 ir 10000.00")]
        [Display(Name = "Kaina")]
        public decimal Price { get; set; }

        [Range(0.0, 5.0, ErrorMessage = "Įvertinimas privalo būti tarp 0.0 ir 5.0")]
        public double Rating { get; set; } = 0.0;

        public int RatingCount { get; set; } = 0;

        [Display(Name = "Kategorija")]
        public int CategoryId { get; set; }

        [Display(Name = "Gamintojas")]
        public int ManufacturerId { get; set; }

        [Display(Name = "Tinkamas oras")]
        public int? WeatherId { get; set; }

        [Display(Name = "Tinkamas sezonas")]
        public int? SeasonId { get; set; }

        [StringLength(255, ErrorMessage = "Šalies pavadinimas negali viršyti 255 raidžių")]
        [Display(Name = "Kilmies šalis")]
        public string OriginCountry { get; set; }

        [Display(Name = "Kategorija")]
        public virtual Category Category { get; set; }

        [Display(Name = "Gamintojas")]
        public virtual Manufacturer Manufacturer { get; set; }

        [Display(Name = "Tinkamas oras")]
        public virtual Weather Weather { get; set; }

        [Display(Name = "Tinkamas sezonas")]
        public virtual Season Season { get; set; }
    }
}
