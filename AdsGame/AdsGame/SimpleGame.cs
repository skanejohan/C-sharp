using Ads.Game;
using AdsGame.Entities;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace AdsGame
{
    public class SimpleGame
    {
        Player player;
        List<Enemy> enemies = new List<Enemy>();

        void AddEnemy(IEngine engine)
        {
            Enemy enemy = engine.AddEntity(new Enemy(engine)) as Enemy;
            engine.RegisterCollisionEvent(player as ICollidable, enemy as ICollidable, (p, e) =>
            {
                player.Health -= 10;
                enemy.Alive = false;
                enemies.Remove(enemy);
                if (player.Health <= 0)
                {
                    player.Alive = false;
                }
            });
            enemies.Add(enemy);
        }

        void FireLaser(IEngine engine)
        {
            var laser = engine.AddEntity(new Laser(player.X, player.Y, engine));
            foreach (var enemy in enemies)
            {
                engine.RegisterCollisionEvent(laser as ICollidable, enemy as ICollidable, (l, e) =>
                {
                    player.Score += 10;
                    engine.AddEntity(new Explosion((e as IPositionable).X, (e as IPositionable).Y, engine));
                    enemies.Remove(enemy);
                    enemy.Alive = false;
                });
            }
        }

        public void Run()
        {
            using (var game = GameFactory.GetGameEngine(s => { }))
            {
                // End the game when "Escape" is pressed
                game.RegisterKeyDownEvent(Keys.Escape, () => game.Exit());

                // Pause/unpause the game when "P" is pressed
                game.RegisterKeyDownEvent(Keys.P, () => game.SwitchAll());

                // Switch debug info on or off when "D" is pressed
                game.RegisterKeyDownEvent(Keys.D, () => game.DebugMode = !game.DebugMode);

                // Register a debug output. If the game is in debug mode, it will output information
                // at the given position, using the given font.
                game.RegisterDebugOutput(10, 80, "Graphics\\gameFont");

                game.AddEntity(new Background(game, "Graphics/mainbackground"));
                game.AddEntity(new ScrollingBackground(game, -1, "Graphics/bgLayer1"));
                game.AddEntity(new ScrollingBackground(game, -2, "Graphics/bgLayer2"));
                player = game.AddEntity(new Player(game)) as Player;

                game.AddText(10, 10, "Graphics\\gameFont", () => $"Score: {player.Score}");
                game.AddText(10, 40, "Graphics\\gameFont", () => $"Health: {player.Health}");

                // Spawn a new enemy every 1.5 s
                game.RegisterTimedEvent(1500, () => AddEnemy(game));

                // Fire a laser beam when the space bar is pressed (if enough time has passed since the last one)
                game.RegisterKeyDownEvent(Keys.Space, () => FireLaser(game));

                //game.AddSong("Sounds\\gameMusic");

                game.Run();
            }
        }
    }
}
