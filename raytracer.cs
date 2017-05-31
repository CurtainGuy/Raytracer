using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;

namespace Template
{

    class Raytracer
    {
        // member variables
        public Surface screen;
        
        public Camera camera;
        public Scene scene;

        public int CameraX = 767;
        public int CameraZ = 500;

        Surface pattern;
        Surface sky;

        // distance from camera to screen (change FOV by changing distance)
        public float fov = 90;
        int maxRecursion = 10;
        // initialize
        public void Init()
        {

            // the camera from where you see the scene
            camera = new Camera(new Vector3(0, 0, 0), new Vector3(0, 0, 1), fov);
            
            scene = new Scene(camera.CameraPosition);

            screen = new Surface(1024, 512);
            

            pattern = new Surface("../../assets/pattern.png");
            sky = new Surface("../../assets/stpeters_probe.png");
            Render();
        }
        // tick: renders one frame
        public void Tick()
        {
            DrawDebug();
            
            //camera coordinates
            screen.Print("Cameraposition:" + camera.cameraPosition, 512, 490, 0xffffff);
        }
        Vector3[] colors;
        public void Render()
        {
            colors = new Vector3[512 * 512];
            // Dit rendert de hele scene. 
            int i = 0;
            for (int y = 0; y < 512; y++)
            {
                for (int x = 0; x < 512; x++)
                {
                    // De rays zijn opgeslagen in een array in camera. 
                    Ray ray = camera.SendRay(i);
                    if (y == 256 && x % 20 == 0)
                    {
                        colors[i] = Trace(ray, true, maxRecursion);
                        DrawDebugRay(new Ray(ray.O, ray.D, 1), new Vector3(255,0,0));
                    }
                    else
                        colors[i] = Trace(ray, false, maxRecursion);


                    i++;
                }
                
            }
            DrawDebug();
            i = 0;
            for (int y = 0; y < 512; y++)
            {
                for (int x = 0; x < 512; x++)
                {
                    screen.Plot(x, y, FixColor(colors[i]));
                    i++;
                }
            }
        }

        // De trace functie van de slides.
        // TO DO: Recursie cappen.
        Vector3 Trace(Ray ray, bool debug, int recursion)
        {
            Intersection I = SearchIntersect(ray);

            if (I.p == null)
            {
                if (debug)
                {
                    if (recursion == maxRecursion)
                        DrawDebugRay(new Ray(ray.D + ray.O, ray.D, ray.t - 1), new Vector3(255, 255, 0));
                    else
                        DrawDebugRay(ray, new Vector3(0, 255, 0));

                }
                return CreateSkyDome(ray); // SkyDome
            }
            Vector3 color = I.p.color;
            if (I.p.Mirror)
            {
                

                if (debug)
                {
                    if (recursion == maxRecursion)
                        DrawDebugRay(new Ray(ray.D + ray.O, ray.D, I.d - 1), new Vector3(255, 255, 0));
                    else
                        DrawDebugRay(new Ray(ray.O, ray.D, I.d), new Vector3(0, 255, 0));
                }
                if (recursion < 0)
                {
                    return Vector3.Zero;
                }
                // Methode om een ray te reflecteren.
                return (Trace(Reflect(ray, I), debug, recursion - 1) * color);
            }
                // Methode om een ray te reflecteren.
            else
            {
                if (debug)
                {
                    if (recursion == maxRecursion)
                        DrawDebugRay(new Ray(ray.D + ray.O, ray.D, I.d - 1), new Vector3(255, 255, 0));
                    else
                        DrawDebugRay(new Ray(ray.O, ray.D, I.d), new Vector3(0, 255, 0));
                }
                if (I.p is Plane)
                    return (DirectIllumination(I, debug) * CreatePattern(I.i));
                
                return (DirectIllumination(I, debug) * color);
            }
        }

        public Ray Reflect(Ray ray, Intersection I)
        {
            Ray seccondaryRay = new Ray();
            seccondaryRay.D = ray.D - ((2 * I.n) * (Vector3.Dot(ray.D, I.n)));
            seccondaryRay.O = I.i;
            seccondaryRay.t = int.MaxValue;
            return seccondaryRay;
        }

        public Ray Refract(Ray ray, Intersection I, float N1, float N2)
        {
            float angle1 = Vector3.Dot(I.n, ray.D) / (I.n.Length * ray.D.Length);

            //N = 1;       brekingsindex van lucht
            //N= (3 / 2); brekingsindex van glass
            //N = (4 / 3); brekingsindex van water

            float angle2 = (float)Math.Asin((N1 / N2) * Math.Sin(angle1));

            Vector3 refractive = -I.n * angle2;
            return new Ray(I.i, refractive, ray.t);
        }
        
