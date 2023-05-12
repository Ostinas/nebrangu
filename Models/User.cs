using System.ComponentModel.DataAnnotations;

namespace nebrangu.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Reikalingas vardas.")]
        [StringLength(255, ErrorMessage = "Vardas negali viršyti 255 simbolių.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Reikalinga pavardė.")]
        [StringLength(255, ErrorMessage = "Pavardė negali viršyti 255 simbolių.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Reikalingas el. paštas.")]
        [StringLength(255, ErrorMessage = "El. paštas negali viršyti 255 simbolių.")]
        [EmailAddress(ErrorMessage = "Neteisingas el. pašto adresas.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Reikalingas lygis.")]
        public int Level { get; set; } = 1;

        [Required(ErrorMessage = "Reikalinga patikimumo reikšmė.")]
        public double Trustability { get; set; } = 5;

        [Required(ErrorMessage = "Reikalinga įvertinimo reikšmė.")]
        public double Rating { get; set; } = 0;

        [Required(ErrorMessage = "Reikalingas telefono numeris.")]
        [StringLength(255, ErrorMessage = "Telefono numeris negali viršyti 255 simbolių.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Reikalingas adresas.")]
        [StringLength(255, ErrorMessage = "Adresas negali viršyti 255 simbolių.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Reikalingas miestas.")]
        [StringLength(255, ErrorMessage = "Miestas negali viršyti 255 simbolių.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Reikalingas pašto kodas.")]
        [StringLength(255, ErrorMessage = "Pašto kodas negali viršyti 255 simbolių.")]
        public string PostalCode { get; set; }

        [StringLength(255, ErrorMessage = "Įmonės kodas negali viršyti 255 simbolių.")]
        public string CompanyCode { get; set; }

        [StringLength(255, ErrorMessage = "Banko sąskaita negali viršyti 255 simbolių.")]
        public string BankAccount { get; set; }
    }
}
