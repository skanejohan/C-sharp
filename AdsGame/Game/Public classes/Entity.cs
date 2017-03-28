namespace Ads.Game
{
    using Ads.Game.Internal;

    public class Entity : IEntity
    {
        public virtual bool Alive
        {
            get
            {
                return alive;
            }
            set
            {
                alive = value;
                if (!alive && (this.Engine != null))
                {
                    (Engine as IEngineInternal).EntityHasDied(this);
                }
            }
        }

        public virtual bool Paused { get; set; }

        public Entity(IEngine engine)
        {
            this.Engine = engine;
            Alive = true;
        }

        protected IEngine Engine;
        private bool alive;
    }
}
