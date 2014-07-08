using Atma.Engine;
using Atma.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Atma.Managers
{
    public class InputManager : ISubsystem
    {
        public static readonly GameUri Uri = "subsystem:input";

        private KeyboardState lastKeyboardState;
        private KeyboardState currentKeyboardState;

        private MouseState lastMouseState;
        private MouseState currentMouseState;

        public InputManager()
        {

        }

        public void init()
        {
        }

        internal void update()
        {
            lastKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
        }

        public bool WasKeyPressed(Keys key)
        {
            return lastKeyboardState.IsKeyDown(key) && currentKeyboardState.IsKeyUp(key);
        }

        public bool IsAnyKeyDown(params Keys[] keys)
        {
            for (var i = 0; i < keys.Length; i++)
                if (currentKeyboardState.IsKeyDown(keys[i]))
                    return true;

            return false;
        }

        public bool IsKeyDown(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key);
        }

        public bool IsKeyUp(Keys key)
        {
            return currentKeyboardState.IsKeyUp(key);
        }

        public bool IsRightMouseDown { get { return currentMouseState.RightButton == ButtonState.Pressed; } }

        public bool IsLeftMouseDown { get { return currentMouseState.LeftButton == ButtonState.Pressed; } }

        public bool WasLeftMousePressed { get { return lastMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed; } }

        public bool WasRightMousePressed { get { return lastMouseState.RightButton == ButtonState.Released && currentMouseState.RightButton == ButtonState.Pressed; } }

        public int MouseX
        {
            get { return currentMouseState.X; }
        }

        public int MouseY
        {
            get { return currentMouseState.Y; }
        }

        public Vector2 MousePosition { get { return new Vector2(MouseX, MouseY); } }

        public Keys GetKeyByIndex(int index)
        {
            switch (index)
            {
                case 0: return Keys.D1;
                case 1: return Keys.D2;
                case 2: return Keys.D3;
                case 3: return Keys.D4;
                case 4: return Keys.D5;
                case 5: return Keys.D6;
                case 6: return Keys.D7;
                case 7: return Keys.D8;
                case 8: return Keys.D9;
                case 9: return Keys.D0;
            }

            return Keys.None;
        }

        public int GetNumberIndex(Keys key)
        {
            switch (key)
            {
                case Keys.D1:
                case Keys.NumPad1: return 0;
                case Keys.D2:
                case Keys.NumPad2: return 1;
                case Keys.D3:
                case Keys.NumPad3: return 2;
                case Keys.D4:
                case Keys.NumPad4: return 3;
                case Keys.D5:
                case Keys.NumPad5: return 4;
                case Keys.D6:
                case Keys.NumPad6: return 5;
                case Keys.D7:
                case Keys.NumPad7: return 6;
                case Keys.D8:
                case Keys.NumPad8: return 7;
                case Keys.D9:
                case Keys.NumPad9: return 8;
                case Keys.D0:
                case Keys.NumPad0: return 9;
            }

            return -1;
        }

        public Atma.Engine.GameUri uri
        {
            get { return Uri; }
        }

        public void preUpdate(float delta)
        {
            update();
        }

        public void postUpdate(float delta)
        {
        }

        public void shutdown()
        {
        }
    }
}
