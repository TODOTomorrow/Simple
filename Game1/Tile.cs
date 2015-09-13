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
    public class Tile : ICloneable, IDrawable
    {
        public static Game g;
        public static Random randomizer = new Random();
        public string name;
        public Dictionary<string, string> options;
        public SpriteBatch spriteBatch { set { this.sb = value; foreach (Text t in texts) t.spriteBatch = sb; } }
        public Vector2 position;
        public int Width { get { return contour.Width; } }
        public int Height { get { return contour.Height; } }
        public bool passable;
        public event onCollisionEvent onCollision;
        public CollisionRect cr;
        public bool Visible = true;
        public bool NeedAnim = false;
        public int frameWidth { get { return currFrameRect.Width; } set { currFrameRect.Width = value; } } //ВЫСОТА КАДРА
        public int frameHeigth { get { return currFrameRect.Height; } set { currFrameRect.Height = value; } }//ШИРИНА КАДРА
        public float animSpeed = 200;//СКОРОСТЬ АНИМАЦИИ

        private Dictionary<string, List<LuaFunction>> luaEvents;
        private Rectangle currFrameRect;
        private static int[] locusOfFrame;
        private float time = 0;

        int imgNum;
        List<Texture2D> img;
        Rectangle contour;
        SpriteBatch sb;
        float angle = 0;
        List<Triangle> collisionContur;
        Vector2 center;
        List<Text> texts = new List<Text>();



        public void AddText(Text t)
        {
            texts.Add(t);
            t.spriteBatch = sb;
        }


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

        public bool Animate(int[] anim = null)
        {
            int i;
            if (anim == null)
            {
                //Генерируем стандартную последовательность анимации
                anim = new int[img[imgNum].Width/currFrameRect.Width];
                for (i = 0; i < anim.Length; i++)
                    anim[i] = i;
            }
            for (i = 0; i < anim.LongLength; i++)
                if ((anim[i] * frameWidth) > img[imgNum].Width)
                    return false;
            time = 0;
            currFrameRect.X = 0;
            currFrameRect.Y = 0;

            locusOfFrame = new int[anim.Length];
            for (i = 0; i < anim.Length; i++)
                locusOfFrame[i] = anim[i];

            NeedAnim = true;
            return true;
        }

        

        public CollisionRect Reindex(Vector2 offset, Rectangle collisionRectangle)
        {
            foreach (Triangle trgl in collisionContur)
                cr = trgl.Reindex(offset + position, collisionRectangle);
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
            t.currFrameRect = new Rectangle(currFrameRect.X, currFrameRect.Y, currFrameRect.Width, currFrameRect.Height);
            t.animSpeed = animSpeed;
            t.NeedAnim = NeedAnim;
            t.luaEvents = new Dictionary<string, List<LuaFunction>>(luaEvents);
            return t;
        }
        public void Resize(int width = -1, int height = -1)
        {
            if (width <= 0) width = img[imgNum].Width;
            if (height <= 0) height = img[imgNum].Height;
            //TODO Add recalc frame width and height
            double kWidth = width / this.Width;
            double kHeight = height / this.Height;
            contour.Width = width;
            contour.Height = height;
            collisionContur = CreateContour(contour);
            center = new Vector2(frameWidth / 2, frameHeigth / 2);
        }
        public Tile(object imageName = null, SpriteBatch sb = null, int frameWidth = 0, int frameHeight = 0)
        {
            img = new List<Texture2D>();

            if (imageName != null)
            {
                if (imageName.GetType() == typeof(Texture2D))
                    img.Add((Texture2D)imageName);
                else if (imageName.GetType() == typeof(String))
                    img.Add(g.Content.Load<Texture2D>((String)imageName));
                else throw new Exception("Error type in Tile initializing");

                if (frameWidth == 0) frameWidth = img[0].Width;
                if (frameHeight == 0) frameHeight = img[0].Height;
                contour = new Rectangle(0, 0, frameWidth, frameHeight);
                center = new Vector2(frameWidth / 2, frameHeight / 2);
                currFrameRect = new Rectangle(0,0,frameWidth,frameHeight);
                this.frameWidth = frameWidth;
                this.frameHeigth = frameHeigth;
            }

            passable = true;
            options = new Dictionary<string, string>();
            this.sb = sb;
            position = new Vector2(0, 0);
            collisionContur = CreateContour(contour);
            cr = new CollisionRect(int.MaxValue, int.MaxValue);
            luaEvents = new Dictionary<string, List<LuaFunction>>();
        }

        public void DrawText(float x, float y, int width = 0, int height = 0)
        {
            foreach (Text t in texts)
                t.Draw(x,y,width,height);
        }

        public virtual void Draw(float x, float y, int width = 0, int height = 0)
        {
            if (!Visible) return;
            contour.X = (int)position.X + (int)x;// +(int)center.X / 4;
            contour.Y = (int)position.Y + (int)y;// +(int)center.Y / 4;
            sb.Draw(img[imgNum], null, contour, currFrameRect, center, angle, null,Color.White, SpriteEffects.None, 0);
            DrawText(contour.X, contour.Y, width, height);
        }
        public void Update(GameTime gt)
        {
            if (NeedAnim)
            {
                time += gt.ElapsedGameTime.Milliseconds;

                if (time > animSpeed)
                {
                    currFrameRect.X += currFrameRect.Width;
                    time = 0;
                }
                if (currFrameRect.X / currFrameRect.Width >= locusOfFrame.Length)
                {
                     currFrameRect.X = 0;
                     currFrameRect.Y = 0;
                }
            }
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
                        this.Raise("onCollision",this,t);
                        return true;
                    }
            return false;
        }
    }
}
