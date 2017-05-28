using System;
using OpenTK;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    public class Primitive
    {
        Vector3 origin;

        public Primitive(Vector3 o)
        {
            origin = o;
        }

        public float Dotproduct3D(Vector3 vec1, Vector3 vec2)
        {
            float sum = (vec1.X * vec2.X) + (vec1.Y * vec2.Y) + (vec1.Z * vec2.Z);
            return sum;
        }

        public Vector3 Origin
        {
            get { return origin; }
        }
    }

    class Sphere : Primitive
    {
        float radius;

        public Sphere(float r, Vector3 o)
            : base(o)
        {
            radius = r;
        }

        void OuterIntersect(Ray ray)
        {
            Vector3 c = Origin - ray.O;
            float t = Dotproduct3D(c, ray.D);
            Vector3 p = c - t * ray.D;
            float p_squared = Dotproduct3D(p, p);
            float r_squared = radius * radius;
            if (p_squared > r_squared) return;
            t -= (float)Math.Sqrt(r_squared - p_squared);
            if ((t < ray.t) && (t > 0)) ray.t = t;
        }

        public float Radius
        {
            get{ return radius; }
        }
    }

    class Plane : Primitive
    {
        Vector3 normal;
        float width, height;
        public Plane(Vector3 n, float w, float h, Vector3 o)
            :base(o)
        {
            normal = n;
            width = w;
            height = h;
        }

        public Vector3 Normal
        {
            get{ return normal; }
        }

        public float Width
        {
            get{ return width; }
        }
        public float Height
        {
            get { return height; }
        }
    }
}
