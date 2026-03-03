using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia;
using System;
using System.Globalization;

namespace HaveItMain.Converters;

public class ProgressToArcConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!(value is double progress))
            return null;

        double size = 120;      // same as Grid width/height
        double thickness = 8;
        double radius = parameter != null ? System.Convert.ToDouble(parameter) : (size - thickness)/2;
        double center = size / 2;

        double angle = progress * 360;
        double radians = (Math.PI / 180) * (angle - 90); // start from top

        double x = center + radius * Math.Cos(radians);
        double y = center + radius * Math.Sin(radians);
        bool isLargeArc = angle > 180;

        var geometry = new StreamGeometry();

        using (var ctx = geometry.Open())
        {
            // Use Point instead of Vector
            ctx.BeginFigure(new Point(center, center - radius), false);
            ctx.ArcTo(
                new Point(x, y),
                new Size(radius, radius),
                0,
                isLargeArc,
                SweepDirection.Clockwise);
        }

        return geometry;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}