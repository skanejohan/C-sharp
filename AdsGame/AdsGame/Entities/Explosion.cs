namespace AdsGame.Entities
{
    using Ads.Game;
    using Microsoft.Xna.Framework.Graphics;

    public class Explosion : AnimatedSprite
    {

        public Explosion(float x, float y, IEngine engine) : base(engine, GraphicsMode.horizontalStrip)
        {
            X = x;
            Y = y;
        }

        public override void LoadResources()
        {
            Texture = Engine.Content.Load<Texture2D>("Graphics\\explosion");
            Width = 134;
            Height = 134;
            FrameCount = 12;
            FrameTime = 30;
            Looping = false;
        }


    }
}
