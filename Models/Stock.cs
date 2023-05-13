using System.ComponentModel.DataAnnotations;

namespace nebrangu.Models
{
    public class Stock
    {
        public int Id { get; set; }

        public int Count { get; set; }
        
        public Product Product { get; set; }

        public User User { get; set; }
    }
}
