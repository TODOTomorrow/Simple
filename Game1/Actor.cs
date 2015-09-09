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
    public delegate void ActorEvent(Actor act);
    public enum ActorState { Stop, Move }
    public class Actor : IDrawable
    {
        public string name;
        public SpriteBatch spriteBatch { set { this.sb = value; foreach (Tile t in tiles) t.spriteBatch = value; } }
        public Vector2 position;
        public float speed = 0.4f;
        public event onCollisionEvent onCollision;
        public ActorEvent onMoveEnd;
        public ActorEvent onMove;
        public ActorEvent onMapOut;
        public ActorState State { get { return state; } }
        public CollisionRect cr;
        float _angle;
        public float angle
        {
            get { return _angle; }
            set { _angle = NormalizeAngle(value); }
        }
        public float RotateSpeed;
        public int Width 
        { 
            get 
            { 
                int width = int.MinValue; 
                foreach (Tile t in tiles) 
                    if (t.Width > width) width = t.Width; 
                return width; 
            } 
        }
        public int Height
        {
            get
            {
                int height = int.MinValue;
                foreach (Tile t in tiles)
                    if (t.Height > height) height = t.Width;
                return height;
            }
        }
        ActorState state = ActorState.Stop;
        SpriteBatch sb;
        List<Tile> tiles;
        Rectangle profile;

        private double dxSpeed;
        private double dySpeed;
        private Vector2 endPoint = new Vector2();
        private bool moveToLeft, moveUp;

        private float rotateToAngle;
        bool inRotation = false;
        bool incRotate = false;
        float movedRotate = 0;
        const float rotateEps = 0.03f;        

        public CollisionRect Reindex(Vector2 offset, Rectangle collisionRectangle)
        {
            foreach (Tile t in tiles)
            {
                cr = t.Reindex(offset + position, collisionRectangle);
            }
            return cr;
        }


        private float NormalizeAngle(float angle)
        {
            if (angle < 0) angle = (float)Math.PI * 2 - angle * -1;
            if (angle > Math.PI * 2) angle = angle % (float)(Math.PI * 2);
            return angle;
        }
        public void Rotate(float angle)
        {
            angle = NormalizeAngle(angle);
            if (angle + rotateEps >= this.angle && angle - rotateEps <= this.angle) return;
            float d1 = NormalizeAngle(angle - this.angle);
            float d2 = (float)Math.PI*2 - d1;

            if (d2 < d1)
            {
                incRotate = false;
                movedRotate = d2;
            }
            else
            {
                incRotate = true;
                movedRotate = d1;
            }
            rotateToAngle = angle;
            inRotation = true;
        }
        private void UpdateRotate(GameTime gt)
        {
            if (!inRotation) return;
            float delta = gt.ElapsedGameTime.Milliseconds * RotateSpeed / 1000;
            if (incRotate)  angle += delta;
            else            angle -= delta;
            movedRotate -= delta;
            if (movedRotate <= 0)
            {
                inRotation = false;
                angle = rotateToAngle;
            }
            foreach (Tile t in tiles)
                t.Rotate(angle);
        }
        public void Rotate(Vector2 to)
        {
            float angle = (float)Math.Atan2((to - position).Y, (to - position).X);
            Rotate(angle);
        }
        public void Stop()
        {
            state = ActorState.Stop;
        }
        public Actor(String name, Tile t,int x,int y)
        {
            profile = new Rectangle();
            this.name = name;
            this.position = new Vector2(x,y);
            tiles = new List<Tile>();
            cr = new CollisionRect(int.MaxValue, int.MaxValue);
            RotateSpeed = 2.7f;
            AddTile(t, 0, 0);
        }

        public Actor(Rectangle profile, string name = "",float angle = 0)
        {
            this.profile = profile;
            this.name = name;
            this.position = new Vector2(profile.X,profile.Y);
            this.angle = angle;
            tiles = new List<Tile>();
            cr = new CollisionRect(int.MaxValue,int.MaxValue);
            RotateSpeed = 2.7f;
        }

        public void StartAnim()
        {
            foreach (Tile t in tiles)
                t.Animate();
        }

        public void AddTile(Tile t, float xOffset = 0, int yOffset = 0)
        {
            tiles.Add((Tile)t.Clone());
            if (t.Width > profile.Width) profile.Width = t.Width;
            if (t.Height > profile.Height) profile.Height = t.Height;
            t.position.X = xOffset;
            t.position.Y = yOffset;
        }
        public void AddTile(List<Tile> tiles)
        {
            foreach (Tile t in tiles)
                AddTile(t);
        }
        public void Draw(float x, float y, int width = 0, int height = 0)
        {
            foreach (Tile t in tiles)
                t.Draw(position.X + x, position.Y + y);
        }

        public void Move(Vector2 p)
        {
            double x1 = p.X;
            double y1 = p.Y;
            double x0 = position.X;
            double y0 = position.Y;
            double k = (y1 - y0) / (x1 - x0);
            double b = y0;
            double S = Math.Sqrt(Math.Pow((y0 - y1), 2) + Math.Pow((x0 - x1), 2));
            double speed = this.speed;
            double t = S / speed;
            dxSpeed = (x1 - x0) / t;
            dySpeed = (y1 - y0) / t;
            endPoint.X = (float)x1;
            endPoint.Y = (float)y1;
            moveToLeft = (position.X >= endPoint.X) ? true : false;
            moveUp = (position.Y >= endPoint.Y) ? true : false;
            state = ActorState.Move;
        }

        private void UpdatePosition(double ms)
        {
            if (state == ActorState.Stop) return;
            double deltaX = ms * dxSpeed;
            double deltaY = ms * dySpeed;
            position.X += (float)deltaX;
            position.Y += (float)deltaY;

            bool endMoveX = false, endMoveY = false;
            if ((moveToLeft && position.X <= endPoint.X) ||
                (!moveToLeft && position.X >= endPoint.X))
            {
                endMoveX = true;
                position.X = endPoint.X;
                dxSpeed = 0;
            }
            if ((moveUp && position.Y <= endPoint.Y) ||
             (!moveUp && position.Y >= endPoint.Y))
            {
                endMoveY = true;
                position.Y = endPoint.Y;
                dySpeed = 0;
            }

            if (endMoveY && endMoveX)
            {
                if (onMoveEnd != null) onMoveEnd(this);
                position.X = endPoint.X;
                position.Y = endPoint.Y;
                endPoint.X = float.NaN;
                endPoint.Y = float.NaN;
                state = ActorState.Stop;
            }
            if (onMove != null) onMove(this);
        }
        public void Update(GameTime gt)
        {
            UpdatePosition(gt.ElapsedGameTime.Milliseconds);
            UpdateRotate(gt);
            foreach (Tile t in tiles) t.Update(gt);
        }
        public bool Collision(Actor p)
        {
            foreach (Tile t in p.tiles)
                if (Collision(t, p.position))
                {
                    if (onCollision != null) onCollision(this, p);
                    return true;
                }
            return false;
        }
        public bool Collision(Tile t, Vector2 tileOffset, bool collisionEventFlag = false)
        {
            if (!CollisionRect.isNear(cr, t.cr)) return false;
            foreach (Tile myTile in tiles)
                if (myTile.Collision(t, this.position, tileOffset))
                {
                    if (onCollision != null && collisionEventFlag) onCollision(this, t);
                    return true;
                }
            return false;
        }
        
        public bool TryCollision(Tile t, Vector2 tileOffset, Vector2 testOffset, Vector2 angle)
        {
            float tmpAngle = (float)Math.Atan2((angle - position).Y, (angle - position).X);
            return TryCollision(t, tileOffset, testOffset, tmpAngle);
        }
        public bool TryCollision(Tile t, Vector2 tileOffset, Vector2 testOffset, float angle)
        {
            if (!CollisionRect.isNear(cr, t.cr)) return false;
            foreach (Tile myTile in tiles)
                if (myTile.TryCollision(t, testOffset, tileOffset, angle))
                    return true;
            return false;
        }
        public bool TryCollision(Actor act, Vector2 offsetActor,Vector2 globalOffset)
        {
            if (!CollisionRect.isNear(cr, act.cr)) return false;
            foreach (Tile t in act.tiles)
                if (TryCollision(t, act.position + offsetActor + globalOffset, position + globalOffset, act.angle))
                    return true;
            return false;
        }
    }
}
