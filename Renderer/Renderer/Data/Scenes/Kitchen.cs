using Renderer.Data.Objects;
using Renderer.Extensions;
using Renderer.Models;

namespace Renderer.Data.Scenes;

internal class Kitchen : BaseScene
{
    public Kitchen()
    {
        Add(new Chair().Rotate((Math.PI / 5), Axis.Y).Rotate(-(Math.PI / 16), Axis.X), 180, 100, 0);
        Add(new Chair().Rotate(-(Math.PI / 5), Axis.Y).Rotate(-(Math.PI / 16), Axis.X), 180, -100, 0);
    }
}
