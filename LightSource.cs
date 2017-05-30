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
