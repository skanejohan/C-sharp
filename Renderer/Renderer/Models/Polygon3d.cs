namespace Renderer.Models;

public record Polygon3d(Coord3d[] Coord3Ds, DrawInfo DrawInfo, string Id)
{
    public override string ToString() => $"{Id} : {string.Join(", ", Coord3Ds)}";
    public Polygon3d Rotate(double angle, Axis axis)
    {
        return new([.. Coord3Ds.Select(c => c.Rotate(angle, axis))], DrawInfo, Id);
    }
};
