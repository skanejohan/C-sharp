namespace Renderer.Data.Objects;

internal class Helper(int x, int y)
{
    public Cuboid CreateCuboid(double x1, double x2, double y1, double y2, double z1, double z2, string id) => new (x1 + x, x2 + x, y1 + y, y2 + y, z1, z2, id);
}
