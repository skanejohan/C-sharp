using Renderer.Data.Scenes;
using Renderer.Operations;
using Renderer.Renderers;

int w = 640;
int h = 400;

var projector = new Projector(w, h, 50);
var htmlRenderer = new HtmlRenderer(w, h);

foreach(var obj in new Kitchen().Objects)
{
    htmlRenderer.Render(projector.ProjectObject(obj));
}
File.WriteAllText(@"D:\Temp\html.html", htmlRenderer.Html);
