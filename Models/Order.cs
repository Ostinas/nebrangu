using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nebrangu.Models
{
    [Table("orders")]
    public class Order
    {
        public int Id { get; set; }

        [Display(Name = "Data")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Suma")]
        public double Sum { get; set; }

        [Display(Name = "Klientas")]
        public User User { get; set; }

        [Display(Name = "Statusas")]
        public Order_Status Status { get; set; }

        [Display(Name = "Adresas")]
        public string? DeliveryAddress { get; set; }

        [Display(Name = "Miestas")]
        public string? DeliveryCity { get; set; }

        [Display(Name = "Pašto kodas")]
        public string? DeliveryPostalCode { get; set; }

        [Display(Name = "Pristatymo būdas")]
        public Delivery_Type? DeliveryType { get; set; }

        [Display(Name = "Apmokėjimo būdas")]
        public Payment_Method? PaymentMethod { get; set; }

        public bool isPaid { get; set; }

        public List<Order_Product> OrderProducts { get; set; }
    }
}
