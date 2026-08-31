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
}

public class PassingTime
{
    [JsonPropertyName("destination")]
    public Destination Destination { get; set; } = new();

    [JsonPropertyName("expectedArrivalTime")]
    public DateTimeOffset ExpectedArrivalTime { get; set; }

    [JsonPropertyName("lineId")]
    public string LineId { get; set; } = "";
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