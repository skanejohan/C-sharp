using Renderer.Data.Objects;
using Renderer.Extensions;
using Renderer.Models;

namespace Renderer.Data.Scenes;

internal class Kitchen : BaseScene
{
    public Kitchen()
    {
        AddTwoDimObject(new Polygon2d([new Coord2d(10, 10), new Coord2d(630, 10), new Coord2d(630, 390), new Coord2d(10, 390)], new("black", 0, "white"), "back_wall"));
        AddThreeDimObject(new Chair().Rotate((Math.PI / 5), Axis.Y).Rotate(-(Math.PI / 16), Axis.X), 180, 100, 0);
        AddThreeDimObject(new Chair().Rotate(-(Math.PI / 5), Axis.Y).Rotate(-(Math.PI / 16), Axis.X), 180, -100, 0);
    }
}
