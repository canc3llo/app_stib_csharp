using System.Net.Http.Json;
using System.Text.Json;

namespace maui_app;

public class StibLineService
{
    private const string StopsByLineUrl = "https://api-management-discovery-production.azure-api.net/api/datasets/stibmivb/static/stopsByLine";
    private const string StopDetailsUrl = "https://api-management-discovery-production.azure-api.net/api/datasets/stibmivb/static/StopDetails";
    private const string TimeByStopUrl = "https://api-management-discovery-production.azure-api.net/api/datasets/stibmivb/rt/WaitingTimes";

    private readonly HttpClient _httpClient = new();

    private List<StopResult>? _stopTimes;
    private List<StopsByLineResult>? _stopsByLine;
    private Dictionary<string, string>? _stopNames;
    private Task? _loadTask;

    /*
    public async Task<List<LineRoute>> GetRoutesAsync(int lineNr)
    {
        await EnsureLoadedAsync();

        var lineId = lineNr.ToString();

        return _stopsByLine!
            .Where(r => r.LineId == lineId)
            .Select(BuildRoute)
            .Where(route => route.Stops.Count > 0)
            .ToList();
    }
    */
    
    public async Task<List<LineRoute>> GetRoutesAsync(int lineNr)
    {
      
        await EnsureLoadedAsync();
        string lineId = lineNr.ToString();
        List<LineRoute> resultat = new List<LineRoute>();
        
        foreach (StopsByLineResult ligneBrute in _stopsByLine)
        {
            if (ligneBrute.LineId != lineId)
            {
                continue;
            }
            
            LineRoute route = BuildRoute(ligneBrute);
            
            if (route.Stops.Count == 0)
            {
                continue;
            }
            resultat.Add(route);
        }
        return resultat;
    }
    
    public async Task<List<StopResult>> StopTime(int lineNr)
    {
        await EnsureLoadedAsync();
        string lineId = lineNr.ToString();
       // return _stopTimes!.Where(r => r.LineId == lineId).ToList();
        
        return _stopTimes!
            .Where(r => r.LineId == lineId)
            .Select(BuildTime)
            .ToList();
    }

    /*
    private LineRoute BuildRoute(StopsByLineResult result)
    {
        var destination = JsonSerializer.Deserialize<StopName>(result.DestinationRaw)?.Fr ?? "";
        var points = JsonSerializer.Deserialize<List<RoutePoint>>(result.PointsRaw) ?? new();

        var stops = points
            .OrderBy(p => p.Order)
            .Select(p => new RouteStop
            {
                Order = p.Order,
                Name = _stopNames!.TryGetValue(p.Id, out var name) ? name : "?"
            })
            .ToList();

        if (stops.Count > 0)
        {
            stops[0].IsTerminus = true;
            stops[^1].IsTerminus = true;
        }

        return new LineRoute
        {
            Direction = result.Direction,
            Destination = destination,
            Stops = stops
        };
    }
    */
    
