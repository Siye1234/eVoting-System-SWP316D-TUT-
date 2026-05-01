using System.ComponentModel.DataAnnotations;

namespace eVotingSystemWebAPIsBackUp.DTOs
{
    public class UpdateVoterDTO
    {
        [Required]
        public string IdNo { get; set; }
    }
}
