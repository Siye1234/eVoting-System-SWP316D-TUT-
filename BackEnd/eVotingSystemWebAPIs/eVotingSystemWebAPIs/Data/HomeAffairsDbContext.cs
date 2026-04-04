using Microsoft.EntityFrameworkCore;

namespace eVotingSystemWebAPIs.Data
{
    public class HomeAffairsDbContext: DbContext
    {
        public HomeAffairsDbContext(DbContextOptions<HomeAffairsDbContext> options) : base(options)
        {
        }
         public DbSet<Models.Person> Persons { get; set; }
    }
}