    private StopResult BuildTime(StopResult result)
    {
        List<PassingTimeRaw> passingTimes = JsonSerializer.Deserialize<List<PassingTimeRaw>>(result.PassingTimesRaw) ?? new();

        result.ExpectedArrivalTime = passingTimes.Select(p => p.ExpectedArrivalTime).ToList();

        string name;
        bool trouve = _stopNames.TryGetValue(result.PointId, out name);

        result.StopName = trouve ? name : "?";

        return result;
    }
    
    
    private LineRoute BuildRoute(StopsByLineResult result)
    {
        // Etape 1 : décoder le nom de la destination (JSON imbriqué)
        StopName destinationObjet = JsonSerializer.Deserialize<StopName>(result.DestinationRaw);
        string destination;
        if (destinationObjet != null && destinationObjet.Fr != null)
        {
            destination = destinationObjet.Fr;
        }
        else
        {
            destination = "";
        }

        // Etape 2 : décoder la liste des points du trajet
        List<RoutePoint> points = JsonSerializer.Deserialize<List<RoutePoint>>(result.PointsRaw);
        if (points == null)
        {
            points = new List<RoutePoint>();
        }

        // Etape 3 : trier les points par ordre
        List<RoutePoint> pointsTries = points.OrderBy(p => p.Order).ToList();

        // Etape 4 : construire la liste des arrêts (RouteStop) à partir des points triés
        List<RouteStop> stops = new List<RouteStop>();

        foreach (RoutePoint p in pointsTries)
        {
            string name;
            bool trouve = _stopNames.TryGetValue(p.Id, out name);

            if (!trouve)
            {
                name = "?";
            }

            RouteStop stop = new RouteStop
            {
                Id = p.Id,
                Order = p.Order,
                Name = name
            };

            stops.Add(stop);
        }

        // Etape 5 : marquer le premier et le dernier arrêt comme terminus
        if (stops.Count > 0)
        {
            stops[0].IsTerminus = true;
            stops[stops.Count - 1].IsTerminus = true;
        }

        // Etape 6 : construire et renvoyer l'objet final
        LineRoute route = new LineRoute();
        route.Direction = result.Direction;
        route.Destination = destination;
        route.Stops = stops;

        return route;
    }
    
    private Task EnsureLoadedAsync()
    {
        return _loadTask ??= LoadAsync();
    }

    /*
    private async Task LoadAsync()
    {
        var stopsByLineTask = _httpClient.GetFromJsonAsync<StopsByLineResponse>(StopsByLineUrl);
        var stopDetailsTask = _httpClient.GetFromJsonAsync<StopDetailsResponse>(StopDetailsUrl);

        await Task.WhenAll(stopsByLineTask, stopDetailsTask);

        _stopsByLine = stopsByLineTask.Result?.Results ?? new();

        _stopNames = new Dictionary<string, string>();
        foreach (var stop in stopDetailsTask.Result?.Results ?? new())
        {
            var name = JsonSerializer.Deserialize<StopName>(stop.NameRaw)?.Fr;
            if (name != null)
                _stopNames[stop.Id] = name;
        }
    }
    */
    
    private async Task LoadAsync()
    {
        // démarrer les deux requêtes HTTP (sans attendre encore)
        Task<StopsByLineResponse> stopsByLineTask = _httpClient.GetFromJsonAsync<StopsByLineResponse>(StopsByLineUrl);
        Task<StopDetailsResponse> stopDetailsTask = _httpClient.GetFromJsonAsync<StopDetailsResponse>(StopDetailsUrl);
        Task<WaitingTimeResponse> timeTask = _httpClient.GetFromJsonAsync<WaitingTimeResponse>(TimeByStopUrl);
        
        await Task.WhenAll(stopsByLineTask, stopDetailsTask, timeTask);
        
        StopsByLineResponse reponse1 = stopsByLineTask.Result;
        if (reponse1 != null && reponse1.Results != null)
        {
            _stopsByLine = reponse1.Results;
        }
        else
        {
            _stopsByLine = new List<StopsByLineResult>();
        }
        
        StopDetailsResponse reponse2 = stopDetailsTask.Result;
        List<StopDetail> listeDetails;
        if (reponse2 != null && reponse2.Results != null)
        {
            listeDetails = reponse2.Results;
        }
        else
        {
            listeDetails = new List<StopDetail>(); 
        }
        
        _stopNames = new Dictionary<string, string>();

        foreach (StopDetail stop in listeDetails)
        {
            StopName stopNameObjet = JsonSerializer.Deserialize<StopName>(stop.NameRaw);

            string name = null;
            if (stopNameObjet != null)
            {
                name = stopNameObjet.Fr;
            }

            if (name != null)
            {
                _stopNames[stop.Id] = name;
            }
        }
        
        WaitingTimeResponse timeResponse = timeTask.Result;
        if (timeResponse != null && timeResponse.Results != null)
        {
            _stopTimes = timeResponse.Results;
        }
        else
        {
            _stopTimes = new List<StopResult>();
        }
    }
}
