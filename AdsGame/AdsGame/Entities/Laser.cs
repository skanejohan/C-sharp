namespace AdsGame.Entities
{
    using Ads.Game;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Laser : Sprite
    {
        private readonly float moveSpeed = 30.0f;

        public Laser(float x, float y, IEngine engine) : base(engine)
        {
            X = x;
            Y = y;
        }

        public override void Update(GameTime gameTime)
        {
            X += moveSpeed;
            if (X > Engine.GraphicsDevice.Viewport.Width)
            {
                Alive = false;
            }
            base.Update(gameTime);
        }

        public override void LoadResources()
        {
            Texture = Engine.Content.Load<Texture2D>("Graphics\\laser");
            Width = 46;
            Height = 16;
        }

    }
}
