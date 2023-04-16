using System.ComponentModel;

namespace nebrangu.Models
{
	public class Category
	{
		public int Id { get; set; }

		[DisplayName("Kategorija")]
		public string Name { get; set; }
	}
}
