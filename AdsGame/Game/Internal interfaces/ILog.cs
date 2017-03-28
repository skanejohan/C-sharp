namespace Ads.Game.Internal
{
    using System;

    public interface ILog
    {
        Action<string> Log { get; }
    }
}
