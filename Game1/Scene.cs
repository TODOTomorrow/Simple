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
    public class Debugger
    {
        string fileName;
        Game g;
        public Debugger(String fileName, Game g)
        {
            this.fileName = fileName;
            System.IO.File.WriteAllText(fileName, "");
            this.g = g;
        }
        public void Log(String text,int level = GameException.LevelInfo)
        {
            System.IO.File.WriteAllText(fileName, System.IO.File.ReadAllText(fileName) + 
                "\n[" + DateTime.Now.ToString() + "] " + text);
        }
        public void Log(GameException e)
        {
            System.IO.File.WriteAllText(fileName, System.IO.File.ReadAllText(fileName) +
               "\n[" + DateTime.Now.ToString() + "] " + e.why);
            if (e.level >= GameException.LevelCatastrophic)
                g.Exit();
        }
    }

    struct Map
    {
        public string name;
        public Sprite defaultSprite;
        public Sprite[,] data;
        public int width;
        public int height;
        public Dictionary<String, String> options;
    }
    public class GameException : System.Exception
    {
        public String why;
        public int level;
        public const int LevelCatastrophic = 10;
        public const int LevelWarning = 5;
        public const int LevelInfo = 0;
        public GameException(String why = "No errors",int level = LevelCatastrophic)
        {
            this.why = why;
            this.level = level;
        }
    }
    struct mapObj
    {
        public string name;
        public Sprite sprite;
        public int x;
        public int y;
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


    class Scene
    {
        Map backgroundMap;
        int tileWidth;
        int tileHeight;
        Rectangle winRect;
        SpriteBatch sb;
        bool bgNeedRedraw;
        public Dictionary<string, Person> persons;
        public Rectangle collisionRect;
        public int X
        {
            get
            {
                return winRect.X;
            }
        }
        public int Y
        {
            get
            {
                return winRect.Y;
            }
        }
        public int winWidth
        {
            get
            {
                return winRect.Width;
            }
        }

        public int winHeight
        {
            get
            {
                return winRect.Height;
            }
        }

        public int width
        {
            get
            {
                return backgroundMap.width * backgroundMap.defaultSprite.width;
            }
        }

        public int height
        {
            get
            {
                return backgroundMap.height * backgroundMap.defaultSprite.height;
            }
        }
        public void ReindexCollisions()
        {
            for (int j = 0; j < backgroundMap.height; j++)
                for (int i = 0;i<backgroundMap.width;i++)
                {
                    if (backgroundMap.data[i, j].collisionRectValid) continue;
                    int x = backgroundMap.data[i, j].x;
                    int y = backgroundMap.data[i, j].y;
                    int w = backgroundMap.data[i, j].width;
                    int h = backgroundMap.data[i, j].height;
                    int cw = collisionRect.Width;
                    int ch = collisionRect.Height;
                    backgroundMap.data[i, j].collisionRect[0] = x / cw + y / ch;
                    backgroundMap.data[i, j].collisionRect[1] = (x + w) / cw + y / ch;
                    backgroundMap.data[i, j].collisionRect[2] = (x + w) / cw + (y + h) / ch;
                    backgroundMap.data[i, j].collisionRect[3] = x / cw + (y + h) / ch;
                    backgroundMap.data[i, j].collisionRectValid = true;
                }
            foreach (Person p in persons.Values)
            {
                if (p.collisionRectValid) continue;
                int x = (int)p.pos.X;
                int y = (int)p.pos.Y;
                int w = p.width;
                int h = p.height;
                int cw = collisionRect.Width;
                int ch = collisionRect.Height;
                p.collisionRect[0] = x / cw + y / ch;
                p.collisionRect[1] = (x + w) / cw + y / ch;
                p.collisionRect[2] = (x + w) / cw + (y + h) / ch;
                p.collisionRect[3] = x / cw + (y + h) / ch;
                p.collisionRectValid = true;
            }
        }
        public Scene(SpriteBatch sb,Map m,Settings set,Dictionary<string,Person> persons = null)
        {
            this.sb = sb;
            backgroundMap = m;
            tileWidth = m.defaultSprite.width;
            tileHeight = m.defaultSprite.height;
            winRect = new Rectangle(set.x, set.y, set.width, set.height);
            if (persons == null) persons = new Dictionary<string, Person>();
            else this.persons = persons;
            bgNeedRedraw = true;
            for (int i = 0;i<backgroundMap.height;i++)
                for (int j = 0; j < backgroundMap.width; j++)
                {
                    backgroundMap.data[i, j].x = j * tileWidth;
                    backgroundMap.data[i, j].y = i * tileHeight;
                }
            collisionRect = new Rectangle(0, 0, 0, 0);
            foreach (Sprite s in backgroundMap.data)
            {
                if (s.width > collisionRect.Width) collisionRect.Width = s.width;
                if (s.height > collisionRect.Height) collisionRect.Height = s.height;
            }
            foreach (Person kv in persons.Values)
            {
                if (kv.width > collisionRect.Width) collisionRect.Width = kv.width;
                if (kv.height > collisionRect.Height) collisionRect.Height = kv.height;
            }
            ReindexCollisions();
        }

        public void setPos(int x,int y)
        {
        }
        public bool isPointInRect(Vector2 p, Rectangle r)
        {
            if (p.X >= r.X && p.Y >= r.Y &&
                p.X <= r.X + r.Width &&
                p.Y <= r.Y + r.Height) return true;
            else return false;
        }
        public bool isPointInWindow(int x,int y)
        {
            return isPointInWindow(new Vector2(x,y));
        }
        public bool isPointInWindow(Vector2 p)
        {
            return isPointInRect(p, winRect);
        }
        public bool isRectInWindow(Rectangle r)
        {
            if (isPointInWindow(r.X, r.Y) || 
                isPointInWindow(r.X + r.Width, r.Y) ||
                isPointInWindow(r.X + r.Width, r.Y + r.Height) ||
                isPointInWindow(r.X, r.Y + r.Height)) return true;
            else return false;
        }
        public void DrawPersons()
        {
            foreach (KeyValuePair<string,Person> kv in persons)
            {
                int h = kv.Value.sprite.height;
                int w = kv.Value.sprite.width;
                Sprite s = kv.Value.sprite;
                if (isRectInWindow(new Rectangle((int)kv.Value.pos.X,(int)kv.Value.pos.Y,kv.Value.width,kv.Value.height)))
                    kv.Value.Draw(sb);
            }
        }
        public void DrawBackground()
        {
            if (!bgNeedRedraw) return;
            int startX = winRect.X / tileWidth;
            int startY = winRect.Y / tileHeight;
            int endX = (winRect.X + winRect.Width) / tileWidth + (((winRect.X + winRect.Width) % tileWidth==0)?0:1);
            int endY = (winRect.Y + winRect.Height) / tileHeight + (((winRect.Y + winRect.Height) % tileHeight == 0) ? 0 : 1);
            sb.Begin();
            int posX = 0, posY = 0;
            for (int i = startY; i < endY; i++ , posY++)
            {
                posX = 0;
                for (int j = startX; j < endX; j++ , posX++)
                {
                    sb.Draw(backgroundMap.data[i, j].img,
                            new Vector2(backgroundMap.data[i, j].x - (winRect.X % tileWidth), 
                                        backgroundMap.data[i, j].y - (winRect.Y % tileHeight)),
                            new Color(255, 255, 255));
                }
            }
            sb.End();
        }

        public void Draw()
        {
            DrawBackground();
            DrawPersons();
        }
        public bool TestMove(Person p,Vector2 to)
        {
            int x = (int)to.X;
            int y = (int)to.Y;
            int w = p.width;
            int h = p.height;
            int cw = collisionRect.Width;
            int ch = collisionRect.Height;
            int[] newCollisionRect = new int[4];
            newCollisionRect[0] = x / cw + y / ch;
            newCollisionRect[1] = (x + w) / cw + y / ch;
            newCollisionRect[2] = (x + w) / cw + (y + h) / ch;
            newCollisionRect[3] = x / cw + (y + h) / ch;
            for (int j = 0; j < backgroundMap.height; j++)
                    for (int i = 0;i<backgroundMap.width;i++)
                    {
                        Sprite s = backgroundMap.data[i, j];
                        if ((s.passable==false) && 
                            (p.collisionRect.Contains<int>(s.collisionRect[0]) ||
                            p.collisionRect.Contains<int>(s.collisionRect[1]) ||
                            p.collisionRect.Contains<int>(s.collisionRect[2]) ||
                            p.collisionRect.Contains<int>(s.collisionRect[3])) &&
                            p.TryCollisionsCheck(to,s)
                            )
                            return false;
                    }
                foreach (Person pp in persons.Values)
                {
                    if (pp == p) continue;
                    if (pp.sprite.passable &&
                        (p.collisionRect.Contains<int>(pp.collisionRect[0]) ||
                        p.collisionRect.Contains<int>(pp.collisionRect[1]) ||
                        p.collisionRect.Contains<int>(pp.collisionRect[2]) ||
                        p.collisionRect.Contains<int>(pp.collisionRect[3])) &&
                        p.TryCollisionsCheck(to,pp.sprite))
                        return false;
                }
                return true;
        }
        public void CollisionsCheck()
        {
            foreach (Person p in persons.Values)
            {
                for (int j = 0; j < backgroundMap.height; j++)
                    for (int i = 0;i<backgroundMap.width;i++)
                    {
                        Sprite s = backgroundMap.data[i, j];
                        if ((s.passable==false) && 
                            (p.collisionRect.Contains<int>(s.collisionRect[0]) ||
                            p.collisionRect.Contains<int>(s.collisionRect[1]) ||
                            p.collisionRect.Contains<int>(s.collisionRect[2]) ||
                            p.collisionRect.Contains<int>(s.collisionRect[3])))
                            p.CollisionsCheck(s);
                    }
                foreach (Person pp in persons.Values)
                {
                    if (pp == p) continue;
                    if (p.collisionRect.Contains<int>(pp.collisionRect[0]) ||
                        p.collisionRect.Contains<int>(pp.collisionRect[1]) ||
                        p.collisionRect.Contains<int>(pp.collisionRect[2]) ||
                        p.collisionRect.Contains<int>(pp.collisionRect[3]))
                    p.CollisionsCheck(pp.sprite);
                }
            }
        }
        public void Update(GameTime gt)
        {
            foreach (KeyValuePair<string, Person> kv in persons)
                kv.Value.Update(gt);
            ReindexCollisions();
            CollisionsCheck();
        }
    }

}
