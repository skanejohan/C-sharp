namespace Ads.Game.Internal.Extensions
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;

    public static class ICollidableExtensions
    {
        private static Dictionary<string,Texture2D> textures = new Dictionary<string, Texture2D>();

        private static Texture2D GetTexture(GraphicsDevice graphicsDevice, int width, int height)
        {
            Texture2D texture;
            var key = $"{width},{height}";
            if (!textures.TryGetValue(key, out texture))
            {
                Color[] data = new Color[width * height];
                for (int x = 0; x < width; x++)
                {
                    data[x] = Color.White;
                    data[(height - 1) * width + x] = Color.White;
                }
                for (int y = 0; y < height; y++)
                {
                    data[y * width] = Color.White;
                    data[y * width + (width - 1)] = Color.White;
                }
                texture = new Texture2D(graphicsDevice, width, height);
                texture.SetData(data);
                textures[key] = texture;
            }
            return texture;
        }

        public static void DrawRectangle(this ICollidable collidable, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            Rectangle r = collidable.Rectangle();
            var texture = GetTexture(graphicsDevice, r.Width, r.Height);
            spriteBatch.Draw(texture, new Vector2(r.Left, r.Top), Color.White);
        }
    }

}
