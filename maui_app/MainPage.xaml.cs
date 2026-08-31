using System.Text.Json;

namespace maui_app;

public partial class MainPage : ContentPage
{
    private readonly LinesRepertory _linesRepresent;

    public MainPage()
    {
        InitializeComponent();
        _linesRepresent = new LinesRepertory();
        BindingContext = _linesRepresent;
    }

    private void OnAllClicked(object sender, EventArgs e) => _linesRepresent.ShowAll();
    private void OnBusClicked(object sender, EventArgs e) => _linesRepresent.ShowBus();
    private void OnTramClicked(object sender, EventArgs e) => _linesRepresent.ShowTram();
    private void OnMetroClicked(object sender, EventArgs e) => _linesRepresent.ShowMetro();
    
    private void OnLignesClicked(object sender, EventArgs e)
    {
        FilterMenu.IsVisible = true;
        LinesView.IsVisible = true;
        FavorisView.IsVisible = false;
    }

    private void OnFavClicked(object sender, EventArgs e)
    {
        FilterMenu.IsVisible = false;
        LinesView.IsVisible = false;
        FavorisView.IsVisible = true;
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