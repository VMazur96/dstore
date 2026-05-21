using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Drajbot.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        // 1. Nivo: JAVNO (Svako može da pregleda cene i katalog)
        [HttpGet]
        public IActionResult GetCatalog()
        {
            return Ok(new { poruka = "Katalog je otvoren: Prikazujem V-Bucks pakete i dostupne skinove..." });
        }

        // 2. Nivo: SAMO ADMIN (Zaključano za sve osim za ulogu Admin)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddProduct()
        {
            // Ovde će kasnije ići prava logika za upis u bazu
            return Ok(new { poruka = "Admin akcija: Novi item je uspešno dodat u sistem!" });
        }
    }
}