        Vector3 DirectIllumination(Intersection I, bool debug)
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
                if(debug)
                    DrawDebugRay(new Ray(I.i, shadowRay.D, shadowRay.t), new Vector3(200, 200, 200));
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
            float tMin = int.MaxValue;
            Primitive intersected = null;
            foreach (Primitive p in scene.primitives)
            {
                float t = p.Intersect(ray).d;
                if (t > 0 && t < tMin)
                {
                    tMin = t;
                    intersected = p;
                }
            }
            if (intersected == null)
                return new Intersection();
            return new Intersection(intersected, tMin, intersected.Intersect(ray).n, intersected.Intersect(ray).i);
        }

        // Tekent een ray op de debug met de line segmenten.
        public void DrawDebugRay(Ray ray, Vector3 color )
        {
            screen.Line(ConverttoDebugX(ray.O.X),
                ConverttoDebugY(ray.O.Z),
                ConverttoDebugX((ray.D.X * ray.t) + ray.O.X),
                ConverttoDebugY((ray.D.Z * ray.t) + ray.O.Z), FixColor(color));
        }

        // Tekent de primitives, camera en screen.
        void DrawDebug()
        {            
            // Maakt de camera & screen aan in de debugwindow. 
            screen.Plot(CameraX, CameraZ, FixColor(new Vector3(255,255,255)));
            screen.Line(CameraX + (int)(camera.screenCorner0.X)*48, CameraZ - 48 - (int)(camera.screenCorner0.Z)*48,
                CameraX + (int)(camera.screenCorner1.X)*48, CameraZ - 48 - (int)(camera.screenCorner1.Z)*48, FixColor(new Vector3(255, 255, 255)));

            List<Sphere> spheres = new List<Sphere>();
            foreach(Primitive p in scene.primitives)
            {
                if (p.type == Type.Sphere)
                    spheres.Add((Sphere)p);
            }
            float angle = 2 * (float)Math.PI / 100;
            foreach (Sphere s in spheres)
            {
                Vector3 c = s.color;
                if (s.Mirror)
                    c = new Vector3(240,255,255);

                float newradius = (float)Math.Sqrt((s.Radius * s.Radius) - ((s.Origin.Y - camera.cameraPosition.Y) * (s.Origin.Y - camera.cameraPosition.Y)));
                if(newradius > 0)
                {
                    for (int a = 0; a < 100; a++)
                    {
                        // Draws a linepiece between two circlepoints.
                        screen.Line(ConverttoDebugX((float)(s.origin.X + newradius * Math.Cos(a * angle))),
                            ConverttoDebugY((float)(s.origin.Z + newradius * Math.Sin(a * angle))),
                            ConverttoDebugX((float)(s.origin.X + newradius * Math.Cos((a + 1) * angle))),
                            ConverttoDebugY((float)(s.origin.Z + newradius * Math.Sin((a + 1) * angle))), FixColor(c));
                    }
                }
            }
        }
        // Zetten de world-coördinaten om in schermcoördinaten. Gebruikt 767 en 500 
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

        public Vector3 CreatePattern(Vector3 P)
        {
            Vector2 point = new Vector2(P.X, P.Z);
            int x = (int)Math.Round(point.X * pattern.width - 0.5);

            //keep x within the range of the surface, as the plane is infinite
            while (x < 0)
            {
                x += pattern.width;
            }
            while (x >= pattern.width)
            {
                x -= pattern.width;
            }

            int y = (int)Math.Round(point.Y * pattern.height - 0.5);
            //keep y within the range of the surface, as the plane is infinite
            while (y < 0)
            {
                y += pattern.height;
            }
            while (y >= pattern.height)
            {
                y -= pattern.height;
            }

            Color col = pattern.bmp.GetPixel(x, y);
            return new Vector3(col.R, col.G, col.B);
        }

        public Vector3 CreateSkyDome(Ray ray)
        {
            float r = (float)((1 / Math.PI) * Math.Acos(ray.D.Z) / Math.Sqrt(ray.D.X * ray.D.X + ray.D.Y * ray.D.Y + 1));
            //Console.WriteLine(r);
            float x = MathHelper.Clamp(((ray.D.X * r + 1) * sky.width / 2), 0, sky.width - 1);
            float y = MathHelper.Clamp(((ray.D.Y * r + 1) * sky.height / 2), 0, sky.height - 1);
            Color col = sky.bmp.GetPixel((int)x, (int)y);
            return new Vector3(col.R, col.G, col.B);
        }
    }

} // namespace Template