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
    public struct CollisionRect
    {
        public int x;
        public int y;
        public CollisionRect(int x,int y)
        {
            this.x = x;
            this.y = y;
        }
        public string ToString()
        {
            return "{" + x + " , " + y + "}";
        }
        public static bool isNear(CollisionRect a, CollisionRect b)
        {
            if   (a.x - 1 <= b.x &&
                  a.x + 1 >= b.x &&
                  a.y - 1 <= b.y &&
                  a.y + 1 >= b.y   ) return true;
            else return false;
        }
    }
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
    public delegate void onCollisionEvent(Object o1, Object o2);
    public delegate void onEventTileMap(Actor act);
    class Stage 
    {
        public int Width { get { return background.WidthPix; } }
        public int Height { get { return background.HeightPix; } }
        TileMap background;
        public Rectangle window;
        int width;
        int height;
        List<Actor> actors;
        SpriteBatch spriteBatch;
        Rectangle collisionIndexRect;
        public event onEventTileMap onMapOut;
        public void Reindex()
        {
            foreach (Actor a in actors)
                a.Reindex(new Vector2(0, 0), collisionIndexRect);
            background.Reindex(new Vector2(0, 0), collisionIndexRect);
        }
        public void CalibrateCollisions()
        {
            int w = int.MinValue, h = int.MinValue;
            foreach (Actor a in actors)
            {
                if (a.Width > w) w = a.Width;
                if (a.Height > h) h = a.Height;
            }
            if (background.MaxTileHeight > h) h = background.MaxTileHeight;
            if (background.MaxTileWidth > w) w = background.MaxTileWidth;
            collisionIndexRect = new Rectangle(0,0,w*2,h*2);
        }
        public bool TryCollision(Actor act,Vector2 testPosition)
        {
            /*for (int i = 0; i < actors.Count; i++)
                    if (act != actors[i] && actors[i].TryCollision(act,testPosition,new Vector2(window.X,window.Y))) return true;*/
            return background.TryCollision(act, testPosition , new Vector2(-window.X, -window.Y));
        }
        public void Collision()
        {
            for (int i = 0;i<actors.Count;i++)
            {
                background.Collision(actors[i],new Vector2(window.X,window.Y));
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
            spriteBatch.Begin();
            background.Draw(-window.X,-window.Y,window.Width,window.Height);
            foreach (Actor act in actors)
                act.Draw(-window.X, -window.Y);
            spriteBatch.End();
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
        private void ActorReindex(Actor act)
        {
            act.Reindex(new Vector2(0, 0), collisionIndexRect);
        }
        public void MapOutCheck(Actor act)
        {
            if (act.position.X + act.Width > background.WidthPix ||
                act.position.Y + act.Height > background.HeightPix ||
                act.position.X < 0 ||
                act.position.Y < 0)
            {
                if (act.onMapOut != null) act.onMapOut(act);
                if (onMapOut != null) onMapOut(act);
            }
        }
        public void addActor(Actor act)
        {
            actors.Add(act);
            act.spriteBatch = this.spriteBatch;
            act.onMove += ActorReindex;
            act.onMove += MapOutCheck;
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
