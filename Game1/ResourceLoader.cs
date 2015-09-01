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
    class ResourceLoader
    {
        Game1 parent;
        public Dictionary<String, Tile> Sprites;
        public Dictionary<String, TileMap> Maps;
        public List<Actor> Persons;
        public Settings settings;
        Tile badSprite;
        void settingsParse(XmlDocument xmlDc)
        {
            settings = new Settings();
            settings.options = new Dictionary<string, string>();
            if (xmlDc.SelectNodes("Resource/Settings").Count == 0)
            {
                Game1.dbg.Log("Settings not defined!");
                settings.x = 0;
                settings.y = 0;
                settings.width = -1;
                settings.height = -1;
                return;
            }
            foreach (XmlNode child in xmlDc.SelectNodes("Resource/Settings")[0].ChildNodes)
            {
                switch (child.Name.ToLower())
                {
                    case "width":
                        try { settings.width = Convert.ToInt32(child.InnerText); }
                        catch { Game1.dbg.Log("Incorrect screen width"); }
                        break;
                    case "height":
                        try { settings.height = Convert.ToInt32(child.InnerText); }
                        catch { Game1.dbg.Log("Incorrect screen height"); }
                        break;
                    case "x":
                        try { settings.x = Convert.ToInt32(child.InnerText); }
                        catch { Game1.dbg.Log("Incorrect screen x pos"); }
                        break;
                    case "y":
                        try { settings.y = Convert.ToInt32(child.InnerText); }
                        catch { Game1.dbg.Log("Incorrect screen y pos"); }
                        break;
                    default:
                        settings.options.Add(child.Name, child.InnerText);
                        break;
                }
            }
        }

        void personsParse(XmlDocument xmlDc)
        {
            foreach (XmlNode node in xmlDc.SelectNodes("Resource/Person"))
            {
                string name = "";
                List<Tile> tiles = new List<Tile>();
                Rectangle pos = new Rectangle();
                float angle = 0;
                Dictionary<string, string> opts = new Dictionary<string, string>();
                foreach (XmlNode child in node.ChildNodes)
                {
                    child.InnerText = child.InnerText.Trim().ToLower();
                    switch (child.Name.ToLower())
                    {
                        case "name":
                            name = child.InnerText;
                            break;
                        case "sprite":
                            tiles.Add(getSprite(child.InnerText));
                            break;
                        case "x":
                            try { pos.X = Convert.ToInt32(child.InnerText); }
                            catch { Game1.dbg.Log("Incorrect person x pos"); }
                            break;
                        case "y":
                            try { pos.Y = Convert.ToInt32(child.InnerText); }
                            catch { Game1.dbg.Log("Incorrect person y pos"); }
                            break;
                        case "width":
                            try { pos.Width = Convert.ToInt32(child.InnerText); }
                            catch { Game1.dbg.Log("Incorrect person width"); }
                            break;
                        case "height":
                            try { pos.Height = Convert.ToInt32(child.InnerText); }
                            catch { Game1.dbg.Log("Incorrect person height"); }
                            break;
                        case "rotated":
                            try { angle = Convert.ToInt32(child.InnerText); }
                            catch { Game1.dbg.Log("Sprite incorrect height"); }
                            angle = angle * (float)Math.PI / 180;
                            break;
                        default:
                            opts.Add(child.Name, child.InnerText);
                            break;
                    }
                }
                Actor act = new Actor(pos, name, angle);
                act.AddTile(tiles);
                Persons.Add(act);
            }

        }

        void spriteParse(XmlDocument xmlDc)
        {
            foreach (XmlNode node in xmlDc.SelectNodes("Resource/Sprite"))
            {
                Dictionary<string, string> opts = new Dictionary<string, string>();
                string name = "";
                List<string> fileName = new List<string>();
                int width = -1;
                int height = -1;
                int x = -1;
                int y = -1;
                bool passable = true;
                float angle = 0;
                foreach (XmlNode child in node.ChildNodes)
                {
                    child.InnerText = child.InnerText.Trim();
                    switch (child.Name.ToLower())
                    {
                        case "name":
                            name = child.InnerText.ToLower();
                            break;
                        case "file":
                            fileName.Add(child.InnerText);
                            break;
                        case "width":
                            try { width = Convert.ToInt32(child.InnerText); }
                            catch { Game1.dbg.Log("Sprite incorrect width"); }
                            break;
                        case "height":
                            try { height = Convert.ToInt32(child.InnerText); }
                            catch { Game1.dbg.Log("Sprite incorrect height"); }
                            break;
                        case "passable":
                            passable = (child.InnerText == "true") ? true : false;
                            break;
                        case "rotated":
                            try { angle = Convert.ToInt32(child.InnerText); }
                            catch { Game1.dbg.Log("Sprite incorrect height"); }
                            angle = angle * (float)Math.PI / 180;
                            break;
                        default:
                            opts.Add(child.Name, child.InnerText);
                            break;
                    }
                }
                if (name == "") { Game1.dbg.Log("Sprite defined, but not described."); continue; }
                //if (fileName == "") { Game1.dbg.Log("Sprite not associated with any image file"); continue; }
                if (fileName.Count == 0) { Game1.dbg.Log("Sprite not associated with any image file"); continue; }
                Tile t = new Tile(fileName[0]);
                t.name = name;
                t.passable = passable;
                t.Resize(width, height);
                t.Rotate(angle);
                foreach (string fn in fileName)
                    t.AddImg(fn);
                Sprites.Add(t.name, t);
            }
        }

        public Tile getSprite(string name)
        {
            if (!Sprites.ContainsKey(name))
                Game1.dbg.Log("Map incorrect height", GameException.LevelCatastrophic);
            return Sprites[name];
        }

        void mapParse(XmlDocument xmlDc)
        {
            foreach (XmlNode node in xmlDc.SelectNodes("Resource/Map"))
            {
                Dictionary<string,string> opts = new Dictionary<string, string>();
                int width = -1;
                int height = -1;
                string name = "";
                Tile defaultSprite = badSprite;
                List<mapObject> objs = new List<mapObject>();
                foreach (XmlNode child in node.ChildNodes)
                {
                    child.InnerText = child.InnerText.Trim();
                    switch (child.Name.ToLower())
                    {
                        case "name":
                            name = child.InnerText;
                            break;
                        case "width":
                            try { width = Convert.ToInt32(child.InnerText); }
                            catch { Game1.dbg.Log("Map incorrect width"); }
                            break;
                        case "height":
                            try { height = Convert.ToInt32(child.InnerText); }
                            catch { Game1.dbg.Log("Map incorrect height"); }
                            break;
                        case "maintilename":
                            defaultSprite = getSprite(child.InnerText.ToLower());
                            break;
                        default:
                            if (child.Name != "object")
                                opts.Add(child.Name, child.InnerText);
                            break;
                    }
                }
                int counter = 0;

                foreach (XmlNode objNode in xmlDc.SelectNodes("Resource/MapObject"))
                {
                    mapObject mobj = new mapObject();
                    foreach (XmlNode childObj in objNode.ChildNodes)
                    {
                        childObj.InnerText = childObj.InnerText.Trim().ToLower();
                        switch (childObj.Name.ToLower())
                        {
                            case "name":
                                mobj.name = childObj.InnerText;
                                break;
                            case "sprite":
                                mobj.sprite = getSprite(childObj.InnerText);
                                break;
                            case "x":
                                try { mobj.x = Convert.ToInt32(childObj.InnerText); }
                                catch { Game1.dbg.Log("Map incorrect object x"); }
                                break;
                            case "y":
                                try { mobj.y = Convert.ToInt32(childObj.InnerText); }
                                catch { Game1.dbg.Log("Map incorrect object y"); }
                                break;
                        }
                    }
                    if (mobj.name == null) mobj.name = "Anonym" + counter;
                    objs.Add(mobj);
                }
                if (width < 0 || height < 0)
                    Game1.dbg.Log("Map width or height is incorrect", GameException.LevelCatastrophic);
                if (defaultSprite.Equals(badSprite))
                    Game1.dbg.Log("Default sprite not defined", GameException.LevelCatastrophic);
                TileMap tm = new TileMap(defaultSprite,width,height);
                for (int i = 0; i < objs.Count; i++)
                {
                    if (objs[i].x < 0 || objs[i].y < 0)
                        Game1.dbg.Log("Object coordinates incorrect", GameException.LevelCatastrophic);
                    tm.AddObject(objs[i].sprite,new Vector2(objs[i].x,objs[i].y));
                }
                Maps.Add(name, tm);
            }
        }
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
            XmlDocument xmlDc = new XmlDocument();
            xmlDc.Load(ResourceFileName);
            spriteParse(xmlDc);
            mapParse(xmlDc);
            settingsParse(xmlDc);
            personsParse(xmlDc);
        }
    }
}
