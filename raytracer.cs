﻿using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;

namespace Template
{

    class Raytracer
    {
        // member variables
        public Surface screen;
        public Vector3 screenCorner0, screenCorner1, screenCorner2;
        
        Camera camera;
        Scene scene;

        Surface pattern;

        // distance from camera to screen (change FOV by changing distance)
        public int distance = 1;
        int maxRecursion = 10;
        // initialize
        public void Init()
        {

            // the camera from where you see the scene
            camera = new Camera(new Vector3(0, 0, 0), new Vector3(0, 0, 1));
            
            scene = new Scene(camera.CameraPosition);

            screen = new Surface(1024, 512);

            // the corners of the screen
            screenCorner0 = camera.CameraPosition + distance * camera.CameraDirection + new Vector3(-1, -1, 0);
            screenCorner1 = camera.CameraPosition + distance * camera.CameraDirection + new Vector3(1, -1, 0);
            screenCorner2 = camera.CameraPosition + distance * camera.CameraDirection + new Vector3(-1, 1, 0);

            pattern = new Surface("../../assets/pattern.png");
            Render();
        }
        // tick: renders one frame (isn't being used)
        public void Tick()
        {
            screen.Clear(0);
            screen.Print("hello world", 2, 2, 0xffffff);
            screen.Line(2, 20, 160, 20, 0xff0000);
        }

        float debugraylength;
        public void Render()
        {
            DrawDebug();
            // Dit rendert de hele scene. 
            int i = 0;
            for (int y = 0; y < 512; y++)
            {
                for (int x = 0; x < 512; x++)
                {
                    // De rays zijn opgeslagen in een array in camera. 
                    Ray ray = camera.SendRay(i);
                    Vector3 vector = Trace(ray, 0);
                    if (y == 256 && x % 10 == 0)
                        DrawDebugRay(new Ray(ray.O, ray.D, debugraylength));
                    screen.Plot(x, y, FixColor(vector));
                    
                    i++;
                }
                
            }
        }

        // De trace functie van de slides.
        // TO DO: Recursie cappen.
        Vector3 Trace(Ray ray, int recursion)
        {
            Intersection I = SearchIntersect(ray);

            if (I.p == null)
            {
                debugraylength = ray.t; 
                return Vector3.Zero; // Zwart.
            }
            Vector3 color = I.p.color;
            if (I.p.Mirror)
            {
                if (recursion < maxRecursion)
                {
                    return Trace(Reflect(ray, I), recursion + 1) * color;
                }
                return new Vector3(1, 1, 1);
                // Methode om een ray te reflecteren.
            }
            /*
            // Dielectric means glass/any seethrough material, appearently...
            // TO DO: isDielectric bool of float bij Primitives.
            
            else if (I.p.DiElectric)
            {
                // TO DO: Fresnel formule toevoegen. 
                float f = Fresnel();
                // TO DO: Methode om een ray te refracteren.
                return (f * Trace(Reflect(ray, I)) + (1 - f) * Trace(Refract(ray, I))) * I.p.color;
            }
            */
            else
            {
                debugraylength = I.d;
                if (I.p is Plane)
                    color = CreatePattern(I.i, pattern);
                return DirectIllumination(I) * color;
            }


        }

        public Ray Reflect(Ray ray, Intersection I)
        {
            Ray seccondaryRay = new Ray();
            seccondaryRay.D = ray.D - ((2 * I.n) * (Vector3.Dot(ray.D, I.n)));
            seccondaryRay.O = I.i;
            return seccondaryRay;
        }

        public Ray Refract(Ray ray, Intersection I)
        {
            return new Ray();
        }

        // TO DO: Dit per lightpoint doen en de waarden meegeven.
        Vector3 DirectIllumination(Intersection I)
        {
            Vector3 illumination = new Vector3(0, 0, 0);
            Ray shadowRay = new Ray();
            foreach (LightSource light in scene.lightsources)
            {
                // create a shadowray
                shadowRay.D = light.Position - I.i;
                shadowRay.O = I.i;
                shadowRay.t = shadowRay.D.Length;
                shadowRay.D.Normalize();

                // check of de lightsource visible is, zo niet, return zwart
                if (!IsVisible(I, shadowRay)) continue; // Zwart.

                float attenuation = light.Intensity / (shadowRay.t * shadowRay.t);
                float NdotL = Vector3.Dot(I.n, shadowRay.D);
                if (NdotL < 0) continue;
                // Dotproduct of the normal of the intersection and shadowray
                illumination = light.Color * attenuation * NdotL;
                continue;
            }

            return illumination; 
        }

