using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    public class UserControl
    {
        public delegate void onEventMouse(Vector2 coord);
        public delegate void onEventKeyboard(KeyboardState ks);
        Vector2 oldMouseCoords;
        public event onEventMouse onMouseLeftDown;
        public event onEventMouse onMouseRightDown;
        public event onEventMouse onMouseMiddleDown;
        public event onEventMouse onMouseMove;
        public event onEventKeyboard onKeydown;
        public UserControl()
        {
            MouseState ms = Mouse.GetState();
            
        }
        
        public void Update(GameTime gt)
        {
            MouseState ms = Mouse.GetState();
            if (ms.LeftButton == ButtonState.Pressed &&
                onMouseLeftDown != null) onMouseLeftDown(new Vector2(ms.X,ms.Y));
            if (ms.RightButton == ButtonState.Pressed &&
                onMouseRightDown != null) onMouseRightDown(new Vector2(ms.X, ms.Y));
            if (ms.MiddleButton == ButtonState.Pressed &&
                onMouseMiddleDown != null) onMouseMiddleDown(new Vector2(ms.X, ms.Y));
            if ((oldMouseCoords.X != ms.X || oldMouseCoords.Y != ms.Y) &&
                onMouseMove != null)
            {
                oldMouseCoords = new Vector2(ms.X,ms.Y);
                onMouseMove(new Vector2(ms.X, ms.Y));
            }
            KeyboardState ks = Keyboard.GetState();
            if ((ks.GetPressedKeys().Length != 0) && onKeydown != null) onKeydown(ks);
        }
    }
}
