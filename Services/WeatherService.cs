using System.Text.Json;
using clima.Models;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private const string WeatherBaseUrl = "https://api.open-meteo.com/v1/forecast";
    private const string GeocodingBaseUrl = "https://geocoding-api.open-meteo.com/v1/search";

    public WeatherService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<WeatherResponse> GetCurrentWeatherAsync(string location)
    {
        var coordinates = await GetCoordinatesAsync(location);
    }

    private async Task<GeoLocation> GetCoordinatesAsync(string location)
    {
        string geocodingUrl = $"{GeocodingBaseUrl}?name={Uri.EscapeDataString(location)}&count=3?&language=es";

        try
        {
            var response = _httpClient.GetAsync(geocodingUrl);
            string jsonContent = await response.Result.Content.ReadAsStringAsync();
            var geocodingData = JsonSerializer.Deserialize<GeocodingResponse>(jsonContent);

            if (geocodingData?.Results?.Any() == true)
            {
                return geocodingData.Results.First();
            }
            else
            {
                throw new Exception($"No se encontró! '{location}', probá ser mas específico.");
            }
     



        }
        catch (HttpRequestException ex)
        {
            Console.Write(ex);
        }

    }

}
