using Renderer.Models;

namespace Renderer.Data.Objects;

public class Rotated(IObject o, double angle, Axis axis) : IObject
{
    public IEnumerable<Polygon3d> Polygon3ds => polygon3ds;

    private readonly List<Polygon3d> polygon3ds = [.. o.Polygon3ds.Select(p => p.Rotate(angle, axis))];
}
