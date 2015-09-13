using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Xml;
using NLua;

namespace Game1
{
    public class Text :  IDrawable
    {
        string _value = "";
        public Vector2 position = new Vector2(0, 0);
        public SpriteBatch spriteBatch { set { this.sb = value; } }
        public static SpriteFont DefaultFont = Game1.MainGame.Content.Load<SpriteFont>("Courier New");
        public float Scale = 0.5f;
        public Color color;
        public int Width { get { return (int)(font.MeasureString(_value).X * Scale); } }
        public int Height { get { return (int)(font.MeasureString(_value).Y * Scale); } }
        SpriteBatch sb;
        SpriteFont font;
        
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public Text(string text, SpriteBatch sb = null, SpriteFont sf = null)
        {
            this.sb = sb;
            if (sf != null) font = sf;
            else font = DefaultFont;
            _value = text;
        }

        public void Update(GameTime gt)
        {

        }

        public virtual void Draw(float x, float y, int width = 0, int height = 0)
        {
            sb.DrawString(font,Value, new Vector2(position.X + x, position.Y + y), color,0,new Vector2(0,0),Scale,SpriteEffects.None,0);
        }
    }
}
