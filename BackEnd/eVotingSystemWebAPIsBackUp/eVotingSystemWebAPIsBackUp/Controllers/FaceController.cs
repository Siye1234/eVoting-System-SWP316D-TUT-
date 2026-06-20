using eVotingSystemWebAPIsBackUp.Data;
using eVotingSystemWebAPIsBackUp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace eVotingSystemWebAPIsBackUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaceController : ControllerBase
    {
        private readonly FaceRecognitionService _faceService;
        private readonly ApplicationDbContext _context;

        public FaceController(
            FaceRecognitionService faceService,
            ApplicationDbContext context)
        {
            _faceService = faceService;
            _context = context;
        }

        
        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromForm] string idNo, IFormFile liveImage)
        {
            if (string.IsNullOrWhiteSpace(idNo) || liveImage == null)
                return BadRequest("ID number and image are required");

            
            var registration = await _context.VotingRegistrations
                .Where(r => r.IdNo == idNo)
                .OrderByDescending(r => r.RegisteredAt)
                .FirstOrDefaultAsync();

            if (registration == null)
                return NotFound("No registration found for this ID");

            if (string.IsNullOrWhiteSpace(registration.FaceImagePath))
                return BadRequest("Face image path is missing in database");

           
            var rootPath = Directory.GetCurrentDirectory();
            var webRootPath = Path.Combine(rootPath, "wwwroot");

            var relativePath = registration.FaceImagePath
                .TrimStart('/')
                .Replace("/", Path.DirectorySeparatorChar.ToString());

            var fullPath = Path.Combine(webRootPath, relativePath);

            if (!System.IO.File.Exists(fullPath))
            {
                return BadRequest(new
                {
                    message = "Stored face image not found",
                    pathChecked = fullPath
                });
            }

            
            var storedFaceBytes = await System.IO.File.ReadAllBytesAsync(fullPath);

           
            using var ms = new MemoryStream();
            await liveImage.CopyToAsync(ms);
            var liveFaceBytes = ms.ToArray();

            
            var similarity = await _faceService.CompareFaces(storedFaceBytes, liveFaceBytes);

            bool match = similarity.HasValue && similarity >= 80;

            return Ok(new
            {
                match,
                similarity = similarity ?? 0,
                idNo
            });
        }

        [HttpPost("check-duplicate-face")]
        public async Task<IActionResult> CheckDuplicateFace(IFormFile liveImage)
        {
            if (liveImage == null)
                return BadRequest("Face image is required");

            using var ms = new MemoryStream();
            await liveImage.CopyToAsync(ms);
            var liveFaceBytes = ms.ToArray();

            var registrations = await _context.VotingRegistrations
                .Where(r => !string.IsNullOrEmpty(r.FaceImagePath))
                .ToListAsync();

            foreach (var registration in registrations)
            {
                var relativePath = registration.FaceImagePath
                    .TrimStart('/')
                    .Replace("/", Path.DirectorySeparatorChar.ToString());

                var fullPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    relativePath);

                if (!System.IO.File.Exists(fullPath))
                    continue;

                var storedFaceBytes =
                    await System.IO.File.ReadAllBytesAsync(fullPath);

                var similarity =
                    await _faceService.CompareFaces(
                        storedFaceBytes,
                        liveFaceBytes);

                if (similarity.HasValue && similarity >= 80)
                {
                    return Ok(new
                    {
                        duplicate = true,
                        similarity,
                        matchedVoter = registration.IdNo
                    });
                }
            }

            return Ok(new
            {
                duplicate = false
            });
        }

    }
}