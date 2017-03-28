namespace Ads.Game
{
    using System;

    public interface ISceneProvider
    {
        IScene AddScene(int borderSize, Action<IEngine,IScene> loader);
    }
}
