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
    class TileSet : Tile
    {
        SpriteBatch sb;
        List<Tile> tiles;
        int tileNum;
        public override void Draw(float x, float y, int width = 0, int height = 0)
        {
            Random r = new Random();
            tileNum = r.Next(tiles.Count - 1);
            tiles[tileNum].position = base.position;
            tiles[tileNum].Draw(x, y, width, height);
        }
        public void AddImage(string imageName)
        {
            Tile t = new Tile(imageName, sb);
            tiles.Add(t);
            t.Resize();
        }
        public TileSet(string imageName, SpriteBatch sb = null) : base(imageName,sb)
        {
            tiles = new List<Tile>();
            this.sb = sb;
            tiles.Add(new Tile(imageName, sb));
        }
    }
}
