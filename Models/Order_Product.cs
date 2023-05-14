using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nebrangu.Models
{
    [Table("order_products")]
    public class Order_Product
    {
        public int? Id { get; set; }

        [Display(Name = "Prekė")]
        public Product Product { get; set; }

        public Order Order { get; set; }
    }
}
