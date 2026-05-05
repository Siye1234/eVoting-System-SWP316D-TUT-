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
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // CREATE ADMIN
        
        [HttpPost("create-admin")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminDTO dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Invalid request" });

            // CHECK IF ADMIN EXISTS (by StaffNumber)
            var exists = await _context.Admins
                .AnyAsync(x => x.StaffNumber == dto.StaffNumber);

            if (exists)
            {
                return Conflict(new
                {
                    message = "Admin with this Staff Number already exists"
                });
            }

            // HASH PASSWORD
            var passwordHasher = new PasswordHasher<Admin>();

            var admin = new Admin
            {
                StaffNumber = dto.StaffNumber,
                Name = dto.Name,
                Surname = dto.Surname,
                DateHired = DateTime.Now,
                Role = "Admin"
            };

            admin.PasswordHash = passwordHasher.HashPassword(admin, dto.Password);

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Admin created successfully",
                staffNumber = admin.StaffNumber
            });
        }

        //GET PENDING DOCUMENTS
        [HttpGet("registrations/pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingDocuments()
        {
            var data = await _context.VotingRegistrations
                .Where(x => x.ProofOfAddressPath != null && x.ProofOfAddressPath != "")
                .Where(x => x.IsApproved == false)
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult Count()
        {
            var count = _context.VotingRegistrations.Count(x => x.IsApproved == true);

            return Ok(new { count });
        }

        //add political party
        [HttpPost("create-party")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateParty([FromForm] CreatePartyDTO dto)
        {
            if (string.IsNullOrEmpty(dto.PartyName))
                return BadRequest("Party name is required");

            if (dto.Logo == null)
                return BadRequest("Logo is required");

            var exists = await _context.PoliticalParties
                .AnyAsync(x => x.Name == dto.PartyName);

            if (exists)
                return Conflict("Party already exists");

            var fileName = Guid.NewGuid() + Path.GetExtension(dto.Logo.FileName);
            var filePath = Path.Combine("uploads", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Logo.CopyToAsync(stream);
            }

            var party = new PoliticalParty
            {
                Name = dto.PartyName,
                LogoUrl = "/uploads/" + fileName,
                IsApproved = false,
                IsRejected = false,

                // 🔥 FORCE ALL BALLOTS
                ElectionType =
                 ElectionType.Regional |
                 ElectionType.Provincial |
                 ElectionType.National
            };

            _context.PoliticalParties.Add(party);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Party submitted for approval" });
        }
        [HttpDelete("delete-party/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteParty(int id)
        {
            var party = await _context.PoliticalParties.FindAsync(id);

            if (party == null)
                return NotFound("Party not found");

            _context.PoliticalParties.Remove(party);
            await _context.SaveChangesAsync();

            return Ok("Party deleted");
        }
    }
}