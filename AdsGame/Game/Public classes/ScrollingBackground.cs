namespace Ads.Game
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class ScrollingBackground : Entity, IVisibleEntity
    {
        private IEngine engine;
        private int scrollSpeed;
        private string texturePath;
        private Texture2D texture;
        private Vector2[] positions;
        private int width;
        private int height;

        public ScrollingBackground(IEngine engine, int scrollSpeed, string texturePath) : base(engine)
        {
            this.engine = engine;
            this.scrollSpeed = scrollSpeed;
            this.texturePath = texturePath;
        }

        public void Initialize()
        {
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i].X += scrollSpeed;
            }
            for (int i = 0; i < positions.Length; i++)
            {
                if (scrollSpeed <= 0 && positions[i].X <= -texture.Width)
                {
                    WrapTextureToLeft(i);
                }
                else if (scrollSpeed > 0 && positions[i].X >= texture.Width * (positions.Length - 1))
                {
                    WrapTextureToRight(i);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                Rectangle rectBg = new Rectangle((int)positions[i].X, (int)positions[i].Y, width, height);
                spriteBatch.Draw(texture, rectBg, Color.White);
            }
        }

        public void LoadResources()
        {
            width = engine.GraphicsDevice.Viewport.Width;
            height = engine.GraphicsDevice.Viewport.Height;
            texture = engine.Content.Load<Texture2D>(texturePath);

            positions = new Vector2[width / texture.Width + 1];
            // Set the initial positions of the parallaxing background
            for (int i = 0; i < positions.Length; i++)
            {
                // We need the tiles to be side by side to create a tiling effect
                positions[i] = new Vector2(i * texture.Width, 0);
            }
        }

        private void WrapTextureToLeft(int index)
        {
            // If the textures are scrolling to the left, when the tile wraps,
            // it should be putone pixel to the right of the tile before it.
            int prevTexture = index - 1;
            if (prevTexture < 0)
            {
                prevTexture = positions.Length - 1;
            }
            positions[index].X = positions[prevTexture].X + texture.Width;
        }

        private void WrapTextureToRight(int index)
        {
            // If the textures are scrolling to the right, when the tile wraps, 
            // it should be placed to the left of the tile that comes after it.
            int nextTexture = index + 1;
            if (nextTexture == positions.Length)
            {
                nextTexture = 0;
            }
            positions[index].X = positions[nextTexture].X - texture.Width;
        }

    }
}
