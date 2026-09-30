using Renderer.Data.Objects;
using Renderer.Models;

namespace Renderer.Data.Scenes;

internal class BaseScene
{
    public IEnumerable<ProjectionObject> Objects => objects;

    protected void AddThreeDimObject(IObject o, int size, int x, int y)
    {
        objects.Add(new ProjectionObject(new ThreeDimObject(o, size, x, y)));
    }

    protected void AddTwoDimObject(Polygon2d polygon2)
    {
        objects.Add(new ProjectionObject(new TwoDimObject(polygon2)));
    }

    private readonly List<ProjectionObject> objects = [];
}
