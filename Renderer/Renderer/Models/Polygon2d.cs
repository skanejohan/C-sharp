using System.Drawing;

namespace Renderer.Models;

public record Polygon2d(Coord2d[] Coord2Ds, string Id)
{
    public Point[] Points => [.. Coord2Ds.Select(c => new Point((int)c.X, (int)c.Y))];
    public override string ToString() => $"{Id} : {string.Join(", ", Coord2Ds)}";
    public Polygon2d Move(double dx, double dy) => new([.. Coord2Ds.Select(c => c.Move(dx, dy))], Id);
    public Polygon2d Scale(double f) => new([.. Coord2Ds.Select(c => c.Scale(f))], Id);
};
