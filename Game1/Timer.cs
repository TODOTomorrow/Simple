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
    
    class Timer
    {
        public TimeSpan elapsedTotal;
        List<TimerEventStruct> events = new List<TimerEventStruct>();

        public Timer()
        {
            elapsedTotal = new TimeSpan();
            SomeTimer = this;
        }

        public void SetTimer(TimeSpan after, TimerEvent te)
        {
            TimerEventStruct evs = new TimerEventStruct();
            evs.time = after + elapsedTotal;
            evs.ev += te;
            for (int i = 0; i < events.Count; i++)
                if (events[i].time == (after + elapsedTotal))
                {
                    events[i].ev += te;
                    return;
                }
            events.Add(evs);
        }
        public void Update(GameTime gt)
        {
            elapsedTotal = gt.TotalGameTime;
            for (int i = 0; i < events.Count; i++)
                if (events[i].time <= gt.TotalGameTime)
                {
                    events[i].generate();
                    events.Remove(events[i]);
                }
        }

        public static Timer SomeTimer;
    }
    public delegate void TimerEvent();
    public struct TimerEventStruct
    {
        public TimeSpan time;
        public event TimerEvent ev;
        public void generate()
        {
            ev();
        }
        public override bool Equals(object obj)
        {
            base.Equals(obj);
            TimerEventStruct es = (TimerEventStruct)obj;
            if (time == es.time) return true;
            else return false;
        }
    }
}
