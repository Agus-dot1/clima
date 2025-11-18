using System.Text.Json;
using System.Globalization;
using clima.Models;
using Spectre.Console;

public class WeatherService : IDisposable
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
        try
        {
            var coordinates = await GetCoordinatesAsync(location);

            string weatherUrl =
                $"{WeatherBaseUrl}?latitude={coordinates.Latitude.ToString(CultureInfo.InvariantCulture)}&longitude={coordinates.Longitude.ToString(CultureInfo.InvariantCulture)}&hourly=temperature_2m&daily=temperature_2m_min,temperature_2m_mean,temperature_2m_max";



            var response = await _httpClient.GetAsync(weatherUrl);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error en open-meteo {response.StatusCode}");
            }

            string jsonContent = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<WeatherResponse>(jsonContent);
            if (result == null)
            {
                AnsiConsole.MarkupLine("[red]Error: no se pudo deserializar la respuesta[/]");
                return null!;
            }

            return result;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"Error: {ex.Message}");
            return null;
        }
    }


    public async Task<GeoLocation> GetCoordinatesAsync(string location)
    {
        string geocodingUrl = $"{GeocodingBaseUrl}?name={Uri.EscapeDataString(location)}&count=5&language=es";
        try
        {
            var response = await _httpClient.GetAsync(geocodingUrl);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error en geocoding {response.StatusCode}");
            }

            string jsonContent = await response.Content.ReadAsStringAsync();

            var geocodingData = JsonSerializer.Deserialize<GeocodingResponse>(jsonContent);


            if (geocodingData?.Results?.Any() != true)
                throw new Exception($"No se encontró '{location}', probá ser más específico.");


            List<string> countries = new List<string>();
            foreach (var result in geocodingData.Results)
            {
                countries.Add(result.DisplayName);
            }

            var countrySelected = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Seleccione el mas cercano").AddChoices(countries));

            foreach (var result in geocodingData.Results)
            {
                if (countrySelected == result.DisplayName)
                {
                    return result;
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener coordenadas para '{location}'", ex);
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }

}
