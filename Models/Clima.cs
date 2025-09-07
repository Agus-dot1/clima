using System.Text.Json.Serialization;

namespace clima.Models
{
    public class WeatherResponse
    {
        [JsonPropertyName("location")]
        public Location Location { get; set; }

        [JsonPropertyName("current")]
        public Current Current { get; set; }
    }

    public class Location
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("localtime")]
        public string LocalTime { get; set; }
    }

    public class Current
    {
        [JsonPropertyName("temp_c")]
        public double TemperatureC { get; set; }

        [JsonPropertyName("temp_f")]
        public double TemperatureF { get; set; }

        [JsonPropertyName("condition")]
        public Condition Condition { get; set; }

        [JsonPropertyName("wind_kph")]
        public double WindKph { get; set; }

        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }

        [JsonPropertyName("feelslike_c")]
        public double FeelsLikeC { get; set; }

        [JsonPropertyName("feelslike_f")]
        public double FeelsLikeF { get; set; }
    }


    public class Condition
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }
    }

}

