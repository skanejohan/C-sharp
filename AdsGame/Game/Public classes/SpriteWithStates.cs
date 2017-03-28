namespace Ads.Game
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;

    public class SpriteWithStates : IVisibleEntity, IPositionable, ICollidable
    {
        public float X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
                foreach (var sprite in sprites.Values)
                {
                    sprite.X = value;
                }
            }
        }

        public float Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
                foreach (var sprite in sprites.Values)
                {
                    sprite.Y = value;
                }
            }
        }

        public int State
        {
            get
            {
                return state;
            }
            set
            {
                if (state != value)
                {
                    state = value;

                    Sprite s;
                    if (sprites.TryGetValue(State, out s))
                    {
                        (s as AnimatedSprite)?.RestartAnimation();
                    }
                }
            }
        }

        public bool Alive { get; set; }
        public bool Paused { get; set; }

        private float x;
        private float y;
        private int state;
        private Dictionary<int, Sprite> sprites;
        protected IEngine engine;

        public SpriteWithStates(IEngine engine)
        {
            Alive = true;
            this.engine = engine;
            sprites = new Dictionary<int, Sprite>();
        }

        public void Add(int state, Sprite sprite)
        {
            sprite.X = X;
            sprite.Y = Y;
            sprites.Add(state, sprite);
        }

        public void Initialize()
        {
        }

        public void Update(GameTime gameTime)
        {
            Sprite()?.Update(gameTime);
        }

        public virtual void LoadResources()
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Sprite()?.Draw(spriteBatch);
        }

        public Rectangle Rectangle()
        {
            return Sprite() != null ? Sprite().Rectangle() : Microsoft.Xna.Framework.Rectangle.Empty;
        }

        private Sprite Sprite()
        {
            Sprite s;
            if (sprites.TryGetValue(State, out s))
            {
                return s;
            }
            return null;
        }
    }
}
