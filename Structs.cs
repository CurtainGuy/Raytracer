using System;
using OpenTK;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    public struct Ray
    {
        public Vector3 O; // ray origin
        public Vector3 D; // ray direction
        public float t;   // distance
        public Ray(Vector3 O, Vector3 D, float t)
        {
            this.O = O;
            this.D = D;
            this.t = t;
        }
    }
}
