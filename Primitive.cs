﻿using System;
using OpenTK;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{

    public enum Type
    {
        Sphere,
        Plane
    }

    public class Primitive
    {
        public Vector3 origin;
        public Vector3 color;
        public Type type;
        public bool mirror;
        public float absorption, reflection;

        public Primitive(Vector3 o, Vector3 c, bool m = false, float ab = 0)
        {
            origin = o;
            color = c;
            mirror = m;
            absorption = ab;
            reflection = 1 - absorption;
        }

        // kleine functie voor het berekenen van een dotproduct in 3D.
        public float Dotproduct3D(Vector3 vec1, Vector3 vec2)
        {
            float sum = (vec1.X * vec2.X) + (vec1.Y * vec2.Y) + (vec1.Z * vec2.Z);
            return sum;
        }

        // Zie Structs klasse voor uitleg over Intersection.
        public virtual Intersection Intersect(Ray ray)
        {
            return new Intersection();
        }

        public Vector3 Origin
        {
            get { return origin; }
        }

        public bool Mirror
        {
            get { return mirror; }
        }
    }

    class Sphere : Primitive
    {
        float radius;

        public Sphere(float r, Vector3 o, Vector3 c, bool m = false, float ab = 0)
            : base(o, c, m, ab)
        {
            radius = r;
            type = Type.Sphere;
        }

        // Werkt alleen voor buiten de sphere.
        // Zie Structs klasse voor uitleg over Intersection.
        public override Intersection Intersect(Ray ray)
        {
            // De Intersection met een sphere volgens de slides.
            Vector3 c = Origin - ray.O;
            float t = Dotproduct3D(c, ray.D);
            Vector3 p = c - t * ray.D;
            float p_squared = Dotproduct3D(p, p);
            float r_squared = radius * radius;
            if (p_squared > r_squared) return new Intersection();
            t -= (float)Math.Sqrt(r_squared - p_squared);
            if ((t < ray.t) && (t > 0)) ray.t = t;

            // Omdat we een intersection willen returnen moeten we nog wat extra waarden vinden.
            Vector3 i = (ray.t * ray.D) + ray.O;          // Punt van de intersection.
            Vector3 normal = 2 * (i - origin);            // Normaal van intersection en Sphere.
            normal.Normalize();
            return new Intersection(this, ray.t, normal, i);
        }

        public float Radius
        {
            get { return radius; }
        }
    }
    
    class Plane : Primitive
    {
        Vector3 normal;
        public Plane(Vector3 n, Vector3 o, Vector3 c, bool m = false, float ab = 0)
            : base(o, c, m, ab)
        {
            normal = n;
            type = Type.Plane;
        }

        public Vector3 Normal
        {
            get { return normal; }
        }

        public override Intersection Intersect(Ray ray)
        {
            ray.t = -1 * (Dotproduct3D(ray.O, Normal) + origin.Length) / Dotproduct3D(ray.D, Normal);

            if (ray.t > 0)
            {
                Vector3 i = (ray.O + (ray.t * ray.D)); //dit maakt een vector van ((ray.O.X + ray.t * ray.D.X), (ray.O.Y + ray.t * ray.D.Y), (ray.O.Z + ray.t * ray.D.Z))
                

                return new Intersection(this, ray.t, Normal, i);
            }
            else
                return new Intersection();
        }
    }
}
