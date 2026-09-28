using Renderer.Data.Objects;
using Renderer.Models;

namespace Renderer.Data.Scenes;

internal class BaseScene
{
    public IEnumerable<ProjectionObject> Objects => objects;

    protected void Add(IObject o, int size, int x, int y)
    {
        objects.Add(new ProjectionObject(o, size, x, y));
    }

    private readonly List<ProjectionObject> objects = [];
}
