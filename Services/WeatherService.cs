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
        table.Title($"Clima de hoy en {config.Name}");

        table.AddColumn(new TableColumn("Hora").Centered());
        table.AddColumn(new TableColumn("Temperatura (°C)").Centered());
        table.AddColumn(new TableColumn("Clima").Centered());

        int[] posiciones = { 8, 10, 12, 14, 16, 18, 20, 22, 23 };

        string ColorearTemp(double temp)
        {
            if (temp < 18) return $"[blue]{temp:0.#}°C[/]";
            if (temp < 25) return $"[yellow]{temp:0.#}°C[/]";
            return $"[red]{temp:0.#}°C[/]";
        }

        string IconoClima(int hora)
        {
            if (hora >= 6 && hora <= 18)
            {
                if (hora < 10) return "☀";
                if (hora < 16) return " ⛅";
                return "☁";
            }
            else
            {
                return "  🌛";
            }
        }

        foreach (var pos in posiciones)
        {
            table.AddRow(
                $"[bold]{pos:00}:00[/]",
                ColorearTemp(result.Hourly.TemperatureC[pos + 2]),
                IconoClima(pos)
            );
        }

        string nubesAscii = @"
      .--.                 .---.
   .-(    )..--.        .-(     ).
  (___.__)__)___).     (___.__)___)
";

        AnsiConsole.WriteLine(nubesAscii);
        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine("Temperaturas de la semana:");


        var grid = new Grid();
        
        // Cantidad de columnas = cantidad de días
        for (int i = 0; i < result.Daily.temperature_2m_mean.Length; i++)
        {
            grid.AddColumn(new GridColumn().Centered());
        }

        int aux = 0;
        List<IRenderable> columnas = new();

        foreach (var _ in result.Daily.temperature_2m_mean)
        {
            double mean = result.Daily.temperature_2m_mean[aux];
            double min = result.Daily.temperature_2m_min[aux];
            double max = result.Daily.temperature_2m_max[aux];

            string icono = mean switch
            {
                < 15 => "☁",
                < 22 => "⛅",
                < 28 => "☀",
                _ => "🔥"
            };

            var dayTable = new Table();
            dayTable.Border = TableBorder.Rounded;
            dayTable.AddColumn(new TableColumn($"[bold]{icono} Día {aux + 1}[/]").Centered().Width(25));
            dayTable.AddRow($"[yellow]Promedio: {mean:0.#}°C[/]");
            dayTable.AddRow($"[blue]Min: {min:0.#}°C[/]\n[red]Max: {max:0.#}°C[/]");

            columnas.Add(dayTable);
            aux++;
        }

        grid.AddRow(columnas.ToArray());
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


    public async Task<GeoLocation> GetCoordinatesAsync(string location, bool settingWeather)
    {
        var config = ConfigService.LoadPreferences();
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

            if (settingWeather)
            {

                var countrySelected = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Seleccione el mas cercano").AddChoices(countries));

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
            throw new Exception($"Error al obtener coordenadas para '{location}'", ex);
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }

}
