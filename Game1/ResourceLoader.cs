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
    class ResourceLoader
    {
        public Dictionary<String, Tile> Sprites;
        public Dictionary<String, TileMap> Maps;
        public List<Actor> Persons;
        public Settings settings;
        Tile badSprite;
        public Stage CreateStage(SpriteBatch sb,string mapName)
        {
            if (settings.width<=0) settings.width = sb.GraphicsDevice.DisplayMode.Width;
            if (settings.height<=0) settings.height = sb.GraphicsDevice.DisplayMode.Height;
            TileMap tm = this.Maps[mapName];
            Stage stg = new Stage(sb,tm.WidthTile,tm.HeightTile, new Rectangle(settings.x,settings.y,settings.width,settings.height));
            stg.Background = tm;
            stg.addActor(this.Persons);
            return stg;
        }

        public ResourceLoader(String ResourceFileName)
        {
            if (!System.IO.File.Exists(ResourceFileName))
                throw new GameException("Game resource '" + ResourceFileName + "' not found", GameException.LevelCatastrophic);
            Sprites = new Dictionary<string, Tile>();
            Persons = new List<Actor>();
            Maps = new Dictionary<string, TileMap>();
            badSprite = new Tile();
            Lua l = new Lua();
            LuaInterfacer lint = new LuaInterfacer();
            l["Simple"] = lint;
            l.DoFile(ResourceFileName);
            this.settings = lint.set;
            this.Sprites = lint.tileList;
            this.Persons = lint.GetActorList();
            this.Maps = lint.mapList;
        }
        public class LuaInterfacer
        {
            public Dictionary<String, Tile> tileList = new Dictionary<string, Tile>();
            public Dictionary<String, TileMap> mapList = new Dictionary<string, TileMap>();
            public Dictionary<String, Actor> actorList = new Dictionary<string, Actor>();
            public Settings set;

            public List<Actor> GetActorList()
            {
                List<Actor> actors = new List<Actor>();
                foreach (KeyValuePair<string, Actor> kvp in actorList)
                    actors.Add(kvp.Value);
                return actors;
            }

            public Tile CreateTile(string name,string filename,int frameWidth = -1)
            {
                Tile t;
                if (frameWidth == -1)
                    t = new Tile(filename);
                else
                    t = new Tile(filename, null, frameWidth);
                t.name = name;
                tileList[name] = t;
                return t;
            }

            public Settings Settings()
            {
                return set;
            }

            public TileMap CreateMap(string name, Tile defaultTile,int width, int height)
            {
                TileMap tm = new TileMap(defaultTile,width,height);
                mapList[name] = tm;
                return tm;
            }

            public Actor CreateActor(string name, Tile fromTile,int startX, int startY)
            {
                Actor act = new Actor(name, fromTile,startX,startY);
                actorList[name] = act;
                return act;
            }
        }
    }
}
