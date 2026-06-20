using eVotingSystemWebAPIsBackUp.Data;
using eVotingSystemWebAPIsBackUp.DTOs;
using eVotingSystemWebAPIsBackUp.Models;
using eVotingSystemWebAPIsBackUp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;
namespace eVotingSystemWebAPIsBackUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Voter")]
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


        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> RegisterVoter(RegisterDTO reg)
        {
            if (string.IsNullOrEmpty(reg.IdNo) || reg.IdNo.Length != 13 || !reg.IdNo.All(char.IsDigit))
                return BadRequest("Invalid ID number.");

            var existingVoter = await _context.Voters
                .FirstOrDefaultAsync(p => p.IdNo == reg.IdNo);

            if (existingVoter != null)
                return BadRequest("Voter with this ID Number already exist.");

            if (!reg.Email.Contains("@"))
                return BadRequest("Invalid Email Address.");

            var existingEmail = await _context.Voters.FirstOrDefaultAsync(v => v.Email == reg.Email);

            if (existingEmail != null)
                return BadRequest("Email is already registered.");


            if (string.IsNullOrEmpty(reg.PhoneNumber) || !reg.PhoneNumber.All(char.IsDigit) || reg.PhoneNumber.Length < 10)
                return BadRequest("Invalid phone number.");

            if (string.IsNullOrEmpty(reg.PhoneNumber) || !Regex.IsMatch(reg.PhoneNumber,
                @"^(\+27|0)(60|61|62|63|64|65|66|67|68|69|71|72|73|74|76|78|79|81|82|83|84)[0-9]{7}$"))
            {
                return BadRequest("Please enter a valid South African cellphone number.");
            }

            var existingPhone = await _context.Voters.FirstOrDefaultAsync(v => v.PhoneNumber == reg.PhoneNumber);

            if (existingPhone != null)
                return BadRequest("Phone number is already registered.");

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
                
                Gender = person.Gender,
                Email = reg.Email,
                PhoneNumber = reg.PhoneNumber,
                Role = "Voter"
            };

            voter.PasswordHash = _passwordHasher.HashPassword(voter, reg.Password);

            _context.Voters.Add(voter);
            await _context.SaveChangesAsync();

            return Ok("Account successfully created.");
        }


        [HttpPut]
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

        
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            if (string.IsNullOrEmpty(login.IdNo) || string.IsNullOrEmpty(login.Password))
                return BadRequest("ID number and password required.");

            
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

            //VOTER LOGIN 
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

        [HttpGet("my-status")]
        public async Task<IActionResult> GetMyStatus()
        {
            var idNo = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(idNo))
                return Unauthorized();

            var data = await _context.VotingRegistrations
                .Where(x => x.IdNo == idNo)
                .Select(x => new
                {
                    isApproved = x.IsApproved,
                    electionType = x.ElectionType,
                    adminComment = x.AdminComment
                })
                .FirstOrDefaultAsync();

            if (data == null)
            {
                return Ok(new
                {
                    isApproved = false,
                    electionType = (ElectionType?)null
                });
            }

            return Ok(data);
        }

        //My voting history
        [HttpGet("my-history")]
        [Authorize(Roles = "Voter")]
        public async Task<IActionResult> GetMyHistory()
        {
            var voterId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var history = await _context.Votes
                .Where(v => v.VoterId == voterId) 
                .Select(v => new
                {
                    electionType = v.ElectionType,
                    dateVoted = v.VoteDate
                })
                .ToListAsync();

            return Ok(history);
        }

        //Personal info for UI Dashboard
        [HttpGet("me")]
        [Authorize(Roles = "Voter")]
        public async Task<IActionResult> GetMyProfile()
        {
            var idNo = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(idNo))
                return Unauthorized();

            var voter = await _context.Voters
                .FirstOrDefaultAsync(v => v.IdNo == idNo);

            if (voter == null)
                return NotFound();

            return Ok(new
            {
                firstName = voter.FirstName,
                middleName = voter.MiddleName,
                lastName = voter.LastName,
                idNo = voter.IdNo
            });
        }

       
    }
}