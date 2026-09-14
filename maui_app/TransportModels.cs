using System.Text.Json.Serialization;

namespace maui_app;

public class WaitingTimeResponse
{
    [JsonPropertyName("results")]
    public List<StopResult> Results { get; set; } = new();

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }
}

public class StopResult
{
    [JsonPropertyName("pointid")]
    public string PointId { get; set; } = "";

    [JsonPropertyName("lineid")]
    public string LineId { get; set; } = "";

    [JsonPropertyName("passingtimes")]
    public string PassingTimesRaw { get; set; } = "";

    public List<DateTimeOffset> ExpectedArrivalTime { get; set; } = new();

    public string StopName { get; set; } = "";
}

public class PassingTimeRaw
{
    [JsonPropertyName("expectedArrivalTime")]
    public DateTimeOffset ExpectedArrivalTime { get; set; }
}

public class Destination
{
    [JsonPropertyName("fr")]
    public string Fr { get; set; } = "";

    [JsonPropertyName("nl")]
    public string Nl { get; set; } = "";
}

public class StopDetailsResponse
{
    [JsonPropertyName("results")]
    public List<StopDetail> Results { get; set; } = new();
}

public class StopDetail
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("name")]
    public string NameRaw { get; set; } = "";

    [JsonPropertyName("gpscoordinates")]
    public string GpsRaw { get; set; } = "";
}

public class StopName
{
    [JsonPropertyName("fr")]
    public string Fr { get; set; } = "";

    [JsonPropertyName("nl")]
    public string Nl { get; set; } = "";
}

public class StopsByLineResponse
{
    [JsonPropertyName("results")]
    public List<StopsByLineResult> Results { get; set; } = new();
}

public class StopsByLineResult
{
    [JsonPropertyName("lineid")]
    public string LineId { get; set; } = "";

    [JsonPropertyName("direction")]
    public string Direction { get; set; } = "";

    [JsonPropertyName("destination")]
    public string DestinationRaw { get; set; } = "";

    [JsonPropertyName("points")]
    public string PointsRaw { get; set; } = "";
}

public class RoutePoint
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("order")]
    public int Order { get; set; }
}

public class LineRoute
{
    public string Direction { get; set; } = "";
    public string Destination { get; set; } = "";
    public List<RouteStop> Stops { get; set; } = new();
}

public class RouteStop
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public int Order { get; set; }
    public bool IsTerminus { get; set; }
    public List<DateTimeOffset> Times { get; set; } = new();
    public string Time1 { get; set; } = "";
    public string Time2 { get; set; } = "";
}