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
    public class Tile : ICloneable, IDrawable
    {
        public static Game g;
        public static Random randomizer = new Random();
        public string name;
        public Dictionary<string, string> options;
        public SpriteBatch spriteBatch { set { this.sb = value; } }
        public Vector2 position;
        public int Width { get { return contour.Width; } }
        public int Height { get { return contour.Height; } }
        public bool passable;
        public event onCollisionEvent onCollision;
        public CollisionRect cr;
        public bool Visible = true;

        int imgNum;
        List<Texture2D> img;
        Rectangle contour;
        SpriteBatch sb;
        float angle = 0;
        List<Triangle> collisionContur;
        Vector2 center;

        public CollisionRect Reindex(Vector2 offset, Rectangle collisionRectangle)
        {
            foreach (Triangle trgl in collisionContur)
            {
                cr = trgl.Reindex(offset + position, collisionRectangle);
            }
            return cr;
        }

        public void Rotate(float angle)
        {
            this.angle = angle;
        }
        public void AddImg(string imageName)
        {
            img.Add(g.Content.Load<Texture2D>(imageName));
            //TODO add check size
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
            t.center = new Vector2(center.X,center.Y);
            t.angle = angle;
            imgNum = randomizer.Next(img.Count - 1);
            t.imgNum = imgNum;
            return t;
        }
        public void Resize(int width = -1, int height = -1)
        {
            if (width <= 0) width = img[0].Width;
            if (height <= 0) height = img[0].Height;
            contour.Width = width;
            contour.Height = height;
            collisionContur = CreateContour(contour);
            center = new Vector2(width / 2, height / 2);
        }
        public Tile(string imageName = null, SpriteBatch sb = null)
        {
            img = new List<Texture2D>();
            if (imageName != null)
            {
                img.Add(g.Content.Load<Texture2D>(imageName));
                contour = new Rectangle(0, 0, img[0].Width, img[0].Height);
                center = new Vector2(img[0].Width / 2, img[0].Height / 2);
            }
            passable = true;
            options = new Dictionary<string, string>();
            this.sb = sb;
            position = new Vector2(0, 0);
            collisionContur = CreateContour(contour);
            cr = new CollisionRect(int.MaxValue, int.MaxValue);
        }

        public virtual void Draw(float x, float y, int width = 0, int height = 0)
        {
            if (!Visible) return;
            contour.X = (int)position.X + (int)x + contour.Width / 2;
            contour.Y = (int)position.Y + (int)y + contour.Height / 2;
            sb.Draw(img[imgNum], contour, null, Color.White, angle, center, SpriteEffects.None, 0);
        }
        public void Update(GameTime gt)
        {

        }
        public bool TryCollision(Tile t, Vector2 thisOffset, Vector2 offset,float angle)
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
