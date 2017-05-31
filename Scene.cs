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

            // Sphere
            primitives.Add(new Sphere(.5f, new Vector3(-1, -1.5f, 4), new Vector3(255, 1, 1)));

            // Mirror
            primitives.Add(new Sphere(.5f, new Vector3(1, -1.5f, 4), new Vector3(1, 1, 1), true, 0.05f));
            primitives.Add(new Sphere(.5f, new Vector3(0, -1.5f, 4), new Vector3(1, 50, 1), true, 0.98f));

            // Floor
            primitives.Add(new Plane(new Vector3(0, 1, 0), 0, 0, new Vector3(0, -2, 0), new Vector3(255, 255, 255)));
            
            // Add lightsources
            lightsources.Add(new LightSource(new Vector3(0, 5, 2), new Vector3(1, 1, 1), 10));
            lightsources.Add(new LightSource(new Vector3(3, 0, 0), new Vector3(1, 2, 1), 25));
            lightsources.Add(new LightSource(new Vector3(-.5f, -1f, 3), new Vector3(2, 1, 1), 5));
        }
    }
}
