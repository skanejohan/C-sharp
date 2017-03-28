namespace Ads.Game.Internal
{
    using System;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework;

    internal sealed class State : IState
    {
        private int state;
        private Engine engine;

        internal State(Engine engine, int state)
        {
            this.state = state;
            this.engine = engine;
        }

        public void RegisterConditionalEvent(IConditionalEvent conditionalEvent)
        {
            engine.RegisterConditionalEvent(state, conditionalEvent);
        }

        public void RegisterCollisionEvent(ICollidable obj1, ICollidable obj2, Action<ICollidable, ICollidable> onCollision)
        {
            engine.RegisterCollisionEvent(state, obj1, obj2, onCollision);
        }

        public void RegisterKeyDownEvent(Keys key, Action perform)
        {
            engine.RegisterKeyDownEvent(state, key, perform);
        }

        public void RegisterTimedEvent(int intervalMS, Action action)
        {
            engine.RegisterTimedEvent(state, intervalMS, action);
        }

        public void RegisterSingleTimedEvent(int intervalMS, Action action)
        {
            engine.RegisterSingleTimedEvent(state, intervalMS, action);
        }

        public IResourceEntity AddEntity(IResourceEntity entity)
        {
            return engine.AddEntity(state, entity);
        }

        public IEntity AddText(int x, int y, string font, Func<string> getText)
        {
            return AddText(x, y, font, Color.Black, getText);
        }

        public IEntity AddText(int x, int y, string font, Color color, Func<string> getText)
        {
            return engine.AddText(state, x, y, font, color, getText);
        }

        public IEntity AddCenteredText(int y, string font, Func<string> getText)
        {
            return AddCenteredText(y, font, Color.Black, getText);
        }

        public IEntity AddCenteredText(int y, string font, Color color, Func<string> getText)
        {
            return engine.AddCenteredText(state, y, font, color, getText);
        }

        public void AddSong(string resource)
        {
            engine.AddSong(state, resource);
        }

        public void RegisterEntry(Action action)
        {
            engine.RegisterEntry(state, action);
        }

        public void RegisterEntryFrom(int fromState, Action action)
        {
            engine.RegisterEntryFrom(fromState, state, action);
        }

        public void RegisterExit(Action action)
        {
            engine.RegisterExit(state, action);
        }

        public void RegisterExitTo(int toState, Action action)
        {
            engine.RegisterExitTo(state, toState, action);
        }

    }
}
