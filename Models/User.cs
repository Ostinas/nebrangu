using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace nebrangu.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Reikalingas vardas.")]
        [StringLength(255, ErrorMessage = "Vardas negali viršyti 255 simbolių.")]
        [DisplayName("Vardas")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Reikalinga pavardė.")]
        [StringLength(255, ErrorMessage = "Pavardė negali viršyti 255 simbolių.")]
        [DisplayName("Pavardė")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Reikalingas el. paštas.")]
        [StringLength(255, ErrorMessage = "El. paštas negali viršyti 255 simbolių.")]
        [EmailAddress(ErrorMessage = "Neteisingas el. pašto adresas.")]
        [DisplayName("El. paštas")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Reikalingas lygis.")]
        [DisplayName("Lygis")]
        public int Level { get; set; } = 1;

        [Required(ErrorMessage = "Reikalinga patikimumo reikšmė.")]
        [DisplayName("Patikimumas")]
        public double Trustability { get; set; } = 5;

        [Required(ErrorMessage = "Reikalinga įvertinimo reikšmė.")]
        [DisplayName("Įvertinimas")]
        public double Rating { get; set; } = 0;

        [Required(ErrorMessage = "Reikalingas telefono numeris.")]
        [StringLength(255, ErrorMessage = "Telefono numeris negali viršyti 255 simbolių.")]
        [DisplayName("Telefono numeris")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Reikalingas adresas.")]
        [StringLength(255, ErrorMessage = "Adresas negali viršyti 255 simbolių.")]
        [DisplayName("Adresas")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Reikalingas miestas.")]
        [StringLength(255, ErrorMessage = "Miestas negali viršyti 255 simbolių.")]
        [DisplayName("Miestas")]
        public string City { get; set; }

        [Required(ErrorMessage = "Reikalingas pašto kodas.")]
        [StringLength(255, ErrorMessage = "Pašto kodas negali viršyti 255 simbolių.")]
        [DisplayName("Pašto kodas")]
        public string PostalCode { get; set; }

        [StringLength(255, MinimumLength = 0, ErrorMessage = "Įmonės kodas negali viršyti 255 simbolių.")]
        [DisplayName("Įmonės kodas")]
        public string? CompanyCode { get; set; }

        [StringLength(255, MinimumLength = 0, ErrorMessage = "Banko sąskaita negali viršyti 255 simbolių.")]
        [DisplayName("Banko sąskaita")]
        public string? BankAccount { get; set; }
    }
}
