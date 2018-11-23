using Microsoft.Xna.Framework.Input;

namespace ip3d_tpc3
{
    /// <summary>
    /// Basic controls wrapper for Monogame Input
    /// </summary>
    public static class Controls
    {

        /*
         * this multidimensional array will contain the keys for various controls
         */ 
        public static Keys[,] MovementKeys;

        // enumerator to simplify basic movement access
        public enum Cursor
        {
            Up,
            Down,
            Left,
            Right
        }

        /*
         * Camera movement keys
         */ 
        public static Keys CameraForward     = Keys.NumPad8;
        public static Keys CameraBackward    = Keys.NumPad5;
        public static Keys CameraStrafeLeft  = Keys.NumPad4;
        public static Keys CameraStrafeRight = Keys.NumPad6;
        public static Keys CameraMoveUp      = Keys.NumPad7;
        public static Keys CameraMoveDown    = Keys.NumPad1;

        public static KeyboardState LastKeyboardState;
        public static KeyboardState CurrKeyboardState;

        public static MouseState LastMouseState;
        public static MouseState CurrMouseState;

        public static void Initilalize()
        {
            // init movement keys array
            MovementKeys = new Keys[2,4];

            LastKeyboardState = Keyboard.GetState();
            CurrKeyboardState = Keyboard.GetState();

            LastMouseState = Mouse.GetState();
            CurrMouseState = Mouse.GetState();

        }

        public static void UpdateCurrentStates()
        {
            CurrKeyboardState = Keyboard.GetState();
            CurrMouseState = Mouse.GetState();
        }

        public static void UpdateLastStates()
        {
            LastKeyboardState = CurrKeyboardState;
            LastMouseState = CurrMouseState;
        }
        
        public static bool IsKeyDown(Keys key)
        {

            return CurrKeyboardState.IsKeyDown(key);

        }

        public static bool IsKeyPressed(Keys key)
        {

            return LastKeyboardState.IsKeyUp(key) && CurrKeyboardState.IsKeyDown(key);

        }

    }
}
