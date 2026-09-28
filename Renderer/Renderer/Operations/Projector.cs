using Renderer.Models;

namespace Renderer.Operations;

internal class Projector(int width, int height, int cameraDistance)
{
    public IEnumerable<Polygon2d> ProjectObject(ProjectionObject o)
    {
        var sortedPolygons = o.Object.Polygon3ds.OrderBy(p => -p.Coord3Ds.Select(c => c.Z).Max());
        var polygons = sortedPolygons.Select(ProjectPolygon).ToArray();
        var xs = polygons.Select(p => p.Coord2Ds.Select(c => c.X)).SelectMany(x => x);
        var ys = polygons.Select(p => p.Coord2Ds.Select(c => c.Y)).SelectMany(y => y);
        var minX = xs.Min();
        var maxX = xs.Max();
        var minY = ys.Min();
        var maxY = ys.Max();
        var factor = o.Size / Math.Max(maxX - minX, maxY - minY);
        polygons = polygons
            .Select(p => p.Move(-minX, -minY))
            .Select(p => p.Scale(factor))
            .Select(p => p.Move(width / 2 - (maxX - minX) / 2 + o.X, height / 2 - (maxY - minY) / 2 + o.Y))
            .ToArray();
        return polygons;
    }

    private Coord2d ProjectCoordinate(Coord3d c) => new(c.X * cameraDistance / (cameraDistance + c.Z), -c.Y * cameraDistance / (cameraDistance + c.Z));
    private Polygon2d ProjectPolygon(Polygon3d p) => new([.. p.Coord3Ds.Select(ProjectCoordinate)], p.Id);
}
