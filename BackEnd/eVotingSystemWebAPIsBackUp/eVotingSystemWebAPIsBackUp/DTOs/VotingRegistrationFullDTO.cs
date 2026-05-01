public class VotingRegistrationFullDTO
{
    public int ElectionType { get; set; }
    public string? ResidentialAddress { get; set; }

    public IFormFile? ProofFile { get; set; }

    public IFormFile FaceImage { get; set; } 
}