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
            Vector3 Sposition = new Vector3(5, 5, 5);
            Vector3 Scolor = new Vector3(10000, 1, 1);
            Sphere sphere = new Sphere(10, Sposition, Scolor);
            primitives.Add(sphere);

            // Add lightsources
            Vector3 Lposition = new Vector3(5, 20, 5);
            Vector3 Lcolor = new Vector3(1, 1, 1);
            LightSource light = new LightSource(Lposition, Lcolor, 1);
            lightsources.Add(light);
        }
    }
}
