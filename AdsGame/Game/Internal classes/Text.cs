namespace Ads.Game.Internal
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    public class Text : Entity, IVisibleEntity, IPositionable
    {
        public float X { get; set; }
        public float Y { get; set; }

        private SpriteFont font;
        private string fontName;
        private Func<string> getText;
        private bool centered;
        private Color color;

        internal Text(float x, float y, bool centered, string fontName, Color color, Func<string> getText, IEngine engine) : base(engine)
        {
            X = x;
            Y = y;
            this.centered = centered;
            this.fontName = fontName;
            this.getText = getText;
            this.color = color;
        }

        public virtual void Initialize()
        {
        }

        public virtual void LoadResources()
        {
            font = Engine.Content.Load<SpriteFont>(fontName);
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            float x;
            var text = getText();
            if (centered)
            {
                x = (Engine.GraphicsDevice.Viewport.TitleSafeArea.Width - font.MeasureString(text).X) / 2;
            }
            else
            {
                x = X;
            }
            if (Alive)
            {
                spriteBatch.DrawString(font, getText(), new Vector2(x, Y), color);
            }
        }

    }
}
