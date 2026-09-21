using System.Collections.ObjectModel;
using System.Linq;

namespace maui_app;

public partial class LineDetailsPopup : ContentView
{
    private readonly StibLineService _stibService = new();
    private List<LineRoute> _routes = new();
    private List<StopResult> _times = new();
    private int _currentRouteIndex;
    private int _currentLineNr;
    private TransportIndex _currentLine;
    private IDispatcherTimer? _refreshTimer;

    public LinesRepertory FavorisManager { get; set; }

    public ObservableCollection<RouteStop> Stops { get; } = new();

    public LineDetailsPopup()
    {
        InitializeComponent();
        BindingContext = this;
    }

    public void SetLine(TransportIndex line)
    {
        TitleImage.Source = line.ImageSource;
        _currentLineNr = line.Nr;
        _currentLine = line;
        ChangeColorFavorisButton();
        _ = LoadRoutesAsync(line.Nr);
        StartAutoRefresh();
    }

    private void StartAutoRefresh()
    {
        if (_refreshTimer == null)
        {
            _refreshTimer = Dispatcher.CreateTimer();
            _refreshTimer.Interval = TimeSpan.FromMinutes(1);
            _refreshTimer.Tick += async (s, e) => await RefreshTimesAsync();
        }

        _refreshTimer.Stop();
        _refreshTimer.Start();
    }

    private async Task RefreshTimesAsync()
    {
        try
        {
            _times = await _stibService.StopTime(_currentLineNr);
        }
        catch (Exception)
        {
            // en cas d'échec on garde les derniers temps connus affichés
            return;
        }

        ShowCurrentRoute();
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
            
            string T1 = FormatMinutes(stop.Times.ElementAtOrDefault(0));
            string T2 = FormatMinutes(stop.Times.ElementAtOrDefault(1));
            
            if (T1 == "0")
            {
                stop.Time1 = "↓↓";
            }
            else
            {
                stop.Time1 = $"{T1}'";
            }
            if (T2 == "0")
            {
                stop.Time2 = "↓↓";
            }
            else
            {
                stop.Time2 = $"{T2}'";
            }

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

    private void OnCloseClicked(object sender, EventArgs e)
    {
        IsVisible = false;
        _refreshTimer?.Stop();
    }
    
    private void OnFavorisClicked(object sender, EventArgs e)
    {
        if (_currentLine.IsFavorite)
        {
            FavorisManager.RetirerFavori(_currentLine);
            _currentLine.IsFavorite = false;
        }
        else
        {
            FavorisManager.SauvegarderFavoris(_currentLine);
            _currentLine.IsFavorite = true;
        }
        ChangeColorFavorisButton();
    }
    
    private void ChangeColorFavorisButton()
    {
        if (_currentLine.IsFavorite)
        {
            btnfavoris.BackgroundColor = Colors.DarkRed;
        }
        else
        {
            btnfavoris.BackgroundColor = Colors.White;
        }
    }
}
