using System;
using System.Collections.Generic;
using OpenTK;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    class Scene
    {
        // Lijsten
        public List<Primitive> primitives;
        public List<LightSource> lightsources;
        
        public Scene(Vector3 pos)
        {
            // Initialiseert lijsten.
            primitives = new List<Primitive>();
            lightsources = new List<LightSource>();

            // Add primitives....

            Vector3 Sposition = new Vector3(0, 0, 8);
            Vector3 Scolor = new Vector3(255, 0, 0);
            Sphere sphere = new Sphere(3, Sposition, Scolor);

            primitives.Add(sphere);

            // Add lightsources
            Vector3 Lposition = new Vector3(0, 30, 8);
            Vector3 Lcolor = new Vector3(255, 255, 0);
            LightSource light = new LightSource(Lposition, Lcolor, 1);
            lightsources.Add(light);
        }
    }
}
