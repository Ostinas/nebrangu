using System.ComponentModel.DataAnnotations;

namespace nebrangu.Models
{
    public class Order_Product
    {
        public int Id { get; set; }

        [Display(Name = "Prekė")]
        public Product Product { get; set; }

        public Order Order { get; set; }
    }
}
