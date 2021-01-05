using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Doodle
{
    class Vector
    {
        public static Vector zeroVector = new Vector(0, 0);
        public float x { get; set; }
        public float y { get; set; }

        public Vector(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector(Vector vector)
        {
            this.x = vector.x;
            this.y = vector.y;
        }

        public Vector Normalize()
        {
            return this * (float)(1 / Math.Sqrt(x * x + y * y));
        }

        public static Vector CreateRandom(float minX, float maxX, float minY, float maxY)
        {
            Random rand = new Random();

            float x = (float)rand.NextDouble() * (maxX - minX) + minX;
            float y = (float)rand.NextDouble() * (maxY - minY) + minY;

            return new Vector(x, y);
        }

        public static Vector CreateRandom(float x , float y)
        {
            return CreateRandom(x, x, y, y);
        }

        public static Vector FromPolar(float r, float theta)
        {
            float x = r * (float)Math.Cos(theta);
            float y = -r * (float)Math.Sin(theta);

            return new Vector(x, y);
        }


        public static Vector operator +(Vector v1, Vector v2)
        {
            return new Vector(v1.x + v2.x , v1.y + v2.y);
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector(v1.x - v2.x, v1.y - v2.y);
        }

        public static Vector operator *(float k ,Vector v)
        {
            return new Vector(k * v.x , k* v.y);
        }
        public static Vector operator *(Vector v, float k)
        {
            return new Vector(k * v.x, k * v.y);
        }

        public static Vector operator *(Vector v1, Vector v2)
        {
            return new Vector(v1.x * v2.x, v1.y * v2.y);
        }

    }
}