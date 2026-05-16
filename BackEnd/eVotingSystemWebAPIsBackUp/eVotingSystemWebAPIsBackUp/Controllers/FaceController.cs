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

        // ================= REAL ID-BASED VERIFICATION =================
        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromForm] string idNo, IFormFile liveImage)
        {
            if (string.IsNullOrWhiteSpace(idNo) || liveImage == null)
                return BadRequest("ID number and image are required");

            // 1. Get latest registration for this voter
            var registration = await _context.VotingRegistrations
                .Where(r => r.IdNo == idNo)
                .OrderByDescending(r => r.RegisteredAt)
                .FirstOrDefaultAsync();

            if (registration == null)
                return NotFound("No registration found for this ID");

            if (string.IsNullOrWhiteSpace(registration.FaceImagePath))
                return BadRequest("Face image path is missing in database");

            // 2. Convert DB path -> real physical path
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

            // 3. Load stored image
            var storedFaceBytes = await System.IO.File.ReadAllBytesAsync(fullPath);

            // 4. Load live image
            using var ms = new MemoryStream();
            await liveImage.CopyToAsync(ms);
            var liveFaceBytes = ms.ToArray();

            // 5. Compare faces (AWS Rekognition)
            var similarity = await _faceService.CompareFaces(storedFaceBytes, liveFaceBytes);

            bool match = similarity.HasValue && similarity >= 80;

            return Ok(new
            {
                match,
                similarity = similarity ?? 0,
                idNo
            });
        }


    }
}