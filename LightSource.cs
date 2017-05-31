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
<<<<<<< HEAD
        // In lightsource gebeurd vrij weinig. Dit bevat alleen informatie en had achteraf waarschijnlijk een struct kunnen zijn.
        // We're in too deep now...
=======
        // positie, kleur en intensiteit van de lightsource
>>>>>>> refs/remotes/origin/master
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