        // Checks to see if the line between the light source and a point is unobstructed.
        public bool IsVisible(Intersection I, Ray shadowRay)
        {
            float tMin = int.MaxValue;
            foreach (Primitive p in scene.primitives)
            {
                float t = p.Intersect(shadowRay).d;
                if (t > 0 && t < tMin)
                    tMin = t;
            }

            if (tMin >= shadowRay.t)
                return true;
            else
                return false;
        }

        // Vind de primitive waarmee de ray intersect. Als er niets wordt gevonden returnt het een lege intersection. 
        Intersection SearchIntersect(Ray ray)
        {
            foreach (Primitive p in scene.primitives)
            {
                if (p.Intersect(ray).p != null)
                {
                    return p.Intersect(ray);
                }
            }

            return new Intersection();
        }

        void DrawDebugRay(Ray ray)
        {
            screen.Line(ConverttoDebugX(camera.CameraPosition.X),
                ConverttoDebugY(camera.CameraPosition.Z),
                ConverttoDebugX(ray.D.X),
                ConverttoDebugY(ray.D.Z), FixColor(new Vector3(255, 0, 0)));

            screen.Line(ConverttoDebugX(ray.D.X),
                ConverttoDebugY(ray.D.Z),
                ConverttoDebugX(ray.D.X * ray.t),
                ConverttoDebugY(ray.D.Z * ray.t), FixColor(new Vector3(255, 255, 0)));
        }

        void DrawDebug()
        {
            // Maakt de camera & screen aan in de debugwindow. 
            screen.Plot(767, 500, FixColor(new Vector3(255,255,255)));
            screen.Line(ConverttoDebugX(screenCorner0.X), ConverttoDebugY(screenCorner0.Z), 
                ConverttoDebugX(screenCorner1.X), ConverttoDebugY(screenCorner1.Z), FixColor(new Vector3(255, 255, 255)));

            List<Sphere> spheres = new List<Sphere>();
            foreach(Primitive p in scene.primitives)
            {
                if (p.type == Type.Sphere)
                    spheres.Add((Sphere)p);
            }
            float angle = 2 * (float)Math.PI / 100;
            foreach (Sphere s in spheres)
            {
                
                float newradius = (float)Math.Sqrt((s.Radius * s.Radius) - (s.Origin.Y * s.origin.Y));
                
                for(int a = 0; a < 100; a++)
                {
                    // Draws a linepiece between two circlepoints.
                    screen.Line(ConverttoDebugX((float)(s.origin.X + newradius * Math.Cos(a * angle))), 
                        ConverttoDebugY((float)(s.origin.Z + newradius * Math.Sin(a * angle))),
                        ConverttoDebugX((float)(s.origin.X + newradius * Math.Cos((a + 1) * angle))), 
                        ConverttoDebugY((float)(s.origin.Z + newradius * Math.Sin((a + 1) * angle))), FixColor(s.color));
                }
            }
        }
        
        int ConverttoDebugX(float x)
        {
            return (int)(767 + (x * 48));
        }

        int ConverttoDebugY(float z)
        {
            return (int)(500 - (z * 48));
        }

        public int FixColor(Vector3 color)
        {
            if (color.X > 255) color.X = 255;
            if (color.Y > 255) color.Y = 255;
            if (color.Z > 255) color.Z = 255;
            return ((int)color.X << 16) + ((int)color.Y << 8) + (int)(color.Z);
        }

        public Vector3 CreatePattern(Vector3 P, Surface T)
        {
            Vector2 point = new Vector2(P.X, P.Z);
            int x = (int)Math.Round(point.X * T.width - 0.5);

            //keep x within the range of the surface, as the plane is infinite
            while (x < 0)
            {
                x += T.width;
            }
            while (x >= T.width)
            {
                x -= T.width;
            }

            int y = (int)Math.Round(point.Y * T.height - 0.5);
            //keep y within the range of the surface, as the plane is infinite
            while (y < 0)
            {
                y += T.height;
            }
            while (y >= T.height)
            {
                y -= T.height;
            }

            Color col = pattern.bmp.GetPixel(x, y);
            return new Vector3(col.R, col.G, col.B);
        }
    }

} // namespace Template