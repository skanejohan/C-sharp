namespace Renderer.Models;

public record Coord2d(double X, double Y)
{
    public Coord2d Move(double dx, double dy) => new(X + dx, Y + dy);
    public Coord2d Scale(double f) => new(X  * f, Y * f);
}
