namespace maui_app;

public class SquareBehavior : Behavior<VisualElement>
{
    protected override void OnAttachedTo(VisualElement bindable)
    {
        bindable.SizeChanged += OnSizeChanged;
        base.OnAttachedTo(bindable);
    }

    protected override void OnDetachingFrom(VisualElement bindable)
    {
        bindable.SizeChanged -= OnSizeChanged;
        base.OnDetachingFrom(bindable);
    }

    private void OnSizeChanged(object? sender, EventArgs e)
    {
        if (sender is VisualElement element && element.Width > 0)
        {
            element.HeightRequest = element.Width;
            element.Dispatcher.Dispatch(() => element.InvalidateMeasure());
        }
    }
}