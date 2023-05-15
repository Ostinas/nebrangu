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
        public double SolutionScore { get; set; }
        public User Buyer { get; set; }
        public User Seller { get; set; }
    }
}
