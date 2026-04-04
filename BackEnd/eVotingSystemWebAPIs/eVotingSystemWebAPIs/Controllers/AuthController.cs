using eVotingSystemWebAPIs.Data;
using eVotingSystemWebAPIs.DTOs;
using eVotingSystemWebAPIs.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace eVotingSystemWebAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;
        private readonly PasswordHasher<string> _hasher;

        public AuthController(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
            _hasher = new PasswordHasher<string>();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            // 🔍 CHECK ADMIN
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.StaffNumber == login.IdNo);

            if (admin != null)
            {
                var result = _hasher.VerifyHashedPassword(login.IdNo, admin.PasswordHash, login.Password);

                if (result == PasswordVerificationResult.Success)
                {
                    return Ok(new
                    {
                        token = _jwtService.GenerateToken(admin.StaffNumber, "Admin"),
                        role = "Admin"
                    });
                }
            }

            // 🔍 CHECK VOTER
            var voter = await _context.Voters
                .FirstOrDefaultAsync(v => v.IdNo == login.IdNo);

            if (voter != null)
            {
                var result = _hasher.VerifyHashedPassword(login.IdNo, voter.PasswordHash, login.Password);

                if (result == PasswordVerificationResult.Success)
                {
                    return Ok(new
                    {
                        token = _jwtService.GenerateToken(voter.IdNo, "Voter"),
                        role = "Voter"
                    });
                }
            }

            return Unauthorized("Invalid credentials");
        }
    }
}