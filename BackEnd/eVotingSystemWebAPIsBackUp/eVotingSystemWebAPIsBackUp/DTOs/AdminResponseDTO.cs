namespace eVotingSystemWebAPIsBackUp.DTOs
{
    public class AdminResponseDTO
    {
        public int Id { get; set; }
        public string StaffNumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateHired { get; set; }
        public string Role { get; set; }
    }
}