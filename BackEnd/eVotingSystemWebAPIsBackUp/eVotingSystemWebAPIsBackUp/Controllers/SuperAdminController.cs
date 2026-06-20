using eVotingSystemWebAPIsBackUp.Data;
using eVotingSystemWebAPIsBackUp.DTOs;
using eVotingSystemWebAPIsBackUp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eVotingSystemWebAPIsBackUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class SuperAdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SuperAdminController(ApplicationDbContext context)
        {
            _context = context;
        }


        // CREATE SUPER ADMIN
        [HttpPost("create-super-admin")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateSuperAdmin([FromBody] CreateSuperAdminDTO dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Invalid request" });

            // Check if SuperAdmin already exists
            var exists = await _context.SuperAdmins.AnyAsync(x => x.StaffNumber == dto.StaffNumber);

            if (exists)
            {
                return Conflict(new
                {
                    message = "SuperAdmin already exists"
                });
            }

            var passwordHasher = new PasswordHasher<SuperAdmin>();

            var superAdmin = new SuperAdmin
            {
                StaffNumber = dto.StaffNumber,
                Name = dto.Name,
                Surname = dto.Surname,
                DateHired = DateTime.Now,
                Role = "SuperAdmin"
            };

            superAdmin.PasswordHash = passwordHasher.HashPassword(superAdmin, dto.Password);

            _context.SuperAdmins.Add(superAdmin);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "SuperAdmin created successfully"
            });
        }

            // APPROVE PARTY

        [HttpPut("approve-party/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ApproveParty(int id)
        {
            var party = await _context.PoliticalParties.FindAsync(id);

            if (party == null)
                return NotFound(new { message = "Political party not found." });

            party.IsApproved = true;
            party.IsRejected = false;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Political party approved successfully." });
        }

        
        //REJECT PARTY
        
        [HttpPut("reject-party/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> RejectParty(int id)
        {
            var party = await _context.PoliticalParties.FindAsync(id);

            if (party == null)
                return NotFound(new { message = "Political party not found." });

            party.IsApproved = false;
            party.IsRejected = true;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Political party rejected." });
        }

        
        // SET ELECTION DATES
        
        [HttpPost("set-election-dates")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> SetElectionDates(DateTime startDate, DateTime endDate)
        {
            if (endDate <= startDate)
                return BadRequest(new { message = "End date must be after start date." });

            var election = await _context.Elections.FirstOrDefaultAsync();

            if (election == null)
            {
                election = new Election
                {
                    StartDate = startDate,
                    EndDate = endDate
                };

                _context.Elections.Add(election);
            }
            else
            {
                election.StartDate = startDate;
                election.EndDate = endDate;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Election dates set successfully.",
                startDate,
                endDate
            });
        }

        [HttpGet("get-parties")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> GetParties()
        {
            var parties = await _context.PoliticalParties
                .Select(p => new
                {
                    id = p.Id,
                    partyName = p.Name,
                    isApproved = p.IsApproved,
                    isRejected = p.IsRejected,
                    logoUrl = p.LogoUrl
                })
                .ToListAsync();

            return Ok(parties);
        }

        [HttpGet("ballot-parties")]
        [Authorize(Roles = "Voter,SuperAdmin,Admin")]
        public async Task<IActionResult> GetBallotParties()
        {
            var national = await _context.PoliticalParties
    .Where(p => p.IsApproved && !p.IsRejected && p.ElectionType.HasFlag(ElectionType.National))
    .Select(p => new
    {
        id = p.Id,
        partyName = p.Name,
        logoUrl = p.LogoUrl
    })
    .ToListAsync();

            var provincial = await _context.PoliticalParties
                .Where(p => p.IsApproved && !p.IsRejected && p.ElectionType.HasFlag(ElectionType.Provincial))
                .Select(p => new
                {
                    id = p.Id,
                    partyName = p.Name,
                    logoUrl = p.LogoUrl
                })
                .ToListAsync();

            return Ok(new
            {
                national,
                provincial
            });
        }


        [HttpGet("election-results")]
        public async Task<IActionResult> GetElectionResults()
        {
            var result = new ElectionResultsResponseDto
            {
                National = await GetWinnerByType(ElectionType.National),
                Provincial = await GetWinnerByType(ElectionType.Provincial)
            };

            return Ok(result);
        }

        
        private async Task<ElectionResultDto> GetWinnerByType(ElectionType electionType)
        {
            var winner = await _context.Votes
                .Where(v => v.ElectionType == electionType)
                .GroupBy(v => v.PartyId)
                .Select(g => new
                {
                    PartyId = g.Key,
                    TotalVotes = g.Count()
                })
                .OrderByDescending(x => x.TotalVotes)
                .FirstOrDefaultAsync();

            if (winner == null)
            {
                return new ElectionResultDto
                {
                    PartyName = "No votes yet",
                    Votes = 0
                };
            }

            var party = await _context.PoliticalParties
                .FirstOrDefaultAsync(p => p.Id == winner.PartyId);

            return new ElectionResultDto
            {
                PartyName = party?.Name ?? "Unknown",
                Votes = winner.TotalVotes
            };
        }

        [HttpPost("publish-results")]
        public async Task<IActionResult> PublishResults()
        {
            var existing = await _context.ElectionPublishStates.FirstOrDefaultAsync();

            if (existing == null)
            {
                existing = new ElectionPublishState
                {
                    IsPublished = true,
                    PublishedAt = DateTime.UtcNow
                };

                _context.ElectionPublishStates.Add(existing);
            }
            else
            {
                existing.IsPublished = true;
                existing.PublishedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Results published successfully" });
        }

        [HttpGet("get-election-dates")]
        [AllowAnonymous] 
        public async Task<IActionResult> GetElectionDates()
        {
            var election = await _context.Elections.FirstOrDefaultAsync();

            if (election == null)
                return NotFound(new { message = "Election dates not set yet." });

            return Ok(new
            {
                startDate = election.StartDate,
                endDate = election.EndDate
            });
        }
    }
}