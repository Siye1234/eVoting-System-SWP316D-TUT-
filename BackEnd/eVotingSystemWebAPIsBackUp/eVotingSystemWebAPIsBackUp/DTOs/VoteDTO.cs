using eVotingSystemWebAPIsBackUp.Models;

namespace eVotingSystemWebAPIsBackUp.DTOs
{
    public class VoteDTO
    {
        public int PartyId { get; set; }
        public ElectionType ElectionType { get; set; }
    }
}
