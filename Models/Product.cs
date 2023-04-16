using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace nebrangu.Models
{
	public class Product
	{
		[DisplayName("Pavadinimas")]
		[MaxLength(255)]
		[Required]
		public string Name { get; set; }

		[DisplayName("Aprašymas")]
		[Required]
		public string Description { get; set; }

		[DisplayName("Kaina")]
		[Range(0.0001, double.MaxValue)]
		[Required]
		public double Price { get; set; }

		[DisplayName("Įvertinimas")]
		[Range(0.0001, double.MaxValue)]
		public double Rating { get; set; }
		
		public int RatingCount { get; set; }

		[DisplayName("Kategorija")]
		[Required]
		public int FkCategory { get; set; }

		[DisplayName("Oras")]
		[Required]
		public int FkWeather { get; set; }

		[DisplayName("Gamintojas")]
		[Required]
		public int FkManufacturer{ get; set; }

		[DisplayName("Sezonas")]
		[Required]
		public int FkSeason { get; set; }

		[DisplayName("Kilmės šalis")]
		[Required]
		public string OriginCountry { get; set; }
	}
}
