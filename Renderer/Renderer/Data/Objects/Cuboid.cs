using Renderer.Models;

namespace Renderer.Data.Objects;

internal class Cuboid : IObject
{
    public IEnumerable<Polygon3d> Polygon3ds => polygon3ds;

    public Cuboid(double x1, double x2, double y1, double y2, double z1, double z2, string id)
    {
        polygon3ds.Add(new ([new (x1, y1, z1), new (x2, y1, z1), new (x2, y2, z1), new (x1, y2, z1)], $"{id}_z1"));
        polygon3ds.Add(new ([new (x1, y1, z2), new (x2, y1, z2), new (x2, y2, z2), new (x1, y2, z2)], $"{id}_z2"));
        polygon3ds.Add(new ([new (x1, y1, z1), new (x2, y1, z1), new (x2, y1, z2), new (x1, y1, z2)], $"{id}_y1"));
        polygon3ds.Add(new ([new (x1, y2, z1), new (x2, y2, z1), new (x2, y2, z2), new (x1, y2, z2)], $"{id}_y2"));
        polygon3ds.Add(new ([new (x1, y1, z1), new (x1, y2, z1), new (x1, y2, z2), new (x1, y1, z2)], $"{id}_x1"));
        polygon3ds.Add(new ([new (x2, y1, z1), new (x2, y2, z1), new (x2, y2, z2), new (x2, y1, z2)], $"{id}_x2"));
    }

    private List<Polygon3d> polygon3ds = [];
}
