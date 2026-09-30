using Renderer.Data.Scenes;
using Renderer.Models;
using Renderer.Operations;
using Renderer.Renderers;

int w = 640;
int h = 400;

var projector = new Projector(w, h, 50);
var renderer = new BitmapRenderer(w, h);
var fileName = @"D:\Temp\imagesharp.bmp";
//var renderer = new HtmlRenderer(w, h);
//var fileName = @"D:\Temp\html.html";

foreach (var obj in new Kitchen().Objects)
{
    IEnumerable<Polygon2d> polygons = [];
    if (obj.TryGetThreeDimObject(out var obj3))
    {
        polygons = projector.ProjectObject(obj3!);
    }
    else if (obj.TryGetTwoDimObject(out var obj2))
    {
        polygons = [obj2!.Polygon2d];
    }
    else
    {
        continue;
    }
    renderer.Render(polygons);
}
renderer.Save(fileName);
