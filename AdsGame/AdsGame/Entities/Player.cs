namespace AdsGame.Entities
{
    using Ads.Game;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    class Player : AnimatedSprite
    {
        public int Health { get; set; }
        public int Score { get; set; }

        private readonly float moveSpeed = 8.0f;

        public Player(IEngine engine) : base(engine, GraphicsMode.horizontalStrip)
        {
            MovementControl = new StandardMovementControl(engine, this, moveSpeed, ClampPosition);
        }

        public override void Initialize()
        {
            Health = 100;
        }

        public override void LoadResources()
        {
            Texture = Engine.Content.Load<Texture2D>("Graphics\\shipAnimation");
            Width = 115;
            Height = 69;
            FrameCount = 8;
            FrameTime = 50;
            X = Engine.GraphicsDevice.Viewport.TitleSafeArea.X;
            Y = Engine.GraphicsDevice.Viewport.TitleSafeArea.Y + Engine.GraphicsDevice.Viewport.TitleSafeArea.Height / 2;
        }

        private void ClampPosition()
        {
            X = MathHelper.Clamp(X, 0, Engine.GraphicsDevice.Viewport.TitleSafeArea.Width - Width);
            Y = MathHelper.Clamp(Y, 0, Engine.GraphicsDevice.Viewport.TitleSafeArea.Height - Height);
        }

    }
}
