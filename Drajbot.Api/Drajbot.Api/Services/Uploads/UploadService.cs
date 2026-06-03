using Drajbot.Api.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Drajbot.Api.Services.Uploads
{
    public class UploadService : IUploadService
    {
        public async Task<string> UploadImageAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                return "Greška: Fajl nije pronađen.";

            // 1. BEZBEDNOST: Limit na 2 MB (2 * 1024 * 1024 bajtova)
            if (file.Length > 2 * 1024 * 1024)
                return "Greška: Fajl je preveliki. Maksimalna veličina je 2 MB.";

            // 2. BEZBEDNOST: Provera MIME tipa (Dozvoljavamo samo JPG, PNG i WEBP)
            var dozvoljeniTipovi = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!dozvoljeniTipovi.Contains(file.ContentType))
                return "Greška: Nedozvoljen format. Dozvoljeni su samo JPG, PNG i WEBP.";

            // Generišemo jedinstveno ime fajla da ne bi došlo do prepisivanja (npr. slika.jpg -> 8f9d-slika.jpg)
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;

            // Putanja do "wwwroot/images/folderName"
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", folderName);

            // Ako folder ne postoji, napravi ga
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, uniqueFileName);

            // Fizičko čuvanje fajla na hard disk servera
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Vraćamo relativni link do slike koji ćemo upisati u bazu
            return $"/images/{folderName}/{uniqueFileName}";
        }
    }
}