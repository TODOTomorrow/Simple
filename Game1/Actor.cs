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
    delegate void ActorEvent(Actor act);
    enum ActorState { Stop, Move }
    class Actor : IDrawable
    {
        public string name;
        public SpriteBatch spriteBatch { set { this.sb = value; foreach (Tile t in tiles) t.spriteBatch = value; } }
        public Vector2 position;
        public float speed = 0.4f;
        public event onCollisionEvent onCollision;
        public ActorEvent onMoveEnd;
        public ActorState State { get { return state; } }
        float angle;
        ActorState state = ActorState.Stop;
        SpriteBatch sb;
        List<Tile> tiles;
        Rectangle profile;
        private double dxSpeed;
        private double dySpeed;
        private Vector2 endPoint = new Vector2();
        private bool moveToLeft, moveUp;
        public void Rotate(Vector2 to)
        {
            angle = (float)Math.Atan2((to - position).Y, (to - position).X);
            foreach (Tile t in tiles)
                t.Rotate(angle);
        }
        public void Stop()
        {
            state = ActorState.Stop;
        }
        public Actor(Rectangle profile, string name = "")
        {
            this.profile = profile;
            this.name = name;
            this.position = new Vector2();
            tiles = new List<Tile>();
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
                t.Draw(position.X, position.Y);
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
                endPoint.X = float.NaN;
                endPoint.Y = float.NaN;
                state = ActorState.Stop;
            }
        }
        public void Update(GameTime gt)
        {
            UpdatePosition(gt.ElapsedGameTime.Milliseconds);
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
            foreach (Tile myTile in tiles)
                if (myTile.Collision(t, this.position, tileOffset))
                {
                    if (onCollision != null && collisionEventFlag) onCollision(this, t);
                    return true;
                }
            return false;
        }

        public bool TryCollision(Tile t, Vector2 tileOffset, Vector2 testOffset)
        {
            foreach (Tile myTile in tiles)
                if (myTile.TryCollision(t, testOffset, tileOffset))
                    return true;
            return false;
        }
    }
}
