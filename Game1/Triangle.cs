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
    public class Triangle : ICloneable
    {
        public event onCollisionEvent onCollision;
        Vector2[] points = new Vector2[3];
        Vector2[] rotatedPoints = new Vector2[3];
        float angle;
        CollisionRect cr;
        public void Move(Vector2 to)
        {
            for (int i=0;i<points.Length;i++)
                points[i]+=to;
        }
        public Triangle(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            points[0] = p1;
            points[1] = p2;
            points[2] = p3;
            cr = new CollisionRect(int.MaxValue,int.MaxValue);
        }
        public static double Area(Vector2 a, Vector2 b, Vector2 c)
        {
            return Math.Abs((a.X - c.X) * (b.Y - c.Y) + (b.X - c.X) * (c.Y - a.Y));
        }
        public double Area(Vector2 offset)
        {
            Vector2 a = points[0];
            Vector2 b = points[1];
            Vector2 c = points[2];
            return Math.Abs((a.X - c.X) * (b.Y - c.Y) + (b.X - c.X) * (c.Y - a.Y));
        }
        public bool Collision(Vector2 p, Vector2 thisOffset, Vector2 offset)
        {
            double d = Area(thisOffset);
            double trA = Triangle.Area(points[0] + thisOffset, points[1] + thisOffset, p + offset);
            double trB = Triangle.Area(points[0] + thisOffset, p + offset, points[2] + thisOffset);
            double trC = Triangle.Area(points[1] + thisOffset, p + offset, points[2] + thisOffset);
            double sum = trA + trB + trC;
            if (d >= sum || (trA == 0 && trB == 0) || (trA == 0 && trC == 0) || (trB == 0 && trC == 0))
            {
                if (onCollision != null) onCollision(this, p);
                return true;
            }
            else return false;
        }

        public bool Collision(Triangle t, Vector2 thisOffset, Vector2 offset)
        {
            if (Collision(t.points[0], thisOffset, offset) ||
                Collision(t.points[1], thisOffset, offset) ||
                Collision(t.points[2], thisOffset, offset))
            {
                if (onCollision != null) onCollision(this, t);
                return true;
            }
            else return false;
        }

        public Object Clone()
        {
            Triangle t = new Triangle(points[0], points[1], points[2]);
            return t;
        }

        public void Rotate(float angle,Vector2 offset)
        {
            this.angle = angle;
            for (int i = 0; i < rotatedPoints.Length; i++)
            {
                double x = (double)points[i].X;
                double x0 = offset.X;
                double y = points[i].Y;
                double y0 = offset.Y;
                points[i].X =  (float)(x0 + (x - x0) * Math.Cos(angle) - (y - y0) * Math.Sin(angle));
                points[i].Y = (float)(y0 + (y - y0) * Math.Cos(angle) + (x - x0) * Math.Sin(angle));
            }
        }

        public CollisionRect Reindex(Vector2 offset, Rectangle collisionRectangle)
        {
            //foreach (Vector2 p in points)
            //{
                cr.x = (int)((offset + points[0]).X / collisionRectangle.Width);
                cr.y = (int)((offset + points[0]).Y / collisionRectangle.Height);
            //}
            return cr;
        }
    }
}
