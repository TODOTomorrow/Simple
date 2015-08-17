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
        public event onCollisionEvent onCollision;
        int width;
        int height;
        Tile[,] data;
        SpriteBatch sb;
        Tile mainTile;
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
            int startX = (int)(x / mainTile.Width);
            int startY = (int)(y / mainTile.Height);
            int endX = (int)((x + width) / mainTile.Width + 1);
            int endY = (int)((y + height) / mainTile.Height + 1);
            for (int i = startY; i < endY && i < this.height; i++)
                for (int j = startX; j < endX && j < this.width; j++)
                    data[i, j].Draw(x, y);
        }

        public void Update(GameTime gt)
        {

        }

        public bool TryCollision(Actor act, Vector2 newPos)
        {
            foreach (Tile t in data)
                if (act.TryCollision(t, new Vector2(0, 0), newPos))
                    return true;
            return false;
        }

        public bool Collision(Actor act)
        {
            foreach (Tile t in data)
                if (act.Collision(t, new Vector2(0, 0), true))
                {
                    if (onCollision != null) onCollision(this, act);
                    return true;
                }
            return false;
        }
    }
}
