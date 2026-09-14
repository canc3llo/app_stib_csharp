using System.Collections.ObjectModel;
using System.Linq;

namespace maui_app;

public partial class LineDetailsPopup : ContentView
{
    private readonly StibLineService _stibService = new();
    private List<LineRoute> _routes = new();
    private List<StopResult> _times = new();
    private int _currentRouteIndex;

    public ObservableCollection<RouteStop> Stops { get; } = new();

    public LineDetailsPopup()
    {
        InitializeComponent();
        BindingContext = this;
    }

    public void SetLine(TransportIndex line)
    {
        TitleImage.Source = line.ImageSource;
        _ = LoadRoutesAsync(line.Nr);
    }

    private async Task LoadRoutesAsync(int lineNr)
    {
        Stops.Clear();
        DirectionLabel.Text = "Chargement...";

        try
        {
            _times = await _stibService.StopTime(lineNr);
        }
        catch (Exception)
        {
            _times = new();
            DirectionLabel.Text = "Erreur de chargement des horaires";
        }

        try
        {
            _routes = await _stibService.GetRoutesAsync(lineNr);
        }
        catch (Exception)
        {
            _routes = new();
            DirectionLabel.Text = "Erreur de chargement des routes";
        }
        
        _currentRouteIndex = 0;
        ShowCurrentRoute();
    }

    private void ShowCurrentRoute()
    {
        Stops.Clear();

        if (_routes.Count == 0)
        {
            DirectionLabel.Text = "Aucune donnée";
            return;
        }

        var route = _routes[_currentRouteIndex];
        DirectionLabel.Text = $"→ {route.Destination}";

        foreach (var stop in route.Stops)
        {
            var matchingTime = _times.FirstOrDefault(t => t.PointId == stop.Id);
            stop.Times = matchingTime?.ExpectedArrivalTime ?? new();

            stop.Time1 = FormatMinutes(stop.Times.ElementAtOrDefault(0));
            stop.Time2 = FormatMinutes(stop.Times.ElementAtOrDefault(1));

            Stops.Add(stop);
        }
    }

    private static string FormatMinutes(DateTimeOffset arrival)
    {
        if (arrival == default)
            return "";

        var minutes = (int)(arrival - DateTimeOffset.Now).TotalMinutes;
        return minutes <= 0 ? "0" : minutes.ToString();
    }

    private void OnSwapDirectionClicked(object sender, EventArgs e)
    {
        if (_routes.Count == 0)
            return;

        _currentRouteIndex = (_currentRouteIndex + 1) % _routes.Count;
        ShowCurrentRoute();
    }

    private void OnCloseClicked(object sender, EventArgs e) => IsVisible = false;
}
