namespace Ads.Game
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public interface IEntities
    {
        // Conditional events
        void RegisterConditionalEvent(IConditionalEvent conditionalEvent);
        void RegisterCollisionEvent(ICollidable obj1, ICollidable obj2, Action<ICollidable, ICollidable> onCollision);
        void RegisterKeyDownEvent(Keys key, Action perform);

        // Timed events
        void RegisterTimedEvent(int intervalMS, Action action);
        void RegisterSingleTimedEvent(int intervalMS, Action action);

        // Entities
        IResourceEntity AddEntity(IResourceEntity entity);
        IEntity AddText(int x, int y, string font, Func<string> getText);
        IEntity AddText(int x, int y, string font, Color color, Func<string> getText);
        IEntity AddCenteredText(int y, string font, Func<string> getText);
        IEntity AddCenteredText(int y, string font, Color color, Func<string> getText);
        void AddSong(string resource);

    }
}
