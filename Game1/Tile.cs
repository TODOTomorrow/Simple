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
    class Tile : ICloneable, IDrawable
    {
        public static Game g;
        public string name;
        public Dictionary<string, string> options;
        public SpriteBatch spriteBatch { set { this.sb = value; } }
        public Vector2 position;
        public int Width { get { return img.Width; } }
        public int Height { get { return img.Height; } }
        public bool passable;
        public event onCollisionEvent onCollision;

        Texture2D img;
        Rectangle contour;
        SpriteBatch sb;
        float angle = 0;
        List<Triangle> collisionContur;

        public void Rotate(float angle)
        {
            this.angle = angle;
            foreach (Triangle t in collisionContur)
                t.Rotate(angle);
        }

        public static List<Triangle> CreateContour(Rectangle r)
        {
            List<Triangle> trgls = new List<Triangle>();
            Vector2 p1 = new Vector2(r.X, r.Y);
            Vector2 p2 = new Vector2(r.X + r.Width, r.Y);
            Vector2 p3 = new Vector2(r.X + r.Width, r.Y + r.Height);
            Vector2 p4 = new Vector2(r.X, r.Y + r.Height);
            trgls.Add(new Triangle(p1, p2, p3));
            trgls.Add(new Triangle(p1, p4, p3));
            return trgls;
        }

        public Object Clone()
        {
            Tile t = new Tile();
            t.img = img;
            t.contour = new Rectangle(contour.X, contour.Y, contour.Width, contour.Height);
            t.passable = passable;
            t.options = options;
            t.name = String.Copy(name);
            t.sb = sb;
            t.position = position;
            t.collisionContur = new List<Triangle>();
            foreach (Triangle tr in collisionContur)
                t.collisionContur.Add((Triangle)tr.Clone());
            return t;
        }
        public void Resize(int width = -1, int height = -1)
        {
            if (width <= 0) width = img.Width;
            if (height <= 0) height = img.Height;
            contour.Width = width;
            contour.Height = height;
            collisionContur = CreateContour(contour);
        }
        public Tile(string imageName = null, SpriteBatch sb = null)
        {
            if (imageName != null)
            {
                img = g.Content.Load<Texture2D>(imageName);
                contour = new Rectangle(0, 0, img.Width, img.Height);
            }
            passable = true;
            options = new Dictionary<string, string>();
            this.sb = sb;
            position = new Vector2(0, 0);
            collisionContur = CreateContour(contour);
        }

        public void Draw(float x, float y, int width = 0, int height = 0)
        {
            if (width == 0) width = img.Width;
            if (height == 0) height = img.Height;
            sb.Begin();
            Vector2 cc = new Vector2(img.Width / 2, img.Height / 2);
            Rectangle r = new Rectangle((int)position.X + (int)x + img.Width / 2,
                                        (int)position.Y + (int)y + img.Height / 2, img.Width, img.Height);
            sb.Draw(img, r, null, Color.White, angle, cc, SpriteEffects.None, 0);
            sb.End();
        }
        public void Update(GameTime gt)
        {

        }
        public bool TryCollision(Tile t, Vector2 thisOffset, Vector2 offset)
        {
            if (t.passable || passable) return false;
            foreach (Triangle trgl in collisionContur)
                foreach (Triangle trgl2 in t.collisionContur)
                    if (trgl.Collision(trgl2, thisOffset + position, t.position + offset))
                        return true;
            return false;
        }
        public bool Collision(Tile t, Vector2 thisOffset, Vector2 offset)
        {
            if (t.passable || passable) return false;
            foreach (Triangle trgl in collisionContur)
                foreach (Triangle trgl2 in t.collisionContur)
                    if (trgl.Collision(trgl2, thisOffset + position, t.position + offset))
                    {
                        if (onCollision != null) onCollision(this, t);
                        return true;
                    }
            return false;
        }
    }
}
