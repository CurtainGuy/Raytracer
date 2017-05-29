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
        
        Camera camera;
        Scene scene;

        // distance from camera to screen (change FOV by changing distance)
        public int distance = 1;
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
                    
                    Vector3 vector = Trace(ray);
                    if (y == 256 && x % 10 == 0)
                        DrawDebugRay(new Ray(ray.O, ray.D, debugraylength));
                    screen.Plot(x, y, FixColor(vector));
                    
                    i++;
                }
                
            }
        }

        // De trace functie van de slides.
        // TO DO: Recursie cappen.
        Vector3 Trace(Ray ray)
        {
            Intersection I = SearchIntersect(ray);
            if (I.p == null)
            {
                debugraylength = ray.t; 
                return Vector3.Zero; // Zwart.
            }
            // TO DO: isMirror bool of float bij Primitives.
            /*
            if (I.p.isMirror())
            {
                // TO DO: Methode om een ray te reflecteren.
                return Trace(Reflect(ray, I)) * I.p.color;
            }
            
            // Dielectric means glass/any seethrough material, appearently...
            // TO DO: isDielectric bool of float bij Primitives.
            
            else if (I.p.isDielectric())
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
                return DirectIllumination(I) * I.p.color;
            }

        }

        public Ray Reflect(Ray ray, Intersection I)
        {
            Vector3 temp = ray.D - 2 * I.n * ((I.n.X * ray.D.X) + (I.n.Y * ray.D.Y) + (I.n.Z * ray.D.Z));
            temp.Normalize();
            return new Ray(I.i, temp, ray.t);
        }

        public Ray Refract(Ray ray, Intersection I)
        {
            return new Ray();
        }

        // TO DO: Dit per lightpoint doen en de waarden meegeven.
        Vector3 DirectIllumination(Intersection I)
        {
            Vector3 illumination = new Vector3(1,1,1);
            foreach (LightSource light in scene.lightsources)
            {
                Vector3 L = light.Position - I.i;
                float distance = L.Length;
                L *= (1.0f / distance);

                // TO DO: Check of het visible is door middel van shadow rays in de IsVisible methode. 
                //if (!Light.IsVisible(I, L, distance)) return Vector3.Zero; // Zwart.
                if (!light.IsVisible(I.i, scene.primitives)) return Vector3.Zero; // Zwart.
                float attenuation = 1 / (distance * distance);
                // Dotproduct of the normal of the intersection and L.
                illumination *= FixColor(light.Color) * attenuation * (I.n.X * L.X) + (I.n.Y * L.Y) + (I.n.Z * L.Z);
            }

            return illumination; 

        }

        // Vind de primitive waarmee de ray intersect. Als er niets wordt gevonden returnt het een lege intersection. 
        Intersection SearchIntersect(Ray ray)
        {
            foreach (Primitive p in scene.primitives)
            {
                p.Intersect(ray); 
            }
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
            return ((int)color.X << 16) + ((int)color.Y << 8) + (int)(color.Z);
        }
    }

} // namespace Template