using Renderer.Models;

namespace Renderer.Data.Objects;

public interface IObject
{
    IEnumerable<Polygon3d> Polygon3ds { get; }
}
