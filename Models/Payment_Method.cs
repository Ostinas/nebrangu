using System.ComponentModel.DataAnnotations;

namespace nebrangu.Models
{
    public class Payment_Method
    {
        public int Id { get; set; }

        [Display(Name = "Apmokėjimo būdas")]
        public string Name { get; set; }
    }
}
