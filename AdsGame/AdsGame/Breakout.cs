using Ads.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Breakout
{
    internal enum Location { Left, Top, Right, Bottom }
    internal enum Direction { Horizontal, Vertical }

    internal static class BreakoutConstants
    {
        internal static readonly float BarMovementSpeed = 5.0f;
    }

    internal class BarMovementControl : IMovementControl
    {
        public IConditionalEvent LeftAction { get; }
        public IConditionalEvent RightAction { get; }
        public IConditionalEvent UpAction { get; }
        public IConditionalEvent DownAction { get; }
        public IConditionalEvent MouseMovementAction { get; }

        public BarMovementControl(IEngine engine, Sprite sprite, 
            int min, int max, Direction direction)
        {
            var minVerPos = min + sprite.Height / 2;
            var maxVerPos = max - sprite.Height / 2;
            var minHorPos = min + sprite.Width / 2;
            var maxHorPos = max - sprite.Width / 2;

            LeftAction = new ConditionalEvent(engine, () => engine.Left(), () =>
            {
                if (direction == Direction.Horizontal)
                {
                    sprite.X = MathHelper.Clamp(sprite.X - BreakoutConstants.BarMovementSpeed, minHorPos, maxHorPos);
                }
            });

            RightAction = new ConditionalEvent(engine, () => engine.Right(), () =>
            {
                if (direction == Direction.Horizontal)
                {
                    sprite.X = MathHelper.Clamp(sprite.X + BreakoutConstants.BarMovementSpeed, minHorPos, maxHorPos);
                }
            });

            UpAction = new ConditionalEvent(engine, () => engine.Up(), () =>
            {
                if (direction == Direction.Vertical)
                {
                    sprite.Y = MathHelper.Clamp(sprite.Y - BreakoutConstants.BarMovementSpeed, minVerPos, maxVerPos);
                }
            });

            DownAction = new ConditionalEvent(engine, () => engine.Down(), () =>
            {
                if (direction == Direction.Vertical)
                {
                    sprite.Y = MathHelper.Clamp(sprite.Y + BreakoutConstants.BarMovementSpeed, minVerPos, maxVerPos);
                }
            });

            MouseMovementAction = new ConditionalEvent(engine, () => engine.MouseDeltaPosition() != Vector2.Zero, () =>
            {
            });
        }
    }

    internal class Bar : Sprite
    {
        internal Bar(IEngine engine, Location location, int min, int max, int current, int fix) : base(engine)
        {
            this.location = location;
            switch (location)
            {
                case Location.Left:
                    X = fix;
                    Y = current;
                    Height = 60;
                    Width = 15;
                    spriteEffects = SpriteEffects.FlipHorizontally;
                    MovementControl = new BarMovementControl(engine, this, min, max, Direction.Vertical);
                    break;
                case Location.Top:
                    X = current;
                    Y = fix;
                    Height = 15;
                    Width = 60;
                    spriteEffects = SpriteEffects.FlipVertically;
                    MovementControl = new BarMovementControl(engine, this, min, max, Direction.Horizontal);
                    break;
                case Location.Right:
                    X = fix;
                    Y = current;
                    Height = 60;
                    Width = 15;
                    MovementControl = new BarMovementControl(engine, this, min, max, Direction.Vertical);
                    break;
                case Location.Bottom:
                    X = current;
                    Y = fix;
                    Height = 15;
                    Width = 60;
                    MovementControl = new BarMovementControl(engine, this, min, max, Direction.Horizontal);
                    break;
            }
        }

        public int Left => (int)X - Width / 2;
        public int Top => (int)Y - Height / 2;
        public int Right => (int)X + Width / 2;
        public int Bottom => (int)Y + Height / 2;

        private Location location;

        public override void LoadResources()
        {
            switch (location)
            {
                case Location.Left:
                    Texture = Engine.Content.Load<Texture2D>("Graphics/Breakout/ver_bar");
                    Height = 60;
                    Width = 15;
                    break;
                case Location.Top:
                    Texture = Engine.Content.Load<Texture2D>("Graphics/Breakout/hor_bar");
                    Height = 15;
                    Width = 60;
                    break;
                case Location.Right:
                    Texture = Engine.Content.Load<Texture2D>("Graphics/Breakout/ver_bar");
                    Height = 60;
                    Width = 15;
                    break;
                case Location.Bottom:
                    Texture = Engine.Content.Load<Texture2D>("Graphics/Breakout/hor_bar");
                    Height = 15;
                    Width = 60;
                    break;
            }
        }
    }

    internal class Ball : Sprite
    {
        internal Ball(IEngine engine, int x, int y, int deltaX, int deltaY) : base(engine)
        {
            X = x;
            Y = y;
            xDelta = 5f;
            yDelta = 2f;
        }

        public override void LoadResources()
        {
            Texture = Engine.Content.Load<Texture2D>("Graphics/Breakout/ball1");
            Scale = 0.03f;
            Height = 800; // 15;
            Width = 800; //15;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Paused)
            {
                X += xDelta;
                Y += yDelta;
                if (X > Engine.GraphicsDevice.Viewport.Width)
                {
                    Alive = false;
                }
                base.Update(gameTime);
            }
        }

        public float xDelta;
        public float yDelta;
    }

    internal class Wall : ICollidable
    {
        public Wall(int x, int y, int xTiles, int yTiles)
        {
            rectangle = new Rectangle(x * 20, y * 20, (x + xTiles) * 20, (y + yTiles) * 20);
        }

        public Rectangle Rectangle() => rectangle;

        private Rectangle rectangle;
    }

    class Game
    {
        private Ball ball;
        private List<Bar> bars = new List<Bar>();
        float lastUpdateTime;

        public void Run()
        {
            using (var engine = GameFactory.GetGameEngine(s => { }))
            {
                //Background = Color.Black;

                // End the game when "Escape" is pressed
                engine.RegisterKeyDownEvent(Keys.Escape, () => engine.Exit());

                // Switch debug info on or off when "D" is pressed
                engine.RegisterKeyDownEvent(Keys.D, () => engine.DebugMode = !engine.DebugMode);

                var scene = engine.AddScene(0, loadSceneAssets);
                ball = engine.AddEntity(new Ball(engine, 200, 160, 1, 1)) as Ball;
                AddBar(engine, Location.Left, 60, 180, 120, 15);
                AddBar(engine, Location.Left, 280, 420, 350, 15);
                AddBar(engine, Location.Top, 100, 300, 200, 15);
                AddBar(engine, Location.Top, 500, 700, 600, 15);
                AddBar(engine, Location.Right, 60, 180, 120, 780);
                AddBar(engine, Location.Right, 280, 420, 350, 780);
                AddBar(engine, Location.Bottom, 100, 300, 200, 465);
                AddBar(engine, Location.Bottom, 500, 700, 600, 465);

                engine.RegisterConditionalEvent(new ConditionalEvent(engine, () => true, ActivateClosestBar));

                engine.Run();
            }
        }

        private void AddBar(IEngine engine, Location location, int min, int max, int current, int fix)
        {
            var bar = engine.AddEntity(new Bar(engine, location, min, max, current, fix)) as Bar;
            engine.RegisterCollisionEvent(ball as ICollidable, bar as ICollidable, hitBar);
            bars.Add(bar);
        }

        private void loadSceneAssets(IEngine engine, IScene scene)
        {
            // Assets from http://www.gameart2d.com/free-platformer-game-tileset.html

            scene.TileWidth = 20;
            scene.TileHeight = 20;

            var wallTile = scene.AddTileType("Graphics/Breakout/wall", BlockingType.All);

            for (var i = 0; i < 5; i++)
            {
                AddWallTile(engine, scene, i, 0, wallTile);
                AddWallTile(engine, scene, i, 23, wallTile);
            }
            engine.RegisterCollisionEvent(ball as ICollidable, new Wall(0, 0, 5, 1), hitTile);
            engine.RegisterCollisionEvent(ball as ICollidable, new Wall(0, 23, 5, 1), hitTile);

            for (var i = 15; i < 25; i++)
            {
                AddWallTile(engine, scene, i, 0, wallTile);
                AddWallTile(engine, scene, i, 23, wallTile);
            }
            engine.RegisterCollisionEvent(ball as ICollidable, new Wall(15, 0, 10, 1), hitTile);
            engine.RegisterCollisionEvent(ball as ICollidable, new Wall(15, 23, 10, 1), hitTile);

            for (var i = 35; i < 40; i++)
            {
                AddWallTile(engine, scene, i, 0, wallTile);
                AddWallTile(engine, scene, i, 23, wallTile);
            }
            engine.RegisterCollisionEvent(ball as ICollidable, new Wall(35, 0, 5, 1), hitTile);
            engine.RegisterCollisionEvent(ball as ICollidable, new Wall(35, 23, 5, 1), hitTile);

            for (var i = 0; i < 3; i++)
            {
                AddWallTile(engine, scene, 0, i, wallTile);
                AddWallTile(engine, scene, 39, i, wallTile);
            }
            engine.RegisterCollisionEvent(ball as ICollidable, new Wall(0, 0, 1, 3), hitTile);
            engine.RegisterCollisionEvent(ball as ICollidable, new Wall(39, 0, 1, 3), hitTile);

            for (var i = 9; i < 15; i++)
            {
                AddWallTile(engine, scene, 0, i, wallTile);
                AddWallTile(engine, scene, 5, i, wallTile);
                AddWallTile(engine, scene, 34, i, wallTile);
                AddWallTile(engine, scene, 39, i, wallTile);
            }
            engine.RegisterCollisionEvent(ball as ICollidable, new Wall(0, 9, 1, 6), hitTile);
            engine.RegisterCollisionEvent(ball as ICollidable, new Wall(5, 9, 1, 6), hitTile);
            engine.RegisterCollisionEvent(ball as ICollidable, new Wall(34, 9, 1, 6), hitTile);
            engine.RegisterCollisionEvent(ball as ICollidable, new Wall(39, 9, 1, 6), hitTile);

            for (var i = 5; i < 34; i++)
            {
                AddWallTile(engine, scene, i, 9, wallTile);
                AddWallTile(engine, scene, i, 14, wallTile);
            }
            engine.RegisterCollisionEvent(ball as ICollidable, new Wall(5, 9, 29, 1), hitTile);
            engine.RegisterCollisionEvent(ball as ICollidable, new Wall(5, 14, 29, 1), hitTile);

            for (var i = 21; i < 23; i++)
            {
                AddWallTile(engine, scene, 0, i, wallTile);
                AddWallTile(engine, scene, 39, i, wallTile);
            }
            engine.RegisterCollisionEvent(ball as ICollidable, new Wall(0, 21, 1, 3), hitTile);
            engine.RegisterCollisionEvent(ball as ICollidable, new Wall(39, 21, 1, 3), hitTile);
        }

        void AddWallTile(IEngine engine, IScene scene, int x, int y, int tileIndex)
        {
            var tile = scene.SetTile(x, y, tileIndex);
            //engine.RegisterCollisionEvent(ball as ICollidable, tile as ICollidable, hitTile);
        }

        void hitBar(ICollidable obj1, ICollidable obj2)
        {
            var ball = obj1 as Ball;
            var bar = obj2 as Bar;
            if (ball.X > bar.Left && ball.X < bar.Right)
            {
                ball.yDelta = -ball.yDelta;
            }
            else if (ball.Y > bar.Top && ball.Y < bar.Bottom)
            {
                ball.xDelta = -ball.xDelta;
            }
            else
            {
                ball.xDelta = -ball.xDelta;
                ball.yDelta = -ball.yDelta;
            }
        }

        void hitTile(ICollidable obj1, ICollidable obj2)
        {
            var ball = obj1 as Ball;
            var tile = obj2 as Wall; // ITile;
            var tileRect = tile.Rectangle();
            if (ball.X > tileRect.Left && ball.X < tileRect.Right)
            {
                ball.yDelta = -ball.yDelta;
            }
            else if (ball.Y > tileRect.Top && ball.Y < tileRect.Bottom)
            {
                ball.xDelta = -ball.xDelta;
            }
            else
            {
                ball.yDelta = -ball.yDelta;
                ball.xDelta = -ball.xDelta;
            }
        }

        float squaredDistanceBetween(IPositionable obj1, IPositionable obj2)
        {
            return (obj1.X - obj2.X) * (obj1.X - obj2.X) + (obj1.Y - obj2.Y) * (obj1.Y - obj2.Y);
        }

        void ActivateClosestBar()
        {
            var index = -1;
            var smallestDistance = float.MaxValue;
            for(var i = 0; i < bars.Count; i++)
            {
                var d = squaredDistanceBetween(bars[i], ball);
                if (d < smallestDistance)
                {
                    index = i;
                    smallestDistance = d;
                }
                bars[i].Paused = true;
            }
            bars[index].Paused = false;
        }
    }
}
