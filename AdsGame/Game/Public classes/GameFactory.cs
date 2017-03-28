namespace Ads.Game
{
    using System;
    using Ads.Game.Internal;

    public static class GameFactory
    {
        private static Engine gameEngine;

        public static IEngine GetGameEngine(Action<string> log)
        {
            if (gameEngine == null)
            {
                gameEngine = new Engine(log);
            }
            return gameEngine;
        }
    }
}
