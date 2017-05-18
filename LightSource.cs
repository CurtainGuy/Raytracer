﻿using System;
using OpenTK;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    class LightSource
    {
        // Note: onzeker of Vector3 of float3 moet worden gebruikt. 
        Vector3 position;
        float intensity;

        public LightSource(Vector3 position, float intensity)
        {
            this.position = position;
            this.intensity = intensity;
        }

        // Checks to see if the line between the light source and a point is unobstructed.
        public bool IsVisible(Vector3 origin)
        {
            
            // To do: Move the point of the light source in the direction of the ray's origin to prevent shadow acne.

            // Creates a ray between this lightsource and given origin.
            Vector3 direction = origin - position;
            float distance = direction.Length;
            direction.Normalize();
            Ray ray = new Ray(origin, direction, distance);

            // To do: intersection. Primitives are necessary for this.
            if (true)
                return false;
            else
                return true;
        }

        public Vector3 Position
        {
            get { return position; }
        }

        public float Intensity
        {
            get { return intensity; }
        }

    }
}