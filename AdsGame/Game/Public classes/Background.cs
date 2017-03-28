namespace Ads.Game
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Background : Entity, IVisibleEntity
    {
        private IEngine engine;
        private string texturePath;
        private Texture2D texture;
        private int width;
        private int height;

        public Background(IEngine engine, string texturePath) : base(engine)
        {
            this.engine = engine;
            this.texturePath = texturePath;
            this.texture = null;
        }

        public void Initialize()
        {
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (texture != null)
            {
                spriteBatch.Draw(texture, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            else
            {
                engine.GraphicsDevice.Clear(Color.CornflowerBlue);
            }
        }

        public void LoadResources()
        {
            if (texturePath != "")
            {
                width = engine.GraphicsDevice.Viewport.Width;
                height = engine.GraphicsDevice.Viewport.Height;
                texture = engine.Content.Load<Texture2D>(texturePath);
            }
        }

    }
}
