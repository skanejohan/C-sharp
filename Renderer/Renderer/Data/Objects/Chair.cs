using Renderer.Models;

namespace Renderer.Data.Objects;

internal class Chair : IObject
{
    public IEnumerable<Polygon3d> Polygon3ds => polygon3ds;

    public Chair(int xFromCamera = 0, int yFromCamera = 0)
    {
        var helper = new Helper(xFromCamera, yFromCamera);
        var drawInfo1 = new DrawInfo("black", 1, "yellow");
        var drawInfo2 = new DrawInfo("black", 1, "green");
        polygon3ds.AddRange(helper.CreateCuboid(0, 1, 0, 5, 0, 1, drawInfo1, "leg1").Polygon3ds);
        polygon3ds.AddRange(helper.CreateCuboid(4, 5, 0, 5, 0, 1, drawInfo1, "leg2").Polygon3ds);
        polygon3ds.AddRange(helper.CreateCuboid(0, 1, 0, 10, 4, 5, drawInfo1, "leg3").Polygon3ds);
        polygon3ds.AddRange(helper.CreateCuboid(4, 5, 0, 10, 4, 5, drawInfo1, "leg4").Polygon3ds);
        polygon3ds.AddRange(helper.CreateCuboid(1, 4, 4.1f, 4.9f, 0, 5, drawInfo2, "seat").Polygon3ds);
        polygon3ds.AddRange(helper.CreateCuboid(1, 4, 7, 7.8f, 4.2f, 5, drawInfo1, "back1").Polygon3ds.Where(p => p.Id != "back1_x1" && p.Id != "back1_x2"));
        polygon3ds.AddRange(helper.CreateCuboid(1, 4, 9, 9.8f, 4.2f, 5, drawInfo1, "back2").Polygon3ds.Where(p => p.Id != "back2_x1" && p.Id != "back2_x2"));
    }

    private List<Polygon3d> polygon3ds = [];
}
