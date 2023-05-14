using System.ComponentModel.DataAnnotations;

namespace nebrangu.Models
{
    public class Delivery_Type
    {
        [Display(Name = "Pristatymo būdas")]
        public int Id { get; set; }

        [Display(Name = "Pristatymo būdas")]
        public string Name { get; set; }
    }
}
