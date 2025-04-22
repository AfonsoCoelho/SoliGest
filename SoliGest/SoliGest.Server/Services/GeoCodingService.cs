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

            var root = doc.RootElement;
            var status = root.GetProperty("status").GetString();

            if (status != "OK")
            {
                var errorMsg = root.GetProperty("error_message").GetString();
                Console.WriteLine($"Status não-OK: {status}, erro: {errorMsg}");
                return new GeocodeResult { Latitude = 1.1, Longitude = 1.1 };
            }

            var results = root.GetProperty("results");
            if (results.GetArrayLength() == 0)
            {
                Console.WriteLine("ZERO_RESULTS: nenhum resultado encontrado.");
                return new GeocodeResult { Latitude = 0.0, Longitude = 0.0 };
            }

            var loc = results.GetProperty("geometry").GetProperty("location");
            Console.WriteLine("location JSON: " + loc.GetRawText());

            return new GeocodeResult
            {
                Latitude = loc.GetProperty("lat").GetDouble(),
                Longitude = loc.GetProperty("lng").GetDouble()
            };
        }
    }

}
