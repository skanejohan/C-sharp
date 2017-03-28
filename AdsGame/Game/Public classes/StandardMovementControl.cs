namespace Ads.Game
{
    using Microsoft.Xna.Framework;
    using System;

    public class StandardMovementControl : IMovementControl
    {
        private IEngine engine;
        private IPositionable sprite;
        private float movementSpeed;
        private Action clampFunction;

        public IConditionalEvent LeftAction { get; private set; }
        public IConditionalEvent RightAction { get; private set; }
        public IConditionalEvent UpAction { get; private set; }
        public IConditionalEvent DownAction { get; private set; }
        public IConditionalEvent MouseMovementAction { get; private set; }

        public StandardMovementControl(IEngine engine, IPositionable sprite, float movementSpeed, Action clampFunction)
        {
            this.engine = engine;
            this.sprite = sprite;
            this.movementSpeed = movementSpeed;
            this.clampFunction = clampFunction;

            LeftAction = new ConditionalEvent(engine, () => engine.Left(), () =>
            {
                sprite.X -= movementSpeed;
                clampFunction();
            });

            RightAction = new ConditionalEvent(engine, () => engine.Right(), () =>
            {
                sprite.X += movementSpeed;
                clampFunction();
            });

            UpAction = new ConditionalEvent(engine, () => engine.Up(), () =>
            {
                sprite.Y -= movementSpeed;
                clampFunction();
            });

            DownAction = new ConditionalEvent(engine, () => engine.Down(), () =>
            {
                sprite.Y += movementSpeed;
                clampFunction();
            });

            MouseMovementAction = new ConditionalEvent(engine, () => engine.MouseDeltaPosition() != Vector2.Zero, () =>
            {
                sprite.X += engine.MouseDeltaPosition().X;
                sprite.Y += engine.MouseDeltaPosition().Y;
                clampFunction();
            });
        }
    }

}
