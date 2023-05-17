using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nebrangu.Models
{
    [Table("disputes")]
    public class Dispute
    {
        public int Id { get; set; }
        [Display(Name = "Problema")]
        public virtual Problem Problem { get; set; }
        [Display(Name = "Sprendimas")]
        public virtual Solution Solution { get; set; }

        public int SolutionId { get; set; }

        [Display(Name = "Sprendimo balas")]
        public double? SolutionScore { get; set; }

        [Display(Name = "Pirkėjas")]
        public User Buyer { get; set; }

        [Display(Name = "Pardavejas")]
        public User Seller { get; set; }

        public int OrderId { get; set; }

        public List<Order> Orders { get; set; }
    }
}
