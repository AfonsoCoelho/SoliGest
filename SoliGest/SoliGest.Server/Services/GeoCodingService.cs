using System.Text.Json;
using SoliGest.Server.Models;

namespace SoliGest.Server.Services
{

    public interface IGeoCodingService
    {
        Task<GeocodeResult?> GeocodeAsync(string address);
    }

    public class GeoCodingService : IGeoCodingService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;
        public GeoCodingService(HttpClient http, IConfiguration cfg)
        {
            _http = http;
            _apiKey = cfg["GoogleMaps:ApiKey"]!;
        }
        public async Task<GeocodeResult?> GeocodeAsync(string address)
        {
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={_apiKey}";

            var resp = await _http.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            
            using var doc = await JsonDocument.ParseAsync(await resp.Content.ReadAsStreamAsync());
            
            if (doc.RootElement.GetProperty("status").GetString() != "OK") return null;

            Console.WriteLine("\n\n returned \n\n" + doc + "\n\n\n\n\n");

            var loc = doc.RootElement.GetProperty("results")[0]
                       .GetProperty("geometry").GetProperty("location");

            return new GeocodeResult
            {
                Latitude = loc.GetProperty("lat").GetDouble(),
                Longitude = loc.GetProperty("lng").GetDouble()
            };
        }
    }

}
