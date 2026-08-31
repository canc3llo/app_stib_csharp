using System.Net.Http.Json;

namespace maui_app;

public class WeatherService
{
    private readonly HttpClient _httpClient = new();
    private const string Url = "https://api.open-meteo.com/v1/forecast?latitude=50.85&longitude=4.35&current=temperature_2m,weather_code&timezone=Europe/Brussels";

    public async Task<WeatherResponse?> GetBrusselsWeatherAsync()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<WeatherResponse>(Url);
            System.Diagnostics.Debug.WriteLine("Météo récupérée avec succès !");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur météo : {ex.Message}");
            return null;
        }
    }
}