using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Template
{

    class Raytracer
    {
        // member variables
        public Surface screen;
        public Vector3 screenCorner0, screenCorner1, screenCorner2;
        
        public Camera camera;
        public Scene scene;

        public int CameraX = 767;
        public int CameraZ = 500;

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
                    if (y == 256 && x % 10 == 0)
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
                        DrawDebugRay(new Ray(ray.D + ray.O, ray.D, ray.t - 1), new Vector3(0, 255, 0));

                }
                return Vector3.Zero; // Zwart.
            }

            if (I.p.Mirror)
            {
                if (recursion < 0)
                {
                    return Vector3.Zero;
                }
                // Methode om een ray te reflecteren.
                return Trace(Reflect(ray, I), debug, recursion -1) * I.p.color;
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
                if (debug)
                {
                    if (recursion == maxRecursion)
                        DrawDebugRay(new Ray(ray.D + ray.O, ray.D, I.d - 1), new Vector3(255, 255, 0));
                    else
                        DrawDebugRay(new Ray(ray.D + ray.O, ray.D, I.d- 1), new Vector3(0, 255, 0));

                }
                return DirectIllumination(I, debug) * I.p.color;
            }

        }

        public Ray Reflect(Ray ray, Intersection I)
        {
            Vector3 temp = ray.D - 2 * I.n * ((I.n.X * ray.D.X) + (I.n.Y * ray.D.Y) + (I.n.Z * ray.D.Z));
            return new Ray(I.i, temp, ray.t);
        }

        public Ray Refract(Ray ray, Intersection I, float N1, float N2)
        {
            float angle1 = (((I.n.X * ray.D.X) + (I.n.Y * ray.D.Y) + (I.n.Z * ray.D.Z)) / (I.n.Length * ray.D.Length));

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
            foreach (LightSource light in scene.lightsources)
            {
                // L is shadowray
                Vector3 L = light.Position - I.i;
                float distance = L.Length;
                L.Normalize();
                if(debug)
                    DrawDebugRay(new Ray(I.i, L, distance), new Vector3(200, 200, 200));
                // check of de lightsource visible is, zo niet, return zwart
                if (!light.IsVisible(I, scene.primitives)) continue; // Zwart.
                float attenuation = light.Intensity / (distance * distance);
                float NdotL = (I.n.X * L.X) + (I.n.Y * L.Y) + (I.n.Z * L.Z);
                if (NdotL < 0) continue;
                // Dotproduct of the normal of the intersection and shadowray
                illumination = light.Color * attenuation * NdotL;
                continue;
            }

            return illumination; 

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
            screen.Line(CameraX + (int)(screenCorner0.X)*48, CameraZ - (int)(screenCorner0.Z)*48,
                CameraX + (int)(screenCorner1.X)*48, CameraZ - (int)(screenCorner1.Z)*48, FixColor(new Vector3(255, 255, 255)));

            List<Sphere> spheres = new List<Sphere>();
            foreach(Primitive p in scene.primitives)
            {
                if (p.type == Type.Sphere)
                    spheres.Add((Sphere)p);
            }
            float angle = 2 * (float)Math.PI / 100;
            foreach (Sphere s in spheres)
            {
                
                float newradius = (float)Math.Sqrt((s.Radius * s.Radius) - (s.Origin.Y * s.origin.Y) - (camera.cameraPosition.Y * camera.cameraPosition.Y));
                if(newradius > 0)
                {
                    for (int a = 0; a < 100; a++)
                    {
                        // Draws a linepiece between two circlepoints.
                        screen.Line(ConverttoDebugX((float)(s.origin.X + newradius * Math.Cos(a * angle))),
                            ConverttoDebugY((float)(s.origin.Z + newradius * Math.Sin(a * angle))),
                            ConverttoDebugX((float)(s.origin.X + newradius * Math.Cos((a + 1) * angle))),
                            ConverttoDebugY((float)(s.origin.Z + newradius * Math.Sin((a + 1) * angle))), FixColor(s.color));
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
    }

} // namespace Template