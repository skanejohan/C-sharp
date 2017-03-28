using Ads.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace AdsGame
{
    public class Avatar : SpriteWithStates
    {
        public static int Idle => 1;
        public static int RunningRight => 2;
        public static int RunningLeft => 3;

        public Avatar(IEngine engine) : base(engine)
        {
        }

        public override void LoadResources()
        {
            X = 200;
            Y = 280;

            var idleSprite =
                new AnimatedSprite(engine, AnimatedSprite.GraphicsMode.individual)
                {
                    Width = 641,
                    Height = 542,
                    Scale = 0.2f,
                    FrameCount = 10,
                    FrameTime = 50,
                };
            for (var i = 1; i < 11; i++)
            {
                idleSprite.AddTexture($"Graphics/Avatar/Idle ({i})");
            }

            var runningRightSprite =
                new AnimatedSprite(engine, AnimatedSprite.GraphicsMode.individual)
                {
                    Width = 641,
                    Height = 542,
                    Scale = 0.2f,
                    FrameCount = 8,
                    FrameTime = 30,
                };
            for (var i = 1; i < 9; i++)
            {
                runningRightSprite.AddTexture($"Graphics/Avatar/Run ({i})");
            }

            var runningLeftSprite =
                new AnimatedSprite(engine, AnimatedSprite.GraphicsMode.individual, SpriteEffects.FlipHorizontally)
                {
                    Width = 641,
                    Height = 542,
                    Scale = 0.2f,
                    FrameCount = 8,
                    FrameTime = 30,
                };
            for (var i = 1; i < 9; i++)
            {
                runningLeftSprite.AddTexture($"Graphics/Avatar/Run ({i})");
            }

            Add(Idle, idleSprite);
            Add(RunningRight, runningRightSprite);
            Add(RunningLeft, runningLeftSprite);

            State = Idle;
        }
    }

    public class TiledGame
    {
        private IScene scene;
        private Avatar avatar;
        private Rectangle availableArea;

        private readonly int moveSpeed = 8;
        private readonly int border = 100;

        private void loadSceneAssets(IEngine engine, IScene scene)
        {
            // Assets from http://www.gameart2d.com/free-platformer-game-tileset.html

            scene.TileWidth = 128;
            scene.TileHeight = 128;

            var roofTile = scene.AddTileType("Graphics\\FreeTileSet\\Tiles\\9", BlockingType.None);
            var leftPlatformTile = scene.AddTileType("Graphics\\FreeTileSet\\Tiles\\13", BlockingType.Top);
            var centerPlatformTile = scene.AddTileType("Graphics\\FreeTileSet\\Tiles\\14", BlockingType.Top);
            var rightPlatformTile = scene.AddTileType("Graphics\\FreeTileSet\\Tiles\\15", BlockingType.Top);
            var leftFloorTile = scene.AddTileType("Graphics\\FreeTileSet\\Tiles\\1", BlockingType.All);
            var centerFloorTile = scene.AddTileType("Graphics\\FreeTileSet\\Tiles\\2", BlockingType.All);
            var rightFloorTile = scene.AddTileType("Graphics\\FreeTileSet\\Tiles\\3", BlockingType.All);

            for (var i = 0; i < 50; i++)
            {
                scene.SetTile(i, 0, roofTile);
            }

            scene.SetTile(5, 2, leftPlatformTile);
            scene.SetTile(6, 2, centerPlatformTile);
            scene.SetTile(7, 2, centerPlatformTile);
            scene.SetTile(8, 2, rightPlatformTile);

            scene.SetTile(1, 3, leftFloorTile);
            scene.SetTile(2, 3, centerFloorTile);
            scene.SetTile(3, 3, centerFloorTile);
            scene.SetTile(4, 3, rightFloorTile);

            availableArea.X = border;
            availableArea.Y = border;
            availableArea.Width = engine.GraphicsDevice.Viewport.TitleSafeArea.Width - 2 * border;
            availableArea.Height = engine.GraphicsDevice.Viewport.TitleSafeArea.Height - 2 * border;
        }

        private void MoveLeft()
        {
            avatar.State = Avatar.RunningLeft;
            scene.MoveBlocking(Direction.Left, avatar, moveSpeed);
        }

        private void MoveRight()
        {
            avatar.State = Avatar.RunningRight;
            scene.MoveBlocking(Direction.Right, avatar as IPositionable, moveSpeed);
        }

        private void MoveUp()
        {
            avatar.State = Avatar.Idle;
            scene.MoveBlocking(Direction.Up, avatar as IPositionable, moveSpeed);
        }

        private void MoveDown()
        {
            avatar.State = Avatar.Idle;
            scene.MoveBlocking(Direction.Down, avatar, moveSpeed);
        }

        public void Run()
        {
            using (var game = GameFactory.GetGameEngine(s => { }))
            {
                game.RegisterKeyDownEvent(Keys.Escape, () => game.Exit());
                game.RegisterKeyDownEvent(Keys.D, () => game.DebugMode = !game.DebugMode);
                game.AddEntity(new Background(game, "Graphics /FreeTileSet/BG/BG"));

                scene = game.AddScene(100, loadSceneAssets);
                game.RegisterConditionalEvent(new ConditionalEvent(game, () => game.Left(), MoveLeft));
                game.RegisterConditionalEvent(new ConditionalEvent(game, () => game.Right(), MoveRight));
                game.RegisterConditionalEvent(new ConditionalEvent(game, () => game.Up(), MoveUp));
                game.RegisterConditionalEvent(new ConditionalEvent(game, () => game.Down(), MoveDown));

                avatar = new Avatar(game);
                game.AddEntity(avatar);

                game.Run();
            }
        }
    }
}
