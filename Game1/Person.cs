using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    class Person
    {
        public Sprite sprite;
        public string name;
        public Dictionary<string, string> options;
        public int width;
        public int height;
        public double _speed;
        bool moveToLeft, moveUp;
        public enum State { MOVE, STOP };
        public State state;
        Vector2 moveTo;
        public delegate void onEventMove(Person who, Vector2 movedTo);
        public delegate void onCollisionEvent(Sprite s);
        public event onEventMove onMoveEnd;
        public event onCollisionEvent onCollision;
        double dx = 0;
        double dy = 0;
        float rotation;
        float scale;
        public Scene parent;
        public Vector2 pos;
        public int[] collisionRect = new int[4];
        public bool collisionRectValid = false;
        public void Stop()
        {
            this.state = State.STOP;
        }
        public void Draw(SpriteBatch sb)
        {
            sb.Begin();
            Vector2 cc = new Vector2(width/2,height/2);
            sb.Draw(sprite.img, new Rectangle((int)pos.X + width/2,(int)pos.Y + height/2, width,height), null,Color.White, rotation, cc, SpriteEffects.None,0);
            sb.End();
        }
        public bool isPointInRect(Vector2 p, Rectangle r)
        {
            if (p.X > r.X && p.Y > r.Y &&
                p.X < (r.X + r.Width) &&
                p.Y < (r.Y + r.Height)) 
                return true;
            else return false;
        }
        public void CollisionsCheck(Sprite sprite)
        {
            Rectangle r = new Rectangle(sprite.x - width, sprite.y - height, sprite.width + width, sprite.height + height);
            if (onCollision != null &&
                isPointInRect(pos, r))
             onCollision(sprite);
        }

        public bool TryCollisionsCheck(Vector2 testPoint, Sprite sprite)
        {
            Rectangle r = new Rectangle(sprite.x - width, sprite.y - height, sprite.width + width, sprite.height + height);
            if (!sprite.passable && isPointInRect(testPoint, r))
                return true;
            else return false;
            /*if (testPoint.X + width > sprite.x &&
                testPoint.X < sprite.x + sprite.width &&
                testPoint.Y + height > sprite.y &&
                testPoint.Y < sprite.y + sprite.height) return true;
            else return false;*/
        }

        public void Update(GameTime elapsedTime)
        {
            if (state == State.STOP) return;
            double dt = elapsedTime.ElapsedGameTime.Milliseconds;
            collisionRectValid = false;
            double deltaX = dt * dx;
            double deltaY = dt * dy;
            Vector2 testP = pos + new Vector2((float)Math.Floor(deltaX), (float)Math.Floor(deltaY));
            if (!parent.TestMove(this, testP))
            {
                state = State.STOP;
                return;
            }
            this.pos.X += (float)Math.Floor(deltaX);
            this.pos.Y += (float)Math.Floor(deltaY);
            
            bool endOfMoveX = false, endOfMoveY = false;

            if ((moveToLeft && this.pos.X <= moveTo.X) ||
             (!moveToLeft && this.pos.X >= moveTo.X))
            {
                endOfMoveX = true;
                dx = 0;
            }
            if ((moveUp && this.pos.Y <= moveTo.Y) ||
             (!moveUp && this.pos.Y >= moveTo.Y))
            {
                endOfMoveY = true;
                dy = 0;
            }

            if (endOfMoveX && endOfMoveY)
            {
                this.state = State.STOP;
                if (onMoveEnd != null)
                    onMoveEnd(this, this.moveTo);
            }

        }


        public void Rotate(float x, float y)
        {
            Rotate(new Vector2(x, y));
        }
        public void Rotate(Vector2 c)
        {
            rotation = (float)Math.Atan2((c - pos).Y, (c-pos).X);
        }
        public void Rotate(int x, int y)
        {
            Rotate(new Vector2(x, y));
        }
        public void Rotate(float angle)
        {
            rotation = angle;
        }
        public void moveToXYLong(Vector2 p)
        {
            double x1 = p.X;
            double y1 = p.Y;
            double x0 = this.pos.X;
            double y0 = this.pos.Y;

            if (x0 >= x1) moveToLeft = true;
            else moveToLeft = false;
            if (y0 >= y1) moveUp = true;
            else moveUp = false;

            double k = (y1 - y0) / (x1 - x0);
            double b = y0;
            double S = Math.Sqrt(Math.Pow((y0 - y1), 2) + Math.Pow((x0 - x1), 2));
            double speed = this._speed;
            double t = S / speed;
            dx = (x1 - x0) / t;
            dy = (y1 - y0) / t;
            moveTo.X = float.NaN;
            moveTo.Y = float.NaN;
            state = State.MOVE;
        }
        public void moveToXY(Vector2 p)
        {
            moveToXY(p.X, p.Y);
        }
        public void moveToXY(double x, double y)
        {
            moveToXY(Convert.ToInt32(x), Convert.ToInt32(y));
        }
        public void moveToXY(int x1, int y1)
        {
            double x0 = this.pos.X;
            double y0 = this.pos.Y;

            if (x0 >= x1) moveToLeft = true;
            else moveToLeft = false;
            if (y0 >= y1) moveUp = true;
            else moveUp = false;

            double k = (y1 - y0) / (x1 - x0);
            double b = y0;
            double S = Math.Sqrt(Math.Pow((y0 - y1), 2) + Math.Pow((x0 - x1), 2));
            double speed = this._speed;
            double t = S / speed;
            dx = (x1 - x0) / t;
            dy = (y1 - y0) / t;
            moveTo.X = x1;
            moveTo.Y = y1;
            state = State.MOVE;
        }

        public Person()
        {
            width = -1;
            height = -1;
            scale = 1.0f;
            state = State.STOP;
        }
    }
}
