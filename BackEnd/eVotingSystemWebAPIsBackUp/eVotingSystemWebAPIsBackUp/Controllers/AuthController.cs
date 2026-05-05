using eVotingSystemWebAPIsBackUp.Data;
using eVotingSystemWebAPIsBackUp.DTOs;
using eVotingSystemWebAPIsBackUp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eVotingSystemWebAPIsBackUp.Controllers
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
            var super_admin = await _context.SuperAdmins
                .FirstOrDefaultAsync(a => a.StaffNumber == login.IdNo);

            if (super_admin != null)
            {
                var result = _hasher.VerifyHashedPassword(login.IdNo, super_admin.PasswordHash, login.Password);

                if (result == PasswordVerificationResult.Success)
                {
                    return Ok(new
                    {
                        token = _jwtService.GenerateToken(super_admin.StaffNumber, "SuperAdmin"),
                        role = "SuperAdmin",
                        idNo = super_admin.StaffNumber
                    });
                }
            }

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
                        role = "Admin",
                        idNo = admin.StaffNumber
                    });
                }
            }

            
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
                        role = "Voter",
                        idNo = voter.IdNo   
                    });
                }
            }

            return Unauthorized("Invalid credentials");
        }
    }
}