using System;
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
        public bool dielectric;

        public Primitive(Vector3 o, Vector3 c)
        {
            origin = o;
            color = c;
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

        /*public bool isMirror
        {
            get { return mirror; }
        }

        public bool isDiElectric
        {
            get { return dielectric; }
        }*/
    }

    class Sphere : Primitive
    {
        float radius;

        public Sphere(float r, Vector3 o, Vector3 c)
            : base(o, c)
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
            Vector3 i = (ray.t * ray.D) + ray.O;    // Punt van de intersection.
            Vector3 normal = i - origin;            // Normaal van intersection en Sphere.
            normal.Normalize();
            return new Intersection(this, ray.t, normal, i);
        }

        public float Radius
        {
            get{ return radius; }
        }
    }

    // Onzeker of de width en height nodig zijn hiervoor. 
    class Plane : Primitive
    {
        Vector3 normal;
        float width, height;
        public Plane(Vector3 n, float w, float h, Vector3 o, Vector3 c)
            :base(o, c)
        {
            normal = n;
            width = w;
            height = h;
            type = Type.Plane;
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

        public Intersection planeIntersect(Ray ray)
        {
            if (Dotproduct3D(Normal, ray.D) == 0)
            {
                //als geldt dat de dotproduct = 0, en hij ligt precies op de plane, dan is de afstand dus 0.
                if (Dotproduct3D((origin - ray.O), Normal) == 0)
                {
                    ray.t = 0;
                }

                //als dotproduct = 0, en hij ligt niet op de plane, dan heeft hij geen enkele intersection en moet er een nieuwe worden gereturned.
                else
                {
                    return new Intersection();
                }
            }

            //is de dotproduct niet 0, dan is er dus een intersectie met de plane, hiervoor formule van de slides invullen voor t; t is dan de afstand.
            else if (Dotproduct3D(Normal, ray.D) != 0)
            {
                ray.t = ((Dotproduct3D((origin - ray.O), Normal)) / (Dotproduct3D((origin - ray.O), Normal)));
            }

            Vector3 i = (ray.O + (ray.t * ray.D)); //dit maakt een vector van ((ray.O.X + ray.t * ray.D.X), (ray.O.Y + ray.t * ray.D.Y), (ray.O.Z + ray.t * ray.D.Z))
            Vector3 normal = i - origin;
            normal.Normalize();

            return new Intersection(this, ray.t, normal, i);
        }
    }
}
