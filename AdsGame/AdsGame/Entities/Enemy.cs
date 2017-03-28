namespace AdsGame.Entities
{
    using System;
    using Ads.Game;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Enemy : AnimatedSprite
    {
        public int Health { get; set; }
        public int Damage { get; set; }
        public int Value { get; set; }

        private readonly float moveSpeed = 6.0f;
        private static readonly Random random = new Random();

        public Enemy(IEngine engine) : base(engine, GraphicsMode.horizontalStrip)
        {
        }

        public override void Initialize()
        {
            Health = 10;
            Damage = 10;
            Value = 100;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            X -= moveSpeed;
            if (X < -Width || Health <= 0)
            {
                Alive = false;
            }
            base.Update(gameTime);
        }

        public override void LoadResources()
        {
            Texture = Engine.Content.Load<Texture2D>("Graphics\\mineAnimation");
            Width = 47;
            Height = 61;
            FrameCount = 8;
            FrameTime = 30;
            X = Engine.GraphicsDevice.Viewport.Width + Width / 2;
            Y = random.Next(100, Engine.GraphicsDevice.Viewport.Height -100);
        }
    }
}
