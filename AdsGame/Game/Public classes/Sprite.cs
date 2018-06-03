namespace Ads.Game
{
    using Ads.Game.Internal;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Diagnostics;

    public class Sprite : Entity, IVisibleEntity, IPositionable, IMoveable, ICollidable
    {
        public Texture2D Texture { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Scale { get; set; }
        public Color Color { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public IMovementControl MovementControl { get; set; }

        public override bool Paused
        {
            get
            {
                return base.Paused;
            }
            set
            {
                base.Paused = value;
                MovementControl.DownAction.Paused = value;
                MovementControl.LeftAction.Paused = value;
                MovementControl.RightAction.Paused = value;
                MovementControl.UpAction.Paused = value;
                MovementControl.MouseMovementAction.Paused = value;
                Debug.WriteLine("MovementControl: Paused="+value);
            }
        }

        protected SpriteEffects spriteEffects;

        public Sprite(IEngine engine, SpriteEffects spriteEffects = SpriteEffects.None) : base(engine)
        {
            this.MovementControl = new VoidMovementControl();
            Color = Color.White;
            Alive = true;
            Scale = 1f;
            this.spriteEffects = spriteEffects;
        }

        public virtual void Initialize()
        {
        }

        public virtual void LoadResources()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Alive)
            {
                spriteBatch.Draw(Texture, new Vector2((int)(X - Scale * Width / 2), (int)(Y - Scale * Height / 2)), 
                    null, Color, 0f, Vector2.Zero, Scale, spriteEffects, 0f);
            }
        }

        public virtual Rectangle Rectangle()
        {
            return new Rectangle(
                (int)(X - Scale * Width / 2), 
                (int)(Y - Scale * Height / 2), 
                (int)(Scale * Width), 
                (int)(Scale * Height));
        }

    }
}
