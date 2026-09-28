using Renderer.Data.Objects;
using Renderer.Models;

namespace Renderer.Extensions;

internal static class IObjectExtensions
{
    public static IObject Rotate(this IObject o, double angle, Axis axis) => new Rotated(o, angle, axis);
}
