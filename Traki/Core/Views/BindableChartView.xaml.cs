using Core.Entity;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System.Collections.ObjectModel;

namespace Core.Views;

public partial class BindableChartView : ContentView
{
    public static readonly BindableProperty EntriesProperty =
        BindableProperty.Create(
            nameof(Entries),
            typeof(IEnumerable<ChartEntryWrapper>),
            typeof(BindableChartView),
            propertyChanged: OnEntriesChanged);

    public IEnumerable<ChartEntryWrapper> Entries
    {
        get => (IEnumerable<ChartEntryWrapper>)GetValue(EntriesProperty);
        set => SetValue(EntriesProperty, value);
    }

    public BindableChartView()
    {
        InitializeComponent();
    }

    private static void OnEntriesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BindableChartView view)
        {
            view.ChartCanvas.InvalidateSurface(); // Redraw
        }
    }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);

        if (Entries == null || !Entries.Any()) return;

        float centerX = e.Info.Width / 2f;
        float centerY = e.Info.Height / 2f;
        float radius = Math.Min(centerX, centerY) - 10;
        float startAngle = -90f;

        float total = Entries.Sum(entry => entry.Value);

        foreach (var entry in Entries)
        {
            float sweepAngle = (entry.Value / total) * 360f;

            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 40,
                Color = entry.Color,
                IsAntialias = true
            };

            var rect = new SKRect(centerX - radius, centerY - radius, centerX + radius, centerY + radius);
            canvas.DrawArc(rect, startAngle, sweepAngle, false, paint);

            startAngle += sweepAngle;
        }
    }

}
