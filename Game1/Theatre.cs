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
    public class TheatreStage
    {
        public Stage value;
        public bool enable;
    }
    public class Theatre
    {
        List<TheatreStage> stages = new List<TheatreStage>();
        public void AddStage(Stage s)
        {
            TheatreStage ts = new TheatreStage();
            ts.value = s;
            ts.enable = true;
            stages.Add(ts);
        }

        public void Disable(Stage s)
        {
            for (int i = 0; i < stages.Count; i++)
                if (stages[i].value == s) stages[i].enable = false;
        }

        public void Enable(Stage s)
        {
            for (int i = 0; i < stages.Count; i++)
                if (stages[i].value == s) stages[i].enable = true;
        }

        public void Draw()
        {
            foreach (TheatreStage s in stages)
            {
                if (s.enable)
                    s.value.Draw();
            }
        }

        public void Update(GameTime gt)
        {
            foreach (TheatreStage s in stages)
            {
                if (s.enable)
                    s.value.Update(gt);
            }
        }
    }
}
