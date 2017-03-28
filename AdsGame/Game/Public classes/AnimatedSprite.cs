namespace Ads.Game
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;

    public class AnimatedSprite : Sprite
    {
        public enum GraphicsMode { horizontalStrip, individual };

        public int FrameTime { get; set; }
        public int FrameCount { get; set; }
        public bool Looping { get; set; }

        public new Texture2D Texture
        {
            get
            {
                if (textures.Count == 0)
                {
                    return null;
                }
                else
                {
                    return textures[0];
                }
            }
            set
            {
                if (textures.Count == 0)
                {
                    textures.Add(value);
                }
                else
                {
                    textures[0] = value;
                }
            }
        }

        private Rectangle sourceRect;
        private Rectangle destinationRect;
        private int elapsedTime;
        private int currentFrame;
        private GraphicsMode graphicsMode;
        private List<Texture2D> textures;

        public AnimatedSprite(IEngine engine, GraphicsMode graphicsMode, SpriteEffects spriteEffects = SpriteEffects.None) : 
            base(engine, spriteEffects)
        {
            this.graphicsMode = graphicsMode;
            textures = new List<Texture2D>();
            sourceRect = new Rectangle();
            destinationRect = new Rectangle();
            elapsedTime = 0;
            currentFrame = 0;
            Looping = true;
        }

        public void RestartAnimation()
        {
            currentFrame = 0;
        }

        public void AddTexture(string sourceFile)
        {
            textures.Add(Engine.Content.Load<Texture2D>(sourceFile));
        }

        public override void Update(GameTime gameTime)
        {
            if (Alive == false)
                return;

            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedTime > FrameTime)
            {
                currentFrame++;
                if (currentFrame == FrameCount)
                {
                    currentFrame = 0;
                    if (Looping == false)
                        Alive = false;
                }
                elapsedTime = 0;
            }

            switch(graphicsMode)
            {
                case GraphicsMode.horizontalStrip:
                    sourceRect = new Rectangle(currentFrame * Width, 0, Width, Height);
                    break;
                case GraphicsMode.individual:
                    sourceRect = new Rectangle(0, 0, Width, Height);
                    break;
            }

            destinationRect = new Rectangle(
                (int)X - (int)(Width * Scale) / 2,
                (int)Y - (int)(Height * Scale) / 2,
                (int)(Width * Scale),
                (int)(Height * Scale));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Alive)
            {
                switch (graphicsMode)
                {
                    case GraphicsMode.horizontalStrip:
                        spriteBatch.Draw(textures[0], destinationRect, sourceRect, Color, 
                            0f, Vector2.Zero, spriteEffects, 0f);
                        break;
                    case GraphicsMode.individual:
                        spriteBatch.Draw(textures[currentFrame], destinationRect, sourceRect, Color,
                            0f, Vector2.Zero, spriteEffects, 0f);
                        break;
                }
            }
        }

    }
}
