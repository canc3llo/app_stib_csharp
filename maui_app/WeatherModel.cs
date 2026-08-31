using System.Text.Json.Serialization;

namespace maui_app;

public class WeatherResponse
{
    [JsonPropertyName("current")]
    public CurrentWeather Current { get; set; }
}

public class CurrentWeather
{
    [JsonPropertyName("temperature_2m")]
    public double Temperature2m { get; set; }

    [JsonPropertyName("weather_code")]
    public int WeatherCode { get; set; }
}