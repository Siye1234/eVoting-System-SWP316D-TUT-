using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eVotingSystemWebAPIsBackUp.Models
{
    public class Voter
    {
        public int Id { get; set; }

        [Required]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "Voter ID must be exactly 13 characters long.")]
        public string IdNo { get; set; }
        [Required]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Name cannot be less than 2 characters.")]
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        [Required]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Name cannot be less than 2 characters.")]
        public string LastName { get; set; }
        [Required]
        public string Gender { get; set; }

       

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
