using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Xml;

namespace Game1
{
    public class Sprite
    {
        public Sprite(Game1 g = null)
        {
            name = "";
            options = new Dictionary<string, string>();
            fileName = "";
            x = -1;
            y = -1;
            width = -1;
            height = -1;
            img = null;
            passable = true;
            parentName = "";
        }
        public Texture2D img;
        public int x;
        public int y;
        public int width;
        public int height;
        public string name;
        public string fileName;
        public bool passable;
        public Dictionary<String, String> options;
        public int[] collisionRect = new int[4];
        public bool collisionRectValid = false;
        public string parentName;

        public Sprite Copy()
        {
            Sprite s = new Sprite();
            s.x = x;
            s.y = y;
            s.img = img;
            s.width = width;
            s.height = height;
            s.name = String.Copy(name);
            s.fileName = String.Copy(fileName);
            s.passable = passable;
            s.options = options;
            Array.Copy(collisionRect, s.collisionRect, collisionRect.Length);
            s.collisionRectValid = collisionRectValid;
            s.parentName = String.Copy(parentName);
            return s;
        }
    }
}
