using System.Text.Json;
using CommunityToolkit.Maui.Extensions;

namespace maui_app;

public partial class MainPage : ContentPage
{
    private readonly LinesRepertory _linesRepresent;
    private readonly WeatherService _weatherService;

    public MainPage()
    {
        InitializeComponent();
        _linesRepresent = new LinesRepertory();
        BindingContext = _linesRepresent;
        _weatherService = new WeatherService(); // 1) créer l'instance
        LoadWeather();                           // 2) déclencher l'appel
    }
    
    private async void LoadWeather()
    {
        var weather = await _weatherService.GetBrusselsWeatherAsync(); // 3) appeler la méthode

        if (weather != null)
        {
            WeatherLabel.Text = $"{weather.Current.Temperature2m}°C";
            WeatherIconLabel.Text = GetWeatherIcon(weather.Current.WeatherCode);
        }
        else
        {
            WeatherLabel.Text = "Erreur météo";
            WeatherIconLabel.Text = "⚠️";
        }
    }

    // Mapping des codes météo WMO (Open-Meteo) vers un emoji
    private static string GetWeatherIcon(int weatherCode) => weatherCode switch
    {
        0 => "☀️",                     // Ciel dégagé
        1 or 2  => "⛅",  
        3 => "☁️",                      // Peu nuageux à couvert
        45 or 48 => "🌫️",              // Brouillard
        51 or 53 or 55 => "️🌧️",        // Bruine
        56 or 57 => "🌧️",              // Bruine verglaçante
        61 or 63 or 65 => "🌧️",        // Pluie
        66 or 67 => "🌧️",              // Pluie verglaçante
        71 or 73 or 75 or 77 => "❄️",   // Neige
        80 or 81 or 82 => "🌧️",        // Averses de pluie
        85 or 86 => "🌨️",              // Averses de neige
        95 or 96 or 99 => "⛈️",         // Orage
        _ => "🌡️"
    };

    private async void OnLineTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not TransportIndex line)
            return; // pour l'instant, seule la ligne 1 ouvre le panneau

        var popup = new LineDetailsPopup(line);
        popup.CloseRequested += async (_, _) => await this.ClosePopupAsync();

        await this.ShowPopupAsync(popup);
    }

    private void OnAllClicked(object sender, EventArgs e) => _linesRepresent.ShowAll();
    private void OnBusClicked(object sender, EventArgs e) => _linesRepresent.ShowBus();
    private void OnTramClicked(object sender, EventArgs e) => _linesRepresent.ShowTram();
    private void OnMetroClicked(object sender, EventArgs e) => _linesRepresent.ShowMetro();
    
    private void OnLignesClicked(object sender, EventArgs e)
    {
        FavorisGrid.IsVisible = false;
        LinesGrid.IsVisible = true;
    }

    private void OnFavClicked(object sender, EventArgs e)
    {
        FavorisGrid.IsVisible = true;
        LinesGrid.IsVisible = false;
    }

    /*
    private async void OnGetWaitingTimesClicked(object? sender, EventArgs e)
    {
        using var client = new HttpClient();

        // Appel 1 : temps d'attente
        string waitingUrl = "https://api-management-discovery-production.azure-api.net/api/datasets/stibmivb/rt/WaitingTimes";
        string waitingJson = await client.GetStringAsync(waitingUrl);
        var waitingData = JsonSerializer.Deserialize<WaitingTimeResponse>(waitingJson);

        // Appel 2 : détails des arrêts (noms)
        string detailsUrl = "https://api-management-discovery-production.azure-api.net/api/datasets/stibmivb/static/StopDetails";
        string detailsJson = await client.GetStringAsync(detailsUrl);
        var detailsData = JsonSerializer.Deserialize<StopDetailsResponse>(detailsJson);

        if (waitingData?.Results != null && detailsData?.Results != null)
        {
            var premierArret = waitingData.Results[1];
            string? nomArret;

            // On cherche le détail correspondant via le pointid
            var detail = detailsData.Results.FirstOrDefault(d => d.Id == premierArret.PointId);
            if (detail != null)
            {
                nomArret = JsonSerializer.Deserialize<StopName>(detail.NameRaw)?.Fr;
            }
            else
            {
                nomArret = "Arrêt inconnu";
            }

            var passages = JsonSerializer.Deserialize<List<PassingTime>>(premierArret.PassingTimesRaw);

            if (passages != null && passages.Count > 0)
            {
                var premierPassage = passages[0];
                //ResultLabel.Text = $"{nomArret} — Ligne {premierArret.LineId} vers {premierPassage.Destination.Fr} : {premierPassage.ExpectedArrivalTime:HH:mm}";
            }
            
        }
        
    }
    */
}