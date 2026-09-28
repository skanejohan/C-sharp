namespace Renderer.Models;

public record Coord3d(double X, double Y, double Z)
{
    public Coord3d Rotate(double angle, Axis axis)
    {
        var s = Math.Sin(angle);
        var c = Math.Cos(angle);
        var m = axis switch
        {
            Axis.X => RotationMatrixAroundX(s, c),
            Axis.Y => RotationMatrixAroundY(s, c),
            _ => RotationMatrixAroundZ(s, c)
        };
        var x = m[0].X * X + m[0].Y * Y + m[0].Z * Z;
        var y = m[1].X * X + m[1].Y * Y + m[1].Z * Z;
        var z = m[2].X * X + m[2].Y * Y + m[2].Z * Z;
        return new (x, y, z);
    }

    private static List<Coord3d> RotationMatrixAroundX(double sin, double cos) => [new(1, 0, 0), new(0, cos, -sin), new(0, sin, cos)];
    private static List<Coord3d> RotationMatrixAroundY(double sin, double cos) => [new(cos, 0, sin), new(0, 1, 0), new(-sin, 0, cos)];
    private static List<Coord3d> RotationMatrixAroundZ(double sin, double cos) => [new(cos, -sin, 0), new(sin, cos, 0), new(0, 0, 1)];

}

