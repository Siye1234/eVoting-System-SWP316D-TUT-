using eVotingSystemWebAPIs.Data;
using eVotingSystemWebAPIs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eVotingSystemWebAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<Admin> _hasher;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
            _hasher = new PasswordHasher<Admin>();
        }

        // 🔐 CREATE ADMIN (NO AUTH REQUIRED OR USE SUPER ADMIN LATER)
        [HttpPost("CreateAdmin")]
        public async Task<IActionResult> CreateAdmin(Admin admin)
        {
            if (admin == null)
                return BadRequest("Invalid admin data.");

            var exists = _context.Admins
                .Any(a => a.StaffNumber == admin.StaffNumber);

            if (exists)
                return BadRequest("Staff Number already exists.");

            // ✔ FIX: assume admin.Password is plain password
            admin.PasswordHash = _hasher.HashPassword(admin, admin.PasswordHash);

            admin.Role = "Admin";

            if (admin.DateHired == default)
                admin.DateHired = DateTime.Now;

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Admin created successfully"
            });
        }
    }
}