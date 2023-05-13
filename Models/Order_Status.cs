using System.ComponentModel.DataAnnotations;

namespace nebrangu.Models
{
    public class Order_Status
    {
        public int Id { get; set; }

        [Display(Name = "Statusas")]
        public string Name { get; set; }
    }
}
