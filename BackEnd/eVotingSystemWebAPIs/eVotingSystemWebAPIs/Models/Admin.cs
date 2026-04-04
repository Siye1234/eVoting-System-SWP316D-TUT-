using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eVotingSystemWebAPIs.Models
{
    public class Admin
    {
        public int Id { get; set; }

        public string StaffNumber { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        [Column (TypeName ="date")]
        public DateTime DateHired { get; set; }

        public string PasswordHash { get; set; }
        [NotMapped]
        public string Role { get; set; }
    }
}
