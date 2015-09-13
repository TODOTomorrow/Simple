using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NLua;

namespace Game1
{
    public delegate void MouseCommonEvent(MouseState ms);
    public class UserControl
    {
        public static UserControl MainUserControl;
        public delegate void onEventMouse(Vector2 coord);
        public delegate void onEventKeyboard(KeyboardState ks);
        Vector2 oldMouseCoords;
        public event onEventMouse onMouseLeftDown;
        public event onEventMouse onMouseRightDown;
        public event onEventMouse onMouseMiddleDown;
        public event onEventMouse onMouseMove;
        public event MouseCommonEvent onMouse;
        public event onEventKeyboard onKeydown;
        private Dictionary<string, List<LuaFunction>> luaEvents = new Dictionary<string, List<LuaFunction>>();
        public void Raise(string eventName, params object[] parameters)
        {
            eventName = eventName.ToLower().Trim();
            if (!luaEvents.ContainsKey(eventName)) return;
            foreach (LuaFunction currFunc in luaEvents[eventName])
                currFunc.Call(parameters);
        }

        public void on(string eventName, LuaFunction func)
        {
            if (!luaEvents.ContainsKey(eventName))
                luaEvents[eventName.ToLower().Trim()] = new List<LuaFunction>();
            luaEvents[eventName.ToLower().Trim()].Add(func);
        }
        public UserControl()
        {
            MouseState ms = Mouse.GetState();
            MainUserControl = this;
        }
        
        public void Update(GameTime gt)
        {
            MouseState ms = Mouse.GetState();
            if (ms.LeftButton == ButtonState.Pressed)
            {
                if (onMouse != null) onMouse(ms);
                if (onMouseLeftDown != null) onMouseLeftDown(new Vector2(ms.X, ms.Y));
                Raise("onmouseleftdown",ms.X,ms.Y);
            }
            if (ms.RightButton == ButtonState.Pressed)
            {
                if (onMouse != null) onMouse(ms);
                if (onMouseRightDown != null) onMouseRightDown(new Vector2(ms.X, ms.Y));
                Raise("onmouserightdown",ms.X,ms.Y);
            }
            if (ms.MiddleButton == ButtonState.Pressed)
            {
                if (onMouse != null) onMouse(ms);
                if (onMouseMiddleDown != null) onMouseMiddleDown(new Vector2(ms.X, ms.Y));
                Raise("onmousemiddledown", ms.X, ms.Y);
            }
            if ((oldMouseCoords.X != ms.X || oldMouseCoords.Y != ms.Y) &&
                onMouseMove != null)
            {
                oldMouseCoords = new Vector2(ms.X,ms.Y);
                onMouseMove(new Vector2(ms.X, ms.Y));
                Raise("onmousemove", ms.X, ms.Y);
            }
            KeyboardState ks = Keyboard.GetState();
            if (ks.GetPressedKeys().Length != 0)
            {
                if (onKeydown != null) onKeydown(ks);
                Raise("onKeydown",ks.GetPressedKeys()[0].ToString());
            }
        }
    }
}
