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
   /* class ResourceLoader
    {
        public Dictionary<String, Tile> Sprites;
        public Dictionary<String, TileMap> Maps;
        public List<Actor> Persons;
        public Settings settings;
        Tile badSprite;


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
    }*/
        class LuaInterfacer
        {
            public Dictionary<String, Tile> tileList = new Dictionary<string, Tile>();
            public Dictionary<String, TileMap> mapList = new Dictionary<string, TileMap>();
            public Dictionary<String, Actor> actorList = new Dictionary<string, Actor>();
            public Settings set;
            public Lua luaContext = new Lua();
            private Dictionary<string, List<LuaFunction>> luaEvents;

            public void Raise(string eventName, params object[] parameters)
            {
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
            public void DoFile(string fileName)
            {
                if (!System.IO.File.Exists(fileName))
                    throw new GameException("Game resource '" + fileName + "' not found", GameException.LevelCatastrophic);
                luaContext.DoFile(fileName);
            }
            public LuaInterfacer()
            {
                
                luaEvents = new Dictionary<string, List<LuaFunction>>();
                luaContext["Simple"] = this;
                luaContext["ActorMoved"] = ActorState.Move;
                return;
            }

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


            int stageNumber = 0;
            public Stage CreateStage(SpriteBatch sb, string mapName)
            {
                if (set.width <= 0) set.width = sb.GraphicsDevice.DisplayMode.Width;
                if (set.height <= 0) set.height = sb.GraphicsDevice.DisplayMode.Height;
                TileMap tm = this.mapList[mapName];
                Stage stg = new Stage(sb, tm.WidthTile, tm.HeightTile, new Rectangle(set.x, set.y, set.width, set.height));
                stg.Background = tm;
                stg.addActor(GetActorList());
                luaContext["stage" + stageNumber] = stg;
                return stg;
            }

            public void SetGlobal(string name,object obj)
            {
                luaContext[name] = obj;
            }

            public void Log(params object[] objs)
            {
                foreach (object obj in objs)
                    Console.WriteLine(obj.ToString());
            }
        }
    
}
