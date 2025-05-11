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

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public BindableChartView()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        InitializeComponent();
    }

    private IEnumerable<ChartEntryWrapper> _entriesCache;

    private static void OnEntriesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BindableChartView view)
        {
            view._entriesCache = (IEnumerable<ChartEntryWrapper>)newValue; // cache it properly!
            view.ChartCanvas.InvalidateSurface();
        }
    }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);

        if (_entriesCache == null || !_entriesCache.Any()) return;

        float centerX = e.Info.Width / 2f;
        float centerY = e.Info.Height / 2f;
        float strokeWidth = 50f;
        float radius = Math.Min(centerX, centerY) - (strokeWidth / 2) - 10; // adjust radius for stroke width

        decimal startAngle = -90m;
        decimal total = _entriesCache.Sum(entry => entry.Value);

        foreach (var entry in _entriesCache)
        {
            decimal sweepAngle = (entry.Value / total) * 360m;

            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = strokeWidth,
                Color = entry.Color,
                IsAntialias = true
            };

            var rect = new SKRect(centerX - radius, centerY - radius, centerX + radius, centerY + radius);
            canvas.DrawArc(rect, (float) startAngle, (float) sweepAngle, false, paint);

            startAngle += sweepAngle;
        }
    }

}
