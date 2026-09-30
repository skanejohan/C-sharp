using Renderer.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing;

namespace Renderer.Renderers;

internal class BitmapRenderer(int width, int height)
{
    public void Render(IEnumerable<Polygon2d> polygons)
    {
        foreach (var p in polygons)
        {
            var polygon = new Polygon(p.Points.Select(p => new PointF(p.X, p.Y)).ToArray());
            image.Mutate(ctx => ctx.Paint(canvas =>
            {
                canvas.Fill(Brushes.Solid(Color.Parse(p.DrawInfo.FillStyle)), polygon);
                canvas.Draw(Pens.Solid(Color.Black, 1), polygon);
            }));
        }
    }

    public void Save(string fileName)
    {
        image.Save(fileName);
    }

    private readonly Image<Rgba32> image = new(width, height);
}
