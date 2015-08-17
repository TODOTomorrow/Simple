using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    class GameResources
    {
        Game1 parent;
        public Dictionary<String, Sprite> Sprites;
        public Dictionary<String, Map> Maps;
        public Dictionary<String, Person> Persons;
        public Settings settings;
        Sprite badSprite;
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
                Person p = new Person();
                p.options = new Dictionary<string, string>();
                p.pos = new Vector2();
                foreach (XmlNode child in node.ChildNodes)
                {
                    child.InnerText = child.InnerText.Trim().ToLower();
                    switch (child.Name.ToLower())
                    {
                        case "name":
                            p.name = child.InnerText;
                            break;
                        case "sprite":
                            p.sprite = getSprite(child.InnerText);
                            break;
                        case "x":
                            try { p.pos.X = Convert.ToInt32(child.InnerText); }
                            catch { Game1.dbg.Log("Incorrect person x pos"); }
                            break;
                        case "y":
                            try { p.pos.Y = Convert.ToInt32(child.InnerText); }
                            catch { Game1.dbg.Log("Incorrect person y pos"); }
                            break;
                        case "width":
                            try { p.width = Convert.ToInt32(child.InnerText); }
                            catch { Game1.dbg.Log("Incorrect person width"); }
                            break;
                        case "height":
                            try { p.height = Convert.ToInt32(child.InnerText); }
                            catch { Game1.dbg.Log("Incorrect person height"); }
                            break;
                        default:
                            p.options.Add(child.Name, child.InnerText);
                            break;
                    }
                }
                if (p.width < 0) p.width = p.sprite.width;
                if (p.height < 0) p.height = p.sprite.height;
                Persons.Add(p.name, p);
            }

        }

        void spriteParse(XmlDocument xmlDc)
        {
            foreach (XmlNode node in xmlDc.SelectNodes("Resource/Sprite"))
            {
                Sprite s = new Sprite();
                foreach (XmlNode child in node.ChildNodes)
                {
                    child.InnerText = child.InnerText.Trim();
                    switch (child.Name.ToLower())
                    {
                        case "name":
                            s.name = child.InnerText.ToLower();
                            break;
                        case "file":
                            s.fileName = child.InnerText;
                            s.img = parent.Content.Load<Texture2D>(s.fileName);
                            break;
                        case "width":
                            try { s.width = Convert.ToInt32(child.InnerText); }
                            catch { Game1.dbg.Log("Sprite incorrect width"); }
                            break;
                        case "height":
                            try { s.height = Convert.ToInt32(child.InnerText); }
                            catch { Game1.dbg.Log("Sprite incorrect height"); }
                            break;
                        case "passable":
                            s.passable = (child.InnerText == "true") ? true : false;
                            break;
                        default:
                            s.options.Add(child.Name, child.InnerText);
                            break;
                    }
                }
                if (s.name == "") { Game1.dbg.Log("Sprite defined, but not described."); continue; }
                if (s.img == null) { Game1.dbg.Log("Sprite not associated with any image file"); continue; }
                if (s.width <= 0) s.width = s.img.Width;
                if (s.height <= 0) s.height = s.img.Height;
                Sprites.Add(s.name, s);
            }
        }

        public Sprite getSprite(string name)
        {
            if (!Sprites.ContainsKey(name))
                Game1.dbg.Log("Map incorrect height", GameException.LevelCatastrophic);
            return Sprites[name];
        }

        void mapParse(XmlDocument xmlDc)
        {
            Maps = new Dictionary<string, Map>();
            foreach (XmlNode node in xmlDc.SelectNodes("Resource/Map"))
            {
                Map m = new Map();
                m.options = new Dictionary<string, string>();
                int width = -1;
                int height = -1;
                Sprite defaultSprite = badSprite;
                List<mapObj> objs = new List<mapObj>();
                foreach (XmlNode child in node.ChildNodes)
                {
                    child.InnerText = child.InnerText.Trim();
                    switch (child.Name.ToLower())
                    {
                        case "name":
                            m.name = child.InnerText;
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
                                m.options.Add(child.Name, child.InnerText);
                            break;
                    }
                }
                int counter = 0;

                foreach (XmlNode objNode in xmlDc.SelectNodes("Resource/object"))
                {
                    mapObj mobj = new mapObj();
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
                m.data = new Sprite[width, height];
                for (int i = 0; i < height; i++)
                    for (int j = 0; j < width; j++)
                        m.data[i, j] = defaultSprite.Copy();
                m.defaultSprite = defaultSprite;
                for (int i = 0; i < objs.Count; i++)
                {
                    if (objs[i].x < 0 || objs[i].y < 0)
                        Game1.dbg.Log("Object coordinates incorrect", GameException.LevelCatastrophic);
                    m.data[objs[i].y, objs[i].x] = objs[i].sprite.Copy();
                }
                m.width = width;
                m.height = height;
                Maps.Add(m.name, m);
            }
        }
        public GameResources(String ResourceFileName, Game1 g)
        {
            if (!System.IO.File.Exists(ResourceFileName))
                throw new GameException("Game resource '" + ResourceFileName + "' not found", GameException.LevelCatastrophic);
            Sprites = new Dictionary<string, Sprite>();
            Persons = new Dictionary<string, Person>();
            parent = g;
            badSprite = new Sprite();
            XmlDocument xmlDc = new XmlDocument();
            xmlDc.Load(ResourceFileName);
            spriteParse(xmlDc);
            mapParse(xmlDc);
            settingsParse(xmlDc);
            personsParse(xmlDc);
        }
    }
}
