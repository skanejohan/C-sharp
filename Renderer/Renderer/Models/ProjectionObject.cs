using Renderer.Data.Objects;

namespace Renderer.Models;

record ThreeDimObject(IObject Object, int Size, int X, int Y);

record TwoDimObject(Polygon2d Polygon2d);

class ProjectionObject
{
    public ProjectionObject(ThreeDimObject threeDimObject)
    {
        this.threeDimObject = threeDimObject;
    }

    public ProjectionObject(TwoDimObject twoDimObject)
    {
        this.twoDimObject = twoDimObject;
    }

    public bool TryGetThreeDimObject(out ThreeDimObject? threeDimObject)
    {
        threeDimObject = this.threeDimObject;
        return threeDimObject != null;
    }

    public bool TryGetTwoDimObject(out TwoDimObject? twoDimObject)
    {
        twoDimObject = this.twoDimObject;
        return twoDimObject != null;
    }

    private readonly ThreeDimObject? threeDimObject;
    private readonly TwoDimObject? twoDimObject;
}