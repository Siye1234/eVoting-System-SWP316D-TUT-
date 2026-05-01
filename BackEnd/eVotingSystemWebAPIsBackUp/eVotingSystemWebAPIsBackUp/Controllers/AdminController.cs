using eVotingSystemWebAPIsBackUp.Data;
using eVotingSystemWebAPIsBackUp.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eVotingSystemWebAPIsBackUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET PENDING DOCUMENTS
        [HttpGet("registrations/pending")]
        public async Task<IActionResult> GetPendingDocuments()
        {
            var data = await _context.VotingRegistrations
                .Where(x => x.ProofOfAddressPath != null && x.ProofOfAddressPath != "")
                .Where(x => x.IsApproved == null)
                .Select(x => new
                {
                    x.Id,
                    x.IdNo,
                    x.ResidentialAddress,
                    x.ProofOfAddressPath,
                    x.FaceImagePath,
                    x.IsApproved
                })
                .ToListAsync();

            return Ok(data);
        }

        //APPROVE 
        [HttpPost("approve")]
        public async Task<IActionResult> Approve([FromBody] ApproveRequestDTO dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.IdNo))
                return BadRequest("Invalid request");

            var reg = await _context.VotingRegistrations
                .FirstOrDefaultAsync(x => x.IdNo == dto.IdNo);

            if (reg == null)
                return NotFound("Registration not found");

            reg.IsApproved = true;

            // optional comment handling
            var comment = dto.Comment ?? "";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Approved successfully",
                commentUsed = comment
            });
        }

        //REJECT 
        [HttpPost("reject")]
        public async Task<IActionResult> Reject([FromBody] RejectRequestDTO dto)
        {
            var reg = await _context.VotingRegistrations
                .FirstOrDefaultAsync(x => x.IdNo == dto.IdNo);

            if (reg == null)
                return NotFound();

            reg.IsApproved = false;

            
            reg.AdminComment = dto.Comment;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Rejected" });
        }

        //VOTER COUNT
        [HttpGet("voters/count")]
        public IActionResult Count()
        {
            var count = _context.VotingRegistrations.Count(x => x.IsApproved == true);

            return Ok(new { count });
        }
    }
}