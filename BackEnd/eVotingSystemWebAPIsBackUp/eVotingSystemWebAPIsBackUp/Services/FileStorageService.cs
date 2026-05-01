namespace eVotingSystemWebAPIsBackUp.Services
{
    public class FileStorageService
    {
        public async Task<string> SaveFile(IFormFile file, string folder)
        {
            var pathFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder);

            if (!Directory.Exists(pathFolder))
                Directory.CreateDirectory(pathFolder);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(pathFolder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/{folder}/{fileName}";
        }
    }
}
