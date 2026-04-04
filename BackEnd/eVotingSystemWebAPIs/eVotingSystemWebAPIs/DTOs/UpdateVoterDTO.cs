using System.ComponentModel.DataAnnotations;

namespace eVotingSystemWebAPIs.DTOs
{
    public class UpdateVoterDTO
    {
        [Required]
        public string IdNo { get; set; }
    }
}
