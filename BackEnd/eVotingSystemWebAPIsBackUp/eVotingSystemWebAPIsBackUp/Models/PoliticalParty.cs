namespace eVotingSystemWebAPIsBackUp.Models
{
    public class PoliticalParty
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public bool IsApproved { get; set; } = false;
        public bool IsRejected { get; set; } = false;
        public ElectionType ElectionType { get; set; }
    }
}
