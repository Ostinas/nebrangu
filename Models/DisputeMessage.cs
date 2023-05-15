namespace nebrangu.Models
{
    public class DisputeMessage
    {
        public int Id { get; set; }

        public int Description { get; set; }

        public Dispute Dispute { get; set; }

        public User User { get; set; }
    }
}
