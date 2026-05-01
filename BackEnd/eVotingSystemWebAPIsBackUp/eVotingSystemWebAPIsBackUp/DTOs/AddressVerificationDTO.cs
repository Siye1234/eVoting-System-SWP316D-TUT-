namespace eVotingSystemWebAPIsBackUp.DTOs
{
    public class AddressVerificationDTO
    {
        public int Id { get; set; }

        public string IdNo { get; set; }

        public string ResidentialAddress { get; set; }

        public string? ProofOfAddressPath { get; set; }

        public bool? AddressVerified { get; set; }
    }
}