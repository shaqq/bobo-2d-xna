using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BTBD.ScreenManager
{
    public class InputState
    {
        public KeyboardState previousState;
        public KeyboardState currentState;

        public InputState()
        {
            previousState = new KeyboardState();
            currentState = new KeyboardState();
        }

        public void Update()
        {
            previousState = currentState;
            currentState = Keyboard.GetState();
        }

        public bool IsNewPress(Keys key)
        {
            return previousState.IsKeyUp(key) && currentState.IsKeyDown(key);
        }

        public bool IsMenuUp()
        {
            return IsNewPress(Keys.Up);
        }

        public bool IsMenuDown()
        {
            return IsNewPress(Keys.Down);
        }

        public bool IsMenuSelect()
        {
            return IsNewPress(Keys.Enter);
        }

        public bool IsMenuCancel()
        {
            return IsNewPress(Keys.Escape);
        }

        public bool IsPause()
        {
            return IsNewPress(Keys.P) || IsNewPress(Keys.Escape);
        }
    }
}
