using eVotingSystemWebAPIsBackUp.Data;
using eVotingSystemWebAPIsBackUp.DTOs;
using eVotingSystemWebAPIsBackUp.Models;
using eVotingSystemWebAPIsBackUp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace eVotingSystemWebAPIsBackUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VotingRegistrationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly FileStorageService _fileService;

        public VotingRegistrationController(ApplicationDbContext context, FileStorageService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        //  REGISTER 
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] VotingRegistrationFullDTO model)
        {
            var idNo = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(idNo))
                return Unauthorized();

            var exists = await _context.VotingRegistrations.AnyAsync(x => x.IdNo == idNo);

            if (exists)
                return BadRequest("Already registered.");

            string proofPath = null;
            string facePath = null;

            if (model.ProofFile != null)
                proofPath = await _fileService.SaveFile(model.ProofFile, "uploads");

            if (model.FaceImage != null)
                facePath = await _fileService.SaveFile(model.FaceImage, "faces");

            var electionType = (ElectionType)model.ElectionType;

            var registration = new VotingRegistration
            {
                IdNo = idNo,
                ElectionType = electionType,
                ResidentialAddress = model.ResidentialAddress,
                ProofOfAddressPath = proofPath,
                FaceImagePath = facePath,
                FacialScanScore = 0.8m,
                RegisteredAt = DateTime.UtcNow
            };

            // 🔥 APPROVAL RULE
            if (electionType == ElectionType.National)
            {
                registration.IsApproved = true;   // auto approve
            }
            else
            {
                registration.IsApproved = false;  // needs admin approval
            }

            _context.VotingRegistrations.Add(registration);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registration successful" });
        }

        //  MY REGISTRATIONS 
        [HttpGet("my-registrations")]
        public async Task<IActionResult> MyRegistrations()
        {
            var idNo = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var data = await _context.VotingRegistrations
                .Where(x => x.IdNo == idNo)
                .ToListAsync();

            return Ok(data);
        }

        //  VOTER COUNT 
        [HttpGet("voters/count")]
        public IActionResult Count()
        {
            var count = _context.VotingRegistrations.Count(x => x.IsApproved);

            return Ok(new { count });
        }
    }
}