using eVotingSystemWebAPIs.Data;
using eVotingSystemWebAPIs.DTOs;
using eVotingSystemWebAPIs.Models;
using eVotingSystemWebAPIs.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eVotingSystemWebAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly HomeAffairsDbContext _homeAffairsDbContext;
        private readonly PasswordHasher<Voter> _passwordHasher;
        private readonly JwtService _jwtService;

        public VoterController(
            ApplicationDbContext context,
            HomeAffairsDbContext homeAffairsDbContext,
            JwtService jwtService)
        {
            _context = context;
            _homeAffairsDbContext = homeAffairsDbContext;
            _jwtService = jwtService;
            _passwordHasher = new PasswordHasher<Voter>();
        }

        // =========================
        // REGISTER (PUBLIC)
        // =========================
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> RegisterVoter(RegisterDTO reg)
        {
            if (string.IsNullOrEmpty(reg.IdNo) || reg.IdNo.Length != 13 || !reg.IdNo.All(char.IsDigit))
                return BadRequest("Invalid ID number.");

            if (string.IsNullOrEmpty(reg.Password) || reg.Password.Length < 6)
                return BadRequest("Password must be at least 6 characters.");

            if (reg.Password != reg.ConfirmPassword)
                return BadRequest("Passwords do not match.");

            var person = await _homeAffairsDbContext.Persons
                .FirstOrDefaultAsync(p => p.IdNo == reg.IdNo);

            if (person == null)
                return BadRequest("No person found in Home Affairs.");

            var voter = new Voter
            {
                IdNo = reg.IdNo,
                FirstName = person.FirstName,
                LastName = person.LastName,
                MiddleName = person.MiddleName,
                DOB = person.DOB,
                Gender = person.Gender,
                Email = reg.Email,
                Role = "Voter"
            };

            voter.PasswordHash = _passwordHasher.HashPassword(voter, reg.Password);

            _context.Voters.Add(voter);
            await _context.SaveChangesAsync();

            return Ok("Account successfully created.");
        }

        // =========================
        // UPDATE (ONLY LOGGED IN VOTER)
        // =========================
        [HttpPut]
        [Authorize(Roles = "Voter")]
        public async Task<IActionResult> UpdateVoter(UpdateVoterDTO reg)
        {
            var person = await _homeAffairsDbContext.Persons
                .FirstOrDefaultAsync(p => p.IdNo == reg.IdNo);

            if (person == null)
                return BadRequest("No record in Home Affairs.");

            var voter = await _context.Voters
                .FirstOrDefaultAsync(v => v.IdNo == reg.IdNo);

            if (voter == null)
                return NotFound("Voter not found.");

            voter.FirstName = person.FirstName;
            voter.LastName = person.LastName;
            voter.MiddleName = person.MiddleName;

            await _context.SaveChangesAsync();

            return Ok("Voter updated successfully.");
        }

        // =========================
        // LOGIN (PUBLIC)
        // =========================
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            if (string.IsNullOrEmpty(login.IdNo) || string.IsNullOrEmpty(login.Password))
                return BadRequest("ID number and password required.");

            // ================= ADMIN LOGIN =================
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.StaffNumber == login.IdNo);

            if (admin != null)
            {
                var adminResult = new PasswordHasher<Admin>()
                    .VerifyHashedPassword(admin, admin.PasswordHash, login.Password);

                if (adminResult == PasswordVerificationResult.Success)
                {
                    var token = _jwtService.GenerateToken(admin.StaffNumber, "Admin");

                    return Ok(new
                    {
                        message = "Admin login successful",
                        token,
                        role = "Admin"
                    });
                }
            }

            // ================= VOTER LOGIN =================
            var voter = await _context.Voters
                .FirstOrDefaultAsync(v => v.IdNo == login.IdNo);

            if (voter != null)
            {
                var voterResult = _passwordHasher.VerifyHashedPassword(
                    voter,
                    voter.PasswordHash,
                    login.Password);

                if (voterResult == PasswordVerificationResult.Success)
                {
                    var token = _jwtService.GenerateToken(voter.IdNo, "Voter");

                    return Ok(new
                    {
                        message = "Voter login successful",
                        token,
                        role = "Voter"
                    });
                }
            }

            return Unauthorized("Invalid credentials.");
        }
    }
}