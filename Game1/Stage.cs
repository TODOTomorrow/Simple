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
    struct mapObject
    {
        public string name;
        public Tile sprite;
        public int x;
        public int y;
    }
    struct Settings
    {
        public int width;
        public int height;
        public int x;
        public int y;
        public Dictionary<string, string> options;
    }

    interface IDrawable
    {
        void Draw(float x, float y, int width = 0, int height = 0);
        void Update(GameTime gt);
    }
    delegate void onCollisionEvent(Object o1, Object o2);
    class Stage 
    {
        public event onCollisionEvent onCollision;
        public int Width { get { return background.WidthPix; } }
        public int Height { get { return background.HeightPix; } }
        TileMap background;
        Rectangle window;
        int width;
        int height;
        List<Actor> actors;
        SpriteBatch spriteBatch;

        public void Collision()
        {
            for (int i = 0;i<actors.Count;i++)
            {
                background.Collision(actors[i]);
                for (int j = 0; j < actors.Count; j++)
                    if (i!=j) actors[i].Collision(actors[j]);
            }
        }
        public void Update(GameTime gt)
        {
            background.Update(gt);
            foreach (Actor act in actors)
                act.Update(gt);
            Collision();
        }
        public void Draw()
        {
            background.Draw(window.X,window.Y,window.Width,window.Height);
            foreach (Actor act in actors)
                act.Draw(window.X, window.Y);
        }
        public TileMap Background
        {
            set
            {
                this.background = value;
                value.spriteBatch = spriteBatch;
            }
            get
            {
                return background;
            }
        }
        public void addActor(Actor act)
        {
            actors.Add(act);
            act.spriteBatch = this.spriteBatch;
        }
        public void addActor(List<Actor> act)
        {
            foreach (Actor a in act)
                addActor(a);
        }
        public Stage(SpriteBatch sb, int width, int height, Rectangle watch)
        {
            this.spriteBatch = sb;
            window = watch;
            actors = new List<Actor>();
            this.width = width;
            this.height = height;
        }
    }
}
