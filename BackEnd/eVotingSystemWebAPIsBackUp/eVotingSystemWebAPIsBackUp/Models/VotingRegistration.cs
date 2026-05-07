using System.ComponentModel.DataAnnotations;

namespace eVotingSystemWebAPIsBackUp.Models
{
    public class VotingRegistration
    {
        public int Id { get; set; }

        [Required]
        public string IdNo { get; set; } = null!;

        [Required]
        public ElectionType ElectionType { get; set; }
        public string? ResidentialAddress { get; set; } =null;  
        public string? ProofOfAddressPath { get; set; } = null;
        public string? FaceImagePath { get; set; }
        public decimal? FacialScanScore { get; set; }       
        public bool? AddressVerified { get; set; } = null;
        public string? AdminComment { get; set; }
        public bool IsApproved { get; set; } = true;
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    }
}