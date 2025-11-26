using System.Text.Json;
using System.Globalization;
using clima.Models;
using Spectre.Console;
using Spectre.Console.Rendering;

public class WeatherService : IDisposable
{
    private readonly HttpClient _httpClient;
    private const string WeatherBaseUrl = "https://api.open-meteo.com/v1/forecast";
    private const string GeocodingBaseUrl = "https://geocoding-api.open-meteo.com/v1/search";



    public WeatherService()
    {
        _httpClient = new HttpClient();
    }

    public async Task ShowWeather()
    {
        UserPreferences config = ConfigService.LoadPreferences();
        var result = await GetCurrentWeatherAsync(config.Name, false);

        var table = new Table();
        table.Border = TableBorder.Rounded;
        table.Title($"Today's weather in {config.Name}");

        table.AddColumn(new TableColumn("Time").Centered());
        table.AddColumn(new TableColumn("Temperature (°C)").Centered());
        table.AddColumn(new TableColumn("Weather").Centered());

        int[] positions = { 8, 10, 12, 14, 16, 18, 20, 22, 23 };

        string ColorizeTemp(double temp)
        {
            if (temp < 18) return $"[blue]{temp:0.#}°C[/]";
            if (temp < 25) return $"[yellow]{temp:0.#}°C[/]";
            return $"[red]{temp:0.#}°C[/]";
        }

        string GetWeatherIcon(int hour)
        {
            if (hour >= 6 && hour <= 18)
            {
                if (hour < 10) return "☀";
                if (hour < 16) return " ⛅";
                return "☁";
            }
            else
            {
                return "  🌛";
            }
        }

        foreach (var pos in positions)
        {
            table.AddRow(
                $"[bold]{pos:00}:00[/]",
                ColorizeTemp(result.Hourly.TemperatureC[pos + 2]),
                GetWeatherIcon(pos)
            );
        }

        string cloudsAscii = @"
      .--.                 .---.
   .-(    )..--.        .-(     ).
  (___.__)__)___).     (___.__)___)
";

        AnsiConsole.WriteLine(cloudsAscii);
        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine("Weekly temperatures:");


        var grid = new Grid();
        
        // Cantidad de columnas = cantidad de días
        for (int i = 0; i < result.Daily.temperature_2m_mean.Length; i++)
        {
            grid.AddColumn(new GridColumn().Centered());
        }

        int dayIndex = 0;
        List<IRenderable> columns = new();

        foreach (var _ in result.Daily.temperature_2m_mean)
        {
            double mean = result.Daily.temperature_2m_mean[dayIndex];
            double min = result.Daily.temperature_2m_min[dayIndex];
            double max = result.Daily.temperature_2m_max[dayIndex];

            string icon = mean switch
            {
                < 15 => "☁",
                < 22 => "⛅",
                < 28 => "☀",
                _ => "🔥"
            };

            var dayTable = new Table();
            dayTable.Border = TableBorder.Rounded;
            dayTable.AddColumn(new TableColumn($"[bold]{icon} Day {dayIndex + 1}[/]").Centered().Width(25));
            dayTable.AddRow($"[yellow]Average: {mean:0.#}°C[/]");
            dayTable.AddRow($"[blue]Min: {min:0.#}°C[/]\n[red]Max: {max:0.#}°C[/]");

            columns.Add(dayTable);
            dayIndex++;
        }

        grid.AddRow(columns.ToArray());
        AnsiConsole.Write(grid);

    }


    public async Task<WeatherResponse> GetCurrentWeatherAsync(string location, bool settingWeather)
    {
        try
        {
            var coordinates = await GetCoordinatesAsync(location, settingWeather);

            string weatherUrl =
                $"{WeatherBaseUrl}?latitude={coordinates.Latitude.ToString(CultureInfo.InvariantCulture)}&longitude={coordinates.Longitude.ToString(CultureInfo.InvariantCulture)}&hourly=temperature_2m&daily=temperature_2m_min,temperature_2m_mean,temperature_2m_max";



            var response = await _httpClient.GetAsync(weatherUrl);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error in open-meteo {response.StatusCode}");
            }

            string jsonContent = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<WeatherResponse>(jsonContent);
            if (result == null)
            {
                AnsiConsole.MarkupLine("[red]Error: could not deserialize response[/]");
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


    public async Task<GeoLocation> GetCoordinatesAsync(string location, bool settingWeather)
    {
        var config = ConfigService.LoadPreferences();
        string geocodingUrl = $"{GeocodingBaseUrl}?name={Uri.EscapeDataString(location)}&count=5&language=en";
        try
        {
            var response = await _httpClient.GetAsync(geocodingUrl);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error in geocoding {response.StatusCode}");
            }

            string jsonContent = await response.Content.ReadAsStringAsync();

            var geocodingData = JsonSerializer.Deserialize<GeocodingResponse>(jsonContent);


            if (geocodingData?.Results?.Any() != true)
                throw new Exception($"'{location}' not found, try being more specific.");


            List<string> countries = new List<string>();
            foreach (var result in geocodingData.Results)
            {
                countries.Add(result.DisplayName);
            }

            if (settingWeather)
            {

                var countrySelected = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Select the closest one").AddChoices(countries));

                foreach (var result in geocodingData.Results)
                {
                    if (countrySelected == result.DisplayName)
                    {
                        config.Name = result.Name;
                        config.City = result.DisplayName;
                        ConfigService.SavePreferences(config);
                        return result;
                    }
                }
            }
            else
            {
                foreach (var result in geocodingData.Results)
                {
                    if (config.City == result.DisplayName)
                        return result;
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting coordinates for '{location}'", ex);
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }

}
