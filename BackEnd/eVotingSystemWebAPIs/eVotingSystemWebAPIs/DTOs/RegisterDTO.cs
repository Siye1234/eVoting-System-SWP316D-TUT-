using System.ComponentModel.DataAnnotations;

namespace eVotingSystemWebAPIs.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string IdNo { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
