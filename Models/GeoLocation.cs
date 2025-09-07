using System.Text.Json.Serialization;

public class GeocodingResponse
{
    [JsonPropertyName("results")]
    public List<GeoLocation> Results { get; set; } = new();
}

// Models/GeoLocation.cs  
public class GeoLocation
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("admin1")]  
    public string Region { get; set; }

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    public string DisplayName => $"{Name}, {Region}, {Country}";
}
