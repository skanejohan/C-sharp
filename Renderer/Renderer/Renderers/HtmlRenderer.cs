using Renderer.Models;
using System.Text;

namespace Renderer.Renderers;

internal class HtmlRenderer(int width, int height)
{
    public string Html => HtmlTemplate
        .Replace("<WIDTH>", $"{width}")
        .Replace("<HEIGHT>", $"{height}")
        .Replace("<CODE>", stringBuilder.ToString());

    public void Render(IEnumerable<Polygon2d> polygons)
    {
        foreach(var p in polygons)
        {
            Render(p);
        }
    }

    private void Render(Polygon2d polygon)
    {
        var x0 = polygon.Points[0].X;
        var y0 = polygon.Points[0].Y;
        var points = string.Join(',', polygon.Points.Skip(1).Select(p => $"[{p.X},{p.Y}]"));
        stringBuilder.Append($"ctx.fillStyle = 'yellow';ctx.strokeStyle = 'black';ctx.lineWidth = 1;ctx.beginPath();ctx.moveTo({x0},{y0});");
        stringBuilder.Append($"for (var p of [{points}]){{");
        stringBuilder.Append($"ctx.lineTo(p[0],p[1])");
        stringBuilder.Append($"}}ctx.lineTo({x0},{y0});ctx.fill();ctx.stroke();");
    }

    private StringBuilder stringBuilder = new();

    private const string HtmlTemplate = """
<!DOCTYPE html>
<html>
    <head>
    </head>
    <body>
        <canvas id="canvas" width="<WIDTH>" height="<HEIGHT>"></canvas>
        <script>
            const canvas = document.getElementById("canvas");
            const ctx = canvas.getContext("2d");
            <CODE>
        </script>
    </body>
</html>
""";
}
