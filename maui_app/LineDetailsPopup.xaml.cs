namespace maui_app;

public partial class LineDetailsPopup : ContentView
{
    public event EventHandler? CloseRequested;

    public LineDetailsPopup(TransportIndex line)
    {
        InitializeComponent();

        TitleLabel.Text = $"Ligne {line.Nr}";
        DetailsLabel.Text = $"Type : {line.Type}";
    }

    private void OnCloseClicked(object sender, EventArgs e) => CloseRequested?.Invoke(this, EventArgs.Empty);
}
