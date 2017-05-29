﻿using System;
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
            primitives.Add(new Sphere(.5f, new Vector3(0, 0, 2), new Vector3(255, 0, 0)));

            primitives.Add(new Plane(new Vector3(0, 1, 0), 0, 0, new Vector3(0, -2, 0), new Vector3(255, 255, 255)));
            
            primitives.Add(new Plane(new Vector3(1, 0, 0), 0, 0, new Vector3(-.5f, 0, 0), new Vector3(1, 1, 5), true));

            // Add lightsources
            //lightsources.Add(new LightSource(new Vector3(0, 5, 0), new Vector3(1, 1, 1), 100));
        }
    }
}
