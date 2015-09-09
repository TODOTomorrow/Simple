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
    
    class TileMap : IDrawable
    {
        public SpriteBatch spriteBatch { set { this.sb = value; foreach (Tile t in data) t.spriteBatch = value; } }
        public int WidthTile { get { return width; } }
        public int HeightTile { get { return height; } }
        public int WidthPix { get { return width * mainTile.Width; } }
        public int HeightPix { get { return height * mainTile.Height; } }
        public int MaxTileWidth { get { int w = int.MinValue; foreach (Tile t in data) if (t.Width > w) w = t.Width; return w; } }
        public int MaxTileHeight { get { int h = int.MinValue; foreach (Tile t in data) if (t.Height > h) h = t.Height; return h; } }
        public event onCollisionEvent onCollision;
        int width;
        int height;
        Tile[,] data;
        SpriteBatch sb;
        Tile mainTile;
        public void Reindex(Vector2 offset, Rectangle collisionRect)
        {
            foreach (Tile t in data)
                t.Reindex(offset, collisionRect);
        }
        public TileMap(Tile defaultTile, int width, int height, SpriteBatch sb = null)
        {
            this.width = width;
            this.height = height;
            this.data = new Tile[height, width];
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    data[i, j] = (Tile)defaultTile.Clone();
                    data[i, j].position.Y = i * defaultTile.Height;
                    data[i, j].position.X = j * defaultTile.Width;
                }
            this.sb = sb;
            this.mainTile = defaultTile;
        }
        public void AddObject(Tile t, int x, int y)
        {
            AddObject(t, new Vector2(x, y));
        }
        public void AddObject(Tile t, Vector2 pos)
        {
            data[(int)pos.Y, (int)pos.X] = (Tile)t.Clone();
            data[(int)pos.Y, (int)pos.X].position.X = pos.X * mainTile.Width;
            data[(int)pos.Y, (int)pos.X].position.Y = pos.Y * mainTile.Height;
        }

        public void Draw(float x, float y, int width = 0, int height = 0)
        {
            if (width == 0) width = this.width;
            if (height == 0) height = this.height;
            int startX = (int)(-x / mainTile.Width);
            int startY = (int)(-y / mainTile.Height);
            if (startX > 0) startX--;
            if (startY > 0) startY--;
            int endX = (int)((-x + width) / mainTile.Width + 1);
            int endY = (int)((-y + height) / mainTile.Height + 1);
            for (int i = startY; i < endY && i < this.height; i++)
                for (int j = startX; j < endX && j < this.width; j++)
                {
                    data[i, j].Draw(x, y);
                }
        }

        public void Update(GameTime gt)
        {

        }

        public bool TryCollision(Actor act, Vector2 newPos,Vector2 globalOffset = new Vector2(),float angle = float.NaN)
        {
            foreach (Tile t in data)
            {
                if (act.TryCollision(t, globalOffset, newPos+globalOffset, (float.IsNaN(angle)) ? act.angle : angle))
                    return true;
            }
            return false;
        }

        public bool Collision(Actor act,Vector2 offset)
        {
            foreach (Tile t in data)
                if (CollisionRect.isNear(act.cr,t.cr) &&
                    act.Collision(t, offset, true))
                {
                    if (onCollision != null) onCollision(this, act);
                    return true;
                }
            return false;
        }

    }
}
