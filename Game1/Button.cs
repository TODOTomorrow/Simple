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
    public delegate void ControlEvent(Control c);
    public abstract class Control 
    {
        public Vector2 position = new Vector2();
        public List<Tile> baseTiles = new List<Tile>();
        public abstract string Title {get; set;}
        public bool Visible = true;
        public bool Enable;
        public abstract int Width { get; set; }
        public abstract int Height { get; set; }
        protected Text title = new Text("");
        public event ControlEvent onClick;

        public Control()
        {
            if (UserControl.MainUserControl != null)
                UserControl.MainUserControl.onMouse += MouseClickCatch;
        }

        void Resize(int newWidth, int newHeight)
        {

        }
        public virtual void Draw(float x, float y, int width = 0, int height = 0)
        {
            if (!Visible) return;
            foreach (Tile t in baseTiles)
                t.Draw(x + position.X + this.Width / 2, y + position.Y + this.Height / 2, width, height);
            title.Draw(x + position.X, y + position.Y);
        }
        public virtual void Update(GameTime gt)
        {

        }

        public void MouseClickCatch(MouseState st)
        {
            if (st.X > this.position.X && st.X < this.position.X + this.Width
             && st.Y > this.position.Y && st.Y < this.position.Y + this.Height)
                if (onClick != null) onClick(this);
        }
    }

    public class Panel : Control
    {
        public override int Width 
        { 
            get { 
                int max=controls[0].Width;
                foreach (Control c in controls)
                    if (c.Width > max) max = c.Width; 
                return max;
            } 
            set { containerMaxWidth = value; Rearrange(); } 
        }
        public override int Height 
        { 
            get { 
                int max=controls[0].Height;
                foreach (Control c in controls)
                    if (c.Height > max) max = c.Height; 
                return max;
            }
            set { containerMaxHeight = value; Rearrange(); } 
        }
        public override string Title { get { return ""; } set { } }

        List<Control> controls = new List<Control>();
        int containerMaxWidth;
        int containerMaxHeight;
        int lastContainerTop = 0;
        public Panel(int maxWidth, int maxHeight)
        {
            containerMaxWidth = maxWidth;
            containerMaxHeight = maxHeight;
        }
        public void Rearrange()
        {
            foreach (Control c in controls)
            {
                c.position.X = this.position.X + containerMaxWidth / 2 - c.Width / 2;
                c.position.Y = lastContainerTop + position.Y;
                lastContainerTop += c.Height + 10;
            }
        }
        public void AddControl(Control c)
        {
            controls.Add(c);
            Rearrange();
        }
        public override void Draw(float x, float y, int width = 0, int height = 0)
        {
            foreach (Control c in controls)
                c.Draw(x, y, width, height);
        }
    }

    public class Button : Control
    {
        bool _enabled = true;
        Texture2D texture;
        Color _backgroundColor = Color.LightGray;
        SpriteBatch sb;
        int _width = 0;
        int _height = 0;
        public override int Width { get { return _width; } set { } }
        public override int Height { get { return _height; } set { } }
        public override string Title { get { return title.Value; } set { title.Value = value; _width = title.Width; } }
        public Color backgroundColor
        {
            set
            {
                for (int i = 0; i < bgColor.Length; i++)
                    bgColor[i] = value;
                texture.SetData(bgColor);
            }
            get
            {
                return _backgroundColor;
            }
        }
        Color[] bgColor;
        string _bgimageName;
        public string BackgroundImage
        {
            set
            {
                texture = Game1.MainGame.Content.Load<Texture2D>(value);
                baseTiles[0] = new Tile(texture, sb);
                baseTiles[0].Resize(_width, _height);
                _bgimageName = value;
            }
            get
            {
                return _bgimageName;
            }
        }

        public void MouseCatch()
        {

        }

        void setButtonPressed()
        {
            this.position += new Vector2(2,2);
        }

        void setButtonUnpressed()
        {
            this.position -= new Vector2(2, 2);
            _enabled = true;
        }

        void AnimateClick()
        {
            setButtonPressed();
            Timer.SomeTimer.SetTimer(TimeSpan.FromMilliseconds(300), setButtonUnpressed);
        }

        public void onButtonClicked(Control c)
        {
            if (!_enabled) return;
            AnimateClick();
            _enabled = false;
        }

        public Button(string Text, GraphicsDevice gd, SpriteBatch sb) : base()
        {
            this.sb = sb;
            title = new Text(Text, sb);
            title.color = Color.White;
            title.position.X = 5;
            title.position.Y = 5;
            _width = title.Width  + 10;
            _height = title.Height + 10;
            texture = new Texture2D(gd, _width, _height);
            bgColor = new Color[_width * _height];
            for (int i =0;i<bgColor.Length;i++)
                bgColor[i] = _backgroundColor;
            texture.SetData(bgColor);
            Tile t = new Tile(texture , sb);
            this.onClick += onButtonClicked;
            baseTiles.Add(t);
        }
    }
}
