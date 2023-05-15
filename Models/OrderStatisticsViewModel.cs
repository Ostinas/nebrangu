using System.ComponentModel.DataAnnotations;

namespace nebrangu.Models
{
    public class OrderStatisticsViewModel
    {
        [Display(Name = "Atliktų užsakymų kiekis")]
        public int DoneOrderCount { get; set; }

        [Display(Name = "Neišspręstų ginčų kiekis")]
        public int LostDisputesCount { get; set; }

        [Display(Name = "Bendra užsakymų suma")]
        public double OrderSum { get; set; }

        [Display(Name = "Aktyvių nuolaidų kiekis")]
        public int ActiveDiscounts { get; set; }

        [Display(Name = "Neišsisiustų užsakymų sąrašas")]
        public List<Order> NotSentOrders { get; set; }
    }
}
