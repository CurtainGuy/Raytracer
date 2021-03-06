﻿using System;
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

    // Intersection houdt een aantal gegevens bij als het intersect met een primitive. 
    // Het is leeg als er geen intersection is.
    public struct Intersection
    {
        public Primitive p; // Primitive waarme er wordt intersected.
        public float d;     // Afstand tussen origin en intersection.
        public Vector3 n;   // Normal van de intersected primitive.
        public Vector3 i;   // Coördinaten van de intersection.
        public Intersection(Primitive p, float d, Vector3 n, Vector3 i)
        {
            this.p = p;
            this.d = d;
            this.n = n;
            this.i = i;
        }
    }
}
