using System.Collections.ObjectModel;
using System.Text.Json;

namespace maui_app;

public class LinesRepertory
{
    public ObservableCollection<TransportIndex> FavoritesLines { get; set; } = new();
    private readonly string cheminFichier = Path.Combine(FileSystem.AppDataDirectory, "favoris.json");
    
    private static readonly int[] LineNumbers =
    {
        1, 2, 4, 5, 6, 7, 8, 9, 10, 12,
        13, 14, 17, 18, 19, 20, 21, 25,
        28, 29, 34, 35, 36, 37, 38, 39,
        41, 42, 43, 44, 45, 46, 47, 48,
        49, 50, 51, 53, 54, 55, 56, 58,
        59, 60, 61, 62, 63, 64, 65, 66,
        69, 71, 72, 73, 74, 75, 76, 77,
        78, 79, 80, 81, 82, 83, 86, 87,
        88, 89, 92, 93, 95, 96
    };
    
    private static readonly int[] TramNumbers =
    {
        4, 7, 8, 9, 10, 18, 19, 25, 35,
        39, 44, 51, 55, 62, 81, 82, 92, 93
    };
    private static readonly int[] MetroNumbers = { 1, 2, 5, 6 };
    
    private List<TransportIndex> _allLines = LineNumbers
        .Select(nr => new TransportIndex()
        {
            Nr = nr,
            ImageSource = $"line_{nr}",
            Type = TramNumbers.Contains(nr) ? TransportType.Tram
                : MetroNumbers.Contains(nr) ? TransportType.Metro
                : TransportType.Bus,
            IsFavorite = false
        })
        .ToList();
    
    public ObservableCollection<TransportIndex> DisplayedLines { get; set; } = new();
    
    public LinesRepertory()
    {
        ChargerFavoris();
        ShowAll();
    }

    public void ShowAll() => Refresh(_allLines);
    public void ShowBus() => Refresh(_allLines.Where(l => l.Type == TransportType.Bus));
    public void ShowTram() => Refresh(_allLines.Where(l => l.Type == TransportType.Tram));
    public void ShowMetro() => Refresh(_allLines.Where(l => l.Type == TransportType.Metro));

    private void Refresh(IEnumerable<TransportIndex> lines)
    {
        DisplayedLines.Clear();
        foreach (var line in lines)
            DisplayedLines.Add(line);
    }
    
    public void SauvegarderFavoris(TransportIndex newFavorite)
    {
        FavoritesLines.Add(newFavorite);
        string json = JsonSerializer.Serialize(FavoritesLines);
        File.WriteAllText(cheminFichier, json);
    }

    public void ChargerFavoris()
    {
        if (File.Exists(cheminFichier))
        {
            string json = File.ReadAllText(cheminFichier);
            var items = JsonSerializer.Deserialize<List<TransportIndex>>(json) ?? new();
            FavoritesLines.Clear();
            foreach (var item in items)
            {
                item.IsFavorite = true;
                FavoritesLines.Add(item);
            }
        }
    }

    public void ViderFavoris()
    {
        FavoritesLines.Clear();

        foreach (var line in _allLines)
            line.IsFavorite = false;

        string json = JsonSerializer.Serialize(FavoritesLines);
        File.WriteAllText(cheminFichier, json);
    }

    public void RetirerFavori(TransportIndex line)
    {
        var favori = FavoritesLines.FirstOrDefault(f => f.Nr == line.Nr && f.Type == line.Type);
        if (favori != null)
            FavoritesLines.Remove(favori);

        var ligneRepertoriee = _allLines.FirstOrDefault(l => l.Nr == line.Nr && l.Type == line.Type);
        if (ligneRepertoriee != null)
            ligneRepertoriee.IsFavorite = false;

        string json = JsonSerializer.Serialize(FavoritesLines);
        File.WriteAllText(cheminFichier, json);
    }
}