namespace maui_app;

public class TransportIndex
{
    public int Nr { get; set; }
    public string? ImageSource { get; set; }
    public TransportType Type { get; set; }

}

public enum TransportType
{
    Bus,
    Tram,
    Metro
}
