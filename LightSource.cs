using System;
using OpenTK;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    public class LightSource
    {
        // Note: onzeker of Vector3 of float3 moet worden gebruikt. 
        Vector3 position;
        Vector3 color;
        float intensity;

        public LightSource(Vector3 position, Vector3 color, float intensity)
        {
            this.position = position;
            this.color = color;
            this.intensity = intensity;
        }

        // Checks to see if the line between the light source and a point is unobstructed.
        public bool IsVisible(Vector3 origin, List<Primitive> primitives)
        {

            // To do: Move the point of the light source in the direction of the ray's origin to prevent shadow acne.

            // Creates a ray between this lightsource and given origin.
            Vector3 direction = origin - position;
            float distance = direction.Length;
            direction.Normalize();
            Ray ray = new Ray(origin, direction, distance);

            // To do: intersection. Primitives are necessary for this.
            foreach (Primitive p in primitives)
            {
                p.Intersect(ray);
                if (p.Intersect(ray).p == null)
                    return true;
            }
                return false;
        }

        public Vector3 Position
        {
            get { return position; }
        }

        public Vector3 Color
        {
            get { return color; }
        }

        public float Intensity
        {
            get { return intensity; }
        }
    }
}
