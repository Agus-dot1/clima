using System.Text.Json.Serialization;

namespace clima.Models
{
    public class WeatherResponse
    {
        [JsonPropertyName("location")]
        public Location Location { get; init; }

        [JsonPropertyName("hourly")]
        public Hourly Hourly { get; init; }

        [JsonPropertyName("daily")]
        public Daily Daily { get; init; }

    }


    public class Location
    {
        [JsonPropertyName("name")]
        public required string Name { get; init; }

        [JsonPropertyName("region")]
        public required string Region { get; init; }

        [JsonPropertyName("country")]
        public required string Country { get; init; }

        [JsonPropertyName("localtime")]
        public DateTime LocalTime { get; init; }
    }


    public class Hourly
    {
        [JsonPropertyName("time")]
        public DateTime[] Time { get; init; }

        [JsonPropertyName("temperature_2m")]
        public double[] TemperatureC { get; init; }    

        [JsonPropertyName("condition")]
        public Condition Condition { get; init; }

        [JsonPropertyName("wind_kph")]
        public double WindKph { get; init; }

        [JsonPropertyName("humidity")]
        public byte Humidity { get; init; }

        [JsonPropertyName("apparent_temperature")]
        public double[] apparent_temperature { get; init; }

        [JsonPropertyName("feelslike_f")]
        public double FeelsLikeFahrenheit { get; init; }
    }

    public class Daily {
        [JsonPropertyName("temperature_2m_min")]
        public double[] temperature_2m_min { get; init; }

        [JsonPropertyName("temperature_2m_mean")]
        public double[] temperature_2m_mean { get; init; }

        [JsonPropertyName("temperature_2m_max")]
        public double[] temperature_2m_max { get; init; }
    }


    public class Condition
    {
        [JsonPropertyName("text")]
        public required string Text { get; set; }

        [JsonPropertyName("icon")]
        public required string Icon { get; set; }
    }

}

