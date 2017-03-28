namespace Ads.Game.Internal
{
    using Ads.Game;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Input.Touch;

    internal class InputManager : IInputManager
    {
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;
        GamePadState currentGamePadState;
        GamePadState previousGamePadState;
        MouseState currentMouseState;
        MouseState previousMouseState;
        Vector2 mouseDeltaPosition;

        public InputManager()
        {
            TouchPanel.EnabledGestures = GestureType.FreeDrag;
            mouseDeltaPosition = new Vector2(0, 0);
        }

        internal void Update()
        {
            previousGamePadState = currentGamePadState;
            previousKeyboardState = currentKeyboardState;
            previousMouseState = currentMouseState;
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            currentMouseState = Mouse.GetState();

            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                mouseDeltaPosition.X = currentMouseState.X - previousMouseState.X;
                mouseDeltaPosition.Y = currentMouseState.Y - previousMouseState.Y;
            }
            else
            {
                mouseDeltaPosition.X = 0;
                mouseDeltaPosition.Y = 0;
            }

        }

        public bool Left() => (KeyIsDown(Keys.Left) || currentGamePadState.DPad.Left == ButtonState.Pressed);
        public bool Right() => (KeyIsDown(Keys.Right) || currentGamePadState.DPad.Right == ButtonState.Pressed);
        public bool Up() => (KeyIsDown(Keys.Up) || currentGamePadState.DPad.Up == ButtonState.Pressed);
        public bool Down() => (KeyIsDown(Keys.Down) || currentGamePadState.DPad.Down == ButtonState.Pressed);
        public bool KeyIsDown(Keys k) => currentKeyboardState.IsKeyDown(k);
        public bool KeyDown(Keys k) => currentKeyboardState.IsKeyDown(k) && !previousKeyboardState.IsKeyDown(k);
        public bool KeyUp(Keys k) => !currentKeyboardState.IsKeyDown(k) && previousKeyboardState.IsKeyDown(k);
        public Vector2 MouseDeltaPosition() => mouseDeltaPosition; 
    }
}
