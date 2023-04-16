using System.ComponentModel;

namespace nebrangu.Models
{
	public class Manufacturer
	{
		public int Id { get; set; }

		[DisplayName("Gamintojas")]
		public string Name { get; set; }
	}
}
