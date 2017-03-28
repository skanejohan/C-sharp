namespace Ads.Game.Internal
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework.Input;
    using Extensions;
    using System.Linq;

    internal sealed class Engine : Game, IEngine, IEngineInternal, ILog
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private InputManager inputManager;
        private ListDictionary<ICollidable, IConditionalEvent> collisionsForCollidable;

        private List<TimedEvent> timedEvents;
        private List<IConditionalEvent> conditionalEvents;
        private List<IResourceEntity> resourceEntities;
        private List<IResourceEntity> debugTexts;

        private ListDictionary<int, TimedEvent> stateTimedEvents;
        private ListDictionary<int, IConditionalEvent> stateConditionalEvents;
        private ListDictionary<int, IResourceEntity> stateResourceEntities;

        private Dictionary<int, State> states;
        private Dictionary<string, Action> stateEntries;
        private Dictionary<string, Action> stateExits;

        private bool contentHasBeenLoaded;
        public Action<string> Log { get; }

        public Color BackgroundColor { get; set; }

        public int CurrentState { get { return currentState; } set { setCurrentState(value); } }

        public bool DebugMode { get; set; }

        public Action<IEngine> OnContentLoading { get; set; }

        public Engine(Action<string> log)
        {
            Log = log;
            graphics = new GraphicsDeviceManager(this);
            conditionalEvents = new List<IConditionalEvent>();
            timedEvents = new List<TimedEvent>();
            resourceEntities = new List<IResourceEntity>();
            debugTexts = new List<IResourceEntity>();
            Content.RootDirectory = "Content";
            inputManager = new InputManager();
            collisionsForCollidable = new ListDictionary<ICollidable, IConditionalEvent>();
            stateTimedEvents = new ListDictionary<int, TimedEvent>();
            stateConditionalEvents = new ListDictionary<int, IConditionalEvent>();
            stateResourceEntities = new ListDictionary<int, IResourceEntity>();
            states = new Dictionary<int, State>();
            stateEntries = new Dictionary<string, Action>();
            stateExits = new Dictionary<string, Action>();
            BackgroundColor = Color.Black;
        }

        public void SetFullScreen()
        {
            graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
        }

        public void SetWindowed(int width, int height)
        {
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
        }

        public void RegisterDebugOutput(int x, int y, string font)
        {
            DoAddText(debugTexts, x, y, font, Color.Black, () => $"Entities: {allResourceEntities.ToList().Count}");
            DoAddText(debugTexts, x, y + 30, font, Color.Black, () => $"Timed events: {allTimedEvents.ToList().Count}");
            DoAddText(debugTexts, x, y + 60, font, Color.Black, () => $"Conditional events: {allConditionalEvents.ToList().Count}");
        }

        #region States
        private int currentState;
        private void setCurrentState(int value)
        {
            Action action;

            var fromString = currentState.ToString("D16");
            var toString = value.ToString("D16");
            var fromToString = fromString + toString;

            if (stateExits.TryGetValue(fromString, out action))
            {
                action();
            }
            if (stateExits.TryGetValue(fromToString, out action))
            {
                action();
            }
            foreach (var te in stateTimedEvents.GetList(currentState))
            {
                te.OutOfState = true;
            }

            currentState = value;

            if (stateEntries.TryGetValue(fromToString, out action))
            {
                action();
            }
            if (stateEntries.TryGetValue(toString, out action))
            {
                action();
            }
            foreach (var te in stateTimedEvents.GetList(currentState))
            {
                te.OutOfState = false;
            }
        }

        public IState State(int stateIndex)
        {
            State state;
            if (!states.TryGetValue(stateIndex, out state))
            {
                state = new State(this, stateIndex);
                states.Add(stateIndex, state);
            }
            return state;
        }

        internal void RegisterEntry(int state, Action action)
        {
            stateEntries[state.ToString("D16")] = action;
        }

        internal void RegisterEntryFrom(int fromState, int toState, Action action)
        {
            stateEntries[fromState.ToString("D16") + toState.ToString("D16")] = action;
        }

        internal void RegisterExit(int state, Action action)
        {
            stateExits[state.ToString("D16")] = action;
        }

        internal void RegisterExitTo(int fromState, int toState, Action action)
        {
            stateExits[fromState.ToString("D16") + toState.ToString("D16")] = action;
        }
        #endregion

        #region IInputManager
        public bool Left() => inputManager.Left();
        public bool Right() => inputManager.Right();
        public bool Up() => inputManager.Up();
        public bool Down() => inputManager.Down();
        public bool KeyIsDown(Keys k) => inputManager.KeyIsDown(k);
        public bool KeyDown(Keys k) => inputManager.KeyDown(k);
        public bool KeyUp(Keys k) => inputManager.KeyUp(k);
        public Vector2 MouseDeltaPosition() => inputManager.MouseDeltaPosition();
        #endregion

        #region IEntities - conditional events (incl. internal functions)
        public void RegisterConditionalEvent(IConditionalEvent conditionalEvent)
        {
            DoRegisterConditionalEvent(conditionalEvents, conditionalEvent);
        }

        public void RegisterCollisionEvent(ICollidable obj1, ICollidable obj2, Action<ICollidable, ICollidable> onCollision)
        {
            DoRegisterCollisionEvent(conditionalEvents, obj1, obj2, onCollision);
        }

        public void RegisterKeyDownEvent(Keys key, Action perform)
        {
            DoRegisterKeyDownEvent(conditionalEvents, key, perform);
        }

        internal void RegisterConditionalEvent(int state, IConditionalEvent conditionalEvent)
        {
            DoRegisterConditionalEvent(stateConditionalEvents.GetList(state), conditionalEvent);
        }

        internal void RegisterCollisionEvent(int state, ICollidable obj1, ICollidable obj2, Action<ICollidable, ICollidable> onCollision)
        {
            DoRegisterCollisionEvent(stateConditionalEvents.GetList(state), obj1, obj2, onCollision);
        }

        internal void RegisterKeyDownEvent(int state, Keys key, Action perform)
        {
            DoRegisterKeyDownEvent(stateConditionalEvents.GetList(state), key, perform);
        }

        private void DoRegisterConditionalEvent(List<IConditionalEvent> list, IConditionalEvent conditionalEvent)
        {
            list.Add(conditionalEvent);
        }

        private void DoRegisterCollisionEvent(List<IConditionalEvent> list, ICollidable obj1, ICollidable obj2, Action<ICollidable, ICollidable> onCollision)
        {
            var coll = new ConditionalEvent(this, () => obj1.Rectangle().Intersects(obj2.Rectangle()), () => onCollision(obj1, obj2));
            collisionsForCollidable.Add(obj1, coll);
            collisionsForCollidable.Add(obj2, coll);
            list.Add(coll);
        }

        private void DoRegisterKeyDownEvent(List<IConditionalEvent> list, Keys key, Action perform)
        {
            DoRegisterConditionalEvent(list, new ConditionalEvent(this, () => KeyDown(key), perform));
        }

        private IEnumerable<IConditionalEvent> activeConditionalEvents => conditionalEvents.Concat(stateConditionalEvents.GetList(CurrentState));

        private IEnumerable<IConditionalEvent> allConditionalEvents => conditionalEvents.Concat(stateConditionalEvents.GetAllValues());
        #endregion

        #region IEntities - timed events (incl. internal functions)
        public void RegisterTimedEvent(int intervalMS, Action action)
        {
            DoRegisterTimedEvent(timedEvents, intervalMS, action, false);
        }

        public void RegisterSingleTimedEvent(int intervalMS, Action action)
        {
            DoRegisterTimedEvent(timedEvents, intervalMS, action, true);
        }

        internal void RegisterTimedEvent(int state, int intervalMS, Action action)
        {
            DoRegisterTimedEvent(stateTimedEvents.GetList(state), intervalMS, action, false);
        }

        internal void RegisterSingleTimedEvent(int state, int intervalMS, Action action)
        {
            DoRegisterTimedEvent(stateTimedEvents.GetList(state), intervalMS, action, true);
        }

        private void DoRegisterTimedEvent(List<TimedEvent> list, int intervalMS, Action action, bool deleteAfterFirstInvocation)
        {
            list.Add(new TimedEvent(this, intervalMS, action, deleteAfterFirstInvocation));
        }

        private IEnumerable<TimedEvent> activeTimedEvents => timedEvents.Concat(stateTimedEvents.GetList(CurrentState));

        private IEnumerable<TimedEvent> allTimedEvents => timedEvents.Concat(stateTimedEvents.GetAllValues());
        #endregion

        #region IEntities - entities (incl. internal functions)
        public IResourceEntity AddEntity(IResourceEntity entity)
        {
            return DoAddEntity(resourceEntities, entity);
        }

        public IEntity AddText(int x, int y, string font, Func<string> getText)
        {
            return AddText(x, y, font, Color.Black, getText);
        }

        public IEntity AddText(int x, int y, string font, Color color, Func<string> getText)
        {
            return DoAddText(resourceEntities, x, y, font, color, getText);
        }

        public IEntity AddCenteredText(int y, string font, Func<string> getText)
        {
            return AddCenteredText(y, font, Color.Black, getText);
        }

        public IEntity AddCenteredText(int y, string font, Color color, Func<string> getText)
        {
            return DoAddCenteredText(resourceEntities, y, font, color, getText);
        }

        public void AddSong(string resource)
        {
            DoAddSong(resourceEntities, resource);
        }

        internal IResourceEntity AddEntity(int state, IResourceEntity entity)
        {
            return DoAddEntity(stateResourceEntities.GetList(state), entity);
        }

        internal IEntity AddText(int state, int x, int y, string font, Color color, Func<string> getText)
        {
            return DoAddText(stateResourceEntities.GetList(state), x, y, font, color, getText);
        }

        internal IEntity AddCenteredText(int state, int y, string font, Color color, Func<string> getText)
        {
            return DoAddCenteredText(stateResourceEntities.GetList(state), y, font, color, getText);
        }

        internal void AddSong(int state, string resource)
        {
            DoAddSong(stateResourceEntities.GetList(state), resource);
        }

        private IResourceEntity DoAddEntity(List<IResourceEntity> list, IResourceEntity entity)
        {
            if (IsActive)
            {
                entity.Initialize();
                entity.LoadResources();
            }

            list.Add(entity);

            if (entity is IMoveable)
            {
                RegisterConditionalEvent((entity as IMoveable).MovementControl.LeftAction);
                RegisterConditionalEvent((entity as IMoveable).MovementControl.RightAction);
                RegisterConditionalEvent((entity as IMoveable).MovementControl.UpAction);
                RegisterConditionalEvent((entity as IMoveable).MovementControl.DownAction);
                RegisterConditionalEvent((entity as IMoveable).MovementControl.MouseMovementAction);
            }

            return entity;
        }

        private IEntity DoAddText(List<IResourceEntity> list, int x, int y, string font, Color color, Func<string> getText)
        {
            var text = new Text(x, y, false, font, color, getText, this);
            list.Add(text);
            if (contentHasBeenLoaded)
            {
                text.LoadResources();
            }
            return text;
        }

        private IEntity DoAddCenteredText(List<IResourceEntity> list, int y, string font, Color color, Func<string> getText)
        {
            var text = new Text(0, y, true, font, color, getText, this);
            list.Add(text);
            if (contentHasBeenLoaded)
            {
                text.LoadResources();
            }
            return text;
        }

        public void DoAddSong(List<IResourceEntity> list, string resource)
        {
            list.Add(new Song(resource, this));
        }

        public IScene AddScene(int borderSize, Action<IEngine, IScene> loader)
        {
            return DoAddScene(resourceEntities, borderSize, loader);
        }

        internal IScene AddScene(int state, int borderSize, Action<IEngine, IScene> loader)
        {
            return DoAddScene(stateResourceEntities.GetList(state), borderSize, loader);
        }

        private IScene DoAddScene(List<IResourceEntity> list, int borderSize, Action<IEngine,IScene> loader)
        {
            var scene = new Scene(this, borderSize, loader);
            list.Add(scene);
            return scene;
        }

        private IEnumerable<IResourceEntity> activeResourceEntities => resourceEntities.Concat(stateResourceEntities.GetList(CurrentState));

        private IEnumerable<IResourceEntity> allResourceEntities => resourceEntities.Concat(stateResourceEntities.GetAllValues());
        #endregion

        #region IEngine
        public void SwitchAll()
        {
            foreach (var entity in activeResourceEntities)
            {
                entity.Paused = !entity.Paused;
            }
            foreach (var timedEvent in activeTimedEvents)
            {
                timedEvent.Paused = !timedEvent.Paused;
            }
        }

        public void PauseAll()
        {
            foreach (var entity in activeResourceEntities)
            {
                entity.Paused = true;
            }
            foreach (var timedEvent in activeTimedEvents)
            {
                timedEvent.Paused = true;
            }
        }

        public void StartAll()
        {
            foreach (var entity in activeResourceEntities)
            {
                entity.Paused = false;
            }
            foreach (var timedEvent in activeTimedEvents)
            {
                timedEvent.Paused = false;
            }
        }
        #endregion

        #region IEngineInternal
        public void EntityHasDied(IEntity entity)
        {
            if (entity is ICollidable)
            {
                List<IConditionalEvent> collisions;
                var collidableEntity = entity as ICollidable;
                if (collisionsForCollidable.TryGetValues(collidableEntity, out collisions))
                {
                    foreach (var collision in collisions)
                    {
                        collision.Alive = false;
                    }
                }
            }
            if (entity is IMoveable)
            {
                (entity as IMoveable).MovementControl.LeftAction.Alive = false;
                (entity as IMoveable).MovementControl.DownAction.Alive = false;
                (entity as IMoveable).MovementControl.RightAction.Alive = false;
                (entity as IMoveable).MovementControl.UpAction.Alive = false;
                (entity as IMoveable).MovementControl.MouseMovementAction.Alive = false;
            }
        }
        #endregion

        #region Game loop overrides
        protected override void Initialize()
        {
            foreach (var entity in allResourceEntities)
            {
                entity.Initialize();
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            OnContentLoading?.Invoke(this);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            foreach (var entity in allResourceEntities)
            {
                entity.LoadResources();
            }
            foreach (var text in debugTexts)
            {
                text.LoadResources();
            }
            contentHasBeenLoaded = true;
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            inputManager.Update();

            // We evaluate the lists here, before we start processing them. This way, we will operate 
            // on the current state through this whole update, even if the current state changes
            // somewhere in the middle, thus reducing the risk of difficult-to-debug problems.
            var stableState = currentState;
            var stableConditionalEvents = activeConditionalEvents.ToList();
            var stableTimedEvents = activeTimedEvents.ToList();
            var stableResourceEntities = activeResourceEntities.ToList();

            foreach (var conditionalEvent in stableConditionalEvents)
            {
                if (conditionalEvent.Applies() && conditionalEvent.Alive)
                {
                    conditionalEvent.Perform();
                }
            }

            foreach (var timedEvent in stableTimedEvents)
            {
                if (timedEvent.Alive)
                {
                    timedEvent.Update(gameTime);
                }
            }

            foreach (var entity in stableResourceEntities)
            {
                if (!entity.Paused)
                {
                    entity.Update(gameTime);
                }
            }

            base.Update(gameTime);

            // Check among stuff that was active when we entered this method. If anything is no longer alive, remove it.
            // Note that we can't use foreach here, since we modify the lists.
            var ceList = conditionalEvents;
            for (var i = ceList.Count - 1; i >= 0; i--)
            {
                if (!ceList[i].Alive)
                {
                    ceList.RemoveAt(i);
                }
            }

            ceList = stateConditionalEvents.GetList(stableState);
            for (var i = ceList.Count - 1; i >= 0; i--)
            {
                if (!ceList[i].Alive)
                {
                    ceList.RemoveAt(i);
                }
            }

            var teList = timedEvents;
            for (var i = teList.Count - 1; i >= 0; i--)
            {
                if (!teList[i].Alive)
                {
                    teList.RemoveAt(i);
                }
            }

            teList = stateTimedEvents.GetList(stableState);
            for (var i = teList.Count - 1; i >= 0; i--)
            {
                if (!teList[i].Alive)
                {
                    teList.RemoveAt(i);
                }
            }

            var reList = resourceEntities;
            for (var i = reList.Count - 1; i >= 0; i--)
            {
                if (!reList[i].Alive)
                {
                    reList.RemoveAt(i);
                }
            }

            reList = stateResourceEntities.GetList(stableState);
            for (var i = reList.Count - 1; i >= 0; i--)
            {
                if (!reList[i].Alive)
                {
                    reList.RemoveAt(i);
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(BackgroundColor);

            spriteBatch.Begin();

            foreach (var entity in activeResourceEntities)
            {
                if (entity is IVisibleEntity)
                {
                    (entity as IVisibleEntity).Draw(spriteBatch);

                    if (DebugMode && entity is ICollidable)
                    {
                        (entity as ICollidable).DrawRectangle(GraphicsDevice, spriteBatch);
                    }
                }
            }
            
            if (DebugMode)
            {
                foreach (var text in debugTexts)
                {
                    (text as IVisibleEntity).Draw(spriteBatch);
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion

    }
}