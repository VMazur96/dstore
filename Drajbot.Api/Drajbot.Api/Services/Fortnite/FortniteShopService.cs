using System.Text.Json;
using Drajbot.Api.DTOs.Fortnite;
using Drajbot.Api.Interfaces;

namespace Drajbot.Api.Services.Fortnite
{
    public class FortniteShopService(HttpClient httpClient) : IFortniteShopService
    {
        // Tvoja tačna matematika mapirana u Rečnik (Dictionary)
        private static readonly Dictionary<int, decimal> _rsdPricing = new()
        {
            { 200, 250 }, { 250, 300 }, { 300, 350 }, { 350, 400 },
            { 400, 450 }, { 500, 550 }, { 550, 600 }, { 600, 650 },
            { 650, 700 }, { 700, 750 }, { 750, 800 }, { 800, 850 },
            { 850, 900 }, { 900, 950 }, { 950, 1000 }, { 1000, 1050 },
            { 1100, 1150 }, { 1200, 1200 }, { 1300, 1300 }, { 1400, 1350 },
            { 1500, 1450 }, { 1600, 1500 }, { 1700, 1600 }, { 1800, 1650 },
            { 1900, 1750 }, { 2000, 1800 }, { 2100, 1900 }, { 2200, 1950 },
            { 2300, 2050 }, { 2400, 2100 }, { 2500, 2150 }, { 2600, 2200 },
            { 2700, 2250 }, { 2800, 2300 }, { 2900, 2350 }, { 3000, 2450 },
            { 3100, 2500 }, { 3200, 2550 }, { 3300, 2600 }, { 3400, 2650 },
            { 3500, 2750 }, { 3600, 2800 }, { 3700, 2850 }, { 3800, 2900 },
            { 3900, 2950 }, { 4000, 3050 }, { 4100, 3100 }, { 4200, 3150 },
            { 4300, 3200 }, { 4400, 3250 }, { 4500, 3350 }, { 4600, 3400 },
            { 4700, 3450 }, { 4800, 3500 }, { 4900, 3550 }, { 5000, 3650 }
        };

        public async Task<FortniteShopResponseDto> GetDailyShopAsync()
        {
            // Pozivamo oficijalni API za shop (Samo Battle Royale shop)
            var response = await httpClient.GetAsync("https://fortnite-api.com/v2/shop");


            if (!response.IsSuccessStatusCode)
                throw new Exception("Neuspešno povezivanje sa Fortnite API serverom.");

            var jsonString = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(jsonString);
            var data = jsonDocument.RootElement.GetProperty("data");

            var shopResponse = new FortniteShopResponseDto
            {
                Date = data.GetProperty("date").GetString() ?? DateTime.UtcNow.ToString("yyyy-MM-dd"),
                ExpirationDate = DateTime.UtcNow.Date.AddDays(1) // Uvek gađa tačno sledeću ponoć!
            };

            var entries = data.GetProperty("entries");

            foreach (var entry in entries.EnumerateArray())
            {
                if (!entry.TryGetProperty("finalPrice", out var finalPriceElement)) continue;

                int vBucks = finalPriceElement.GetInt32();
                if (vBucks == 0) continue;

                // ISPRAVKA: API sada koristi reč "layout" za kategorije!
                string sectionName = "Shop";
                if (entry.TryGetProperty("layout", out var layoutObj) && layoutObj.ValueKind == JsonValueKind.Object)
                {
                    if (layoutObj.TryGetProperty("name", out var sName) && sName.ValueKind != JsonValueKind.Null)
                        sectionName = sName.GetString() ?? "Shop";
                }

                JsonElement itemsArray = default;
                if (entry.TryGetProperty("brItems", out var brItems)) itemsArray = brItems;
                else if (entry.TryGetProperty("cars", out var cars)) itemsArray = cars;
                else if (entry.TryGetProperty("instruments", out var inst)) itemsArray = inst;
                else if (entry.TryGetProperty("tracks", out var tracks)) itemsArray = tracks;
                else if (entry.TryGetProperty("items", out var oldItems)) itemsArray = oldItems;

                if (itemsArray.ValueKind == JsonValueKind.Undefined || itemsArray.GetArrayLength() == 0) continue;

                var firstItem = itemsArray[0];

                string name = "Nepoznato";
                if (firstItem.TryGetProperty("name", out var nameProp) && nameProp.ValueKind != JsonValueKind.Null)
                    name = nameProp.GetString() ?? "Nepoznato";

                string desc = "";
                if (firstItem.TryGetProperty("description", out var descProp) && descProp.ValueKind != JsonValueKind.Null)
                    desc = descProp.GetString() ?? "";

                string rarity = "Common";
                if (firstItem.TryGetProperty("rarity", out var rarityObj) && rarityObj.TryGetProperty("displayValue", out var rarityVal))
                    rarity = rarityVal.GetString() ?? "Common";

                string imageUrl = "";
                if (firstItem.TryGetProperty("images", out var images))
                {
                    if (images.TryGetProperty("icon", out var iconElement) && iconElement.ValueKind != JsonValueKind.Null)
                        imageUrl = iconElement.GetString() ?? "";
                }

                if (entry.TryGetProperty("bundle", out var bundle) && bundle.ValueKind == JsonValueKind.Object)
                {
                    if (bundle.TryGetProperty("name", out var bName) && bName.ValueKind != JsonValueKind.Null)
                        name = bName.GetString() ?? name;

                    if (bundle.TryGetProperty("image", out var bImage) && bImage.ValueKind != JsonValueKind.Null)
                        imageUrl = bImage.GetString() ?? imageUrl;
                }

                // ARMIRANI FILTER: Ako je Nepoznato ili nema sliku, preskačemo celokupan upis!
                if (name == "Nepoznato" || string.IsNullOrWhiteSpace(imageUrl))
                    continue;

                decimal rsdPrice = CalculateRsdPrice(vBucks);

                shopResponse.Items.Add(new FortniteItemDto
                {
                    Name = name,
                    Description = desc,
                    Rarity = rarity,
                    ImageUrl = imageUrl,
                    Section = sectionName, // Očekujemo "The Mandalorian", "Jam Tracks", itd.
                    VBucksPrice = vBucks,
                    RsdPrice = rsdPrice
                });
            }

            return shopResponse;
        }

        // Metoda koja provlači V-Bucks kroz tvoj cenovnik
        private static decimal CalculateRsdPrice(int vBucks)
        {
            // Ako cena postoji u tvom cenovniku, vrati je tačno u dinar
            if (_rsdPricing.TryGetValue(vBucks, out decimal exactPrice))
                return exactPrice;

            // Sigurnosna mreža: Ako Fortnite izbaci neku nepostojeću cenu u budućnosti (npr. 875 V-Bucks),
            // tražimo najbližu cenu iz tvog cenovnika da aplikacija ne bi pukla.
            int closestKey = _rsdPricing.Keys.OrderBy(k => Math.Abs(k - vBucks)).First();
            return _rsdPricing[closestKey];
        }
    }
}