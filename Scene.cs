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

            SelectionSort(0, primitives.Count, pos);

        }
        
        public void SelectionSort(int p, int q, Vector3 pos)
        {
            for (int i = p; i < q; i++)
            {
                int k = i;
                for (int j = i + 1; j < q; j++)
                {
                    Vector3 vecj = primitives[j].Origin - pos;
                    Vector3 veck = primitives[k].Origin - pos;
                    if(vecj.Length < veck.Length)
                            k = j;
                }
                Switch(i, k);
            }
        }
        
        public void Switch(int a, int b)
        {
            Primitive p = primitives[a];
            primitives[a] = primitives[b];
            primitives[b] = p;
        }
    }
}
}
