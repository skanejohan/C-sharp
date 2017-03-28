namespace Ads.Game
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public interface IInputManager
    {
        bool Left();
        bool Right();
        bool Up();
        bool Down();
        bool KeyIsDown(Keys keys);
        bool KeyDown(Keys keys);
        bool KeyUp(Keys keys);
        Vector2 MouseDeltaPosition();
    }
}
