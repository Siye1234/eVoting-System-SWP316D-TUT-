using eVotingSystemWebAPIsBackUp.Data;
using eVotingSystemWebAPIsBackUp.DTOs;
using eVotingSystemWebAPIsBackUp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace eVotingSystemWebAPIsBackUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Voter")]
    public class VotingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VotingController(ApplicationDbContext context)
        {
            _context = context;
        }


        // CHECK IF ELECTION IS OPEN        
        [HttpGet("status")]
        [AllowAnonymous]
        public async Task<IActionResult> GetElectionStatus()
        {
            var election = await _context.Elections.FirstOrDefaultAsync();

            if (election == null)
                return Ok(new { status = "ToBeAnnounced" });

            var now = DateTime.Now;

            bool isOpen = now >= election.StartDate && now <= election.EndDate;

            return Ok(new
            {
                status = isOpen ? "Active" : "Closed",
                startDate = election.StartDate,
                endDate = election.EndDate
            });
        }


        // GET BALLOT (PARTIES)        
        [HttpGet("ballot")]
        public async Task<IActionResult> GetBallot()
        {
            var election = await _context.Elections.FirstOrDefaultAsync();

            if (election == null || !election.IsActive)
                return BadRequest(new { message = "Election is not open" });

            var now = DateTime.Now;

            if (now < election.StartDate || now > election.EndDate)
                return BadRequest(new { message = "Voting is closed" });

            var parties = await _context.PoliticalParties
                .Where(p => p.IsApproved)
                .Select(p => new BallotDTO
                {
                    PartyId = p.Id,
                    PartyName = p.Name,
                    LogoUrl = p.LogoUrl
                })
                .ToListAsync();

            return Ok(parties);
        }

        [HttpPost("cast-vote")]
        [Authorize(Roles = "Voter")]
        public async Task<IActionResult> CastVote([FromBody] VoteDTO dto)
        {
            // 1. GET ACTIVE ELECTION (BASED ON DATES ONLY)
            var now = DateTime.UtcNow;

            var election = await _context.Elections.FirstOrDefaultAsync();

            if (election == null)
                return BadRequest(new { message = "No election configured" });

            if (now < election.StartDate || now > election.EndDate)
                return BadRequest(new { message = "Voting is closed" });

            // 2. GET VOTER ID
            var idNo = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(idNo))
                return Unauthorized();

            // 3. PREVENT DOUBLE VOTING PER BALLOT TYPE
            var alreadyVoted = await _context.Votes.AnyAsync(x =>
                x.VoterId == idNo &&
                x.ElectionType == dto.ElectionType);

            if (alreadyVoted)
                return BadRequest(new { message = "You already voted for this ballot" });

            // 4. VALIDATE PARTY EXISTS
            var party = await _context.PoliticalParties
                .FirstOrDefaultAsync(p => p.Id == dto.PartyId);

            if (party == null)
                return NotFound(new { message = "Party not found" });

            
            if (!party.ElectionType.HasFlag(dto.ElectionType))
            {
                return BadRequest(new { message = "This party is not available for this ballot" });
            }

            // 6. SAVE VOTE
            var vote = new Vote
            {
                PartyId = dto.PartyId,
                VoterId = idNo,
                ElectionType = dto.ElectionType,
                VoteDate = DateTime.UtcNow
            };

            _context.Votes.Add(vote);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Vote cast successfully" });
        }

        private async Task<object> GetWinner(ElectionType type)
        {
            var winner = await _context.Votes
                .Where(v => v.ElectionType == type)
                .GroupBy(v => v.PartyId)
                .Select(g => new
                {
                    PartyId = g.Key,
                    Votes = g.Count()
                })
                .OrderByDescending(x => x.Votes)
                .FirstOrDefaultAsync();

            if (winner == null)
            {
                return new
                {
                    partyName = "No votes",
                    votes = 0
                };
            }

            var party = await _context.PoliticalParties
                .FirstOrDefaultAsync(p => p.Id == winner.PartyId);

            return new
            {
                partyName = party?.Name ?? "Unknown",
                votes = winner.Votes
            };
        }

        // ===============================
        // GET RESULTS (ONLY IF PUBLISHED)
        // ===============================
        [HttpGet("election-results")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublishedResults()
        {
            var published = await _context.ElectionPublishStates.FirstOrDefaultAsync();

            if (published == null || published.IsPublished == false)
            {
                return Ok(new
                {
                    isPublished = false,
                    message = "Results not published yet"
                });
            }

            var result = new
            {
                isPublished = true,
                data = new
                {
                    national = await GetWinner(ElectionType.National),
                    provincial = await GetWinner(ElectionType.Provincial),
                    regional = await GetWinner(ElectionType.Regional)
                }
            };

            return Ok(result);
        }
    }
}