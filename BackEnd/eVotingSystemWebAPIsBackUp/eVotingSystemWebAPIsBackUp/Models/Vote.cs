using System;

namespace eVotingSystemWebAPIsBackUp.Models
{
    public class Vote
    {
        public int Id { get; set; }

        public string VoterId { get; set; }

        public int PartyId { get; set; }

        public ElectionType ElectionType { get; set; }

        public DateTime VoteDate { get; set; }
    }
}