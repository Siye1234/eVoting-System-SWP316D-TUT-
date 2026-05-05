using eVotingSystemWebAPIsBackUp.Models;

namespace eVotingSystemWebAPIsBackUp.DTOs
{
    public class CreatePartyDTO
    {
        public string PartyName { get; set; }
        public IFormFile Logo { get; set; }
    }
}
