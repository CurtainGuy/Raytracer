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

            // the screen where the rays are shot at
            screen = new Surface(512, 512);
            // the corners of the screen
            screenCorner0 = camera.CameraPosition + distance * camera.CameraDirection + new Vector3(-1, -1, 0);
            screenCorner1 = camera.CameraPosition + distance * camera.CameraDirection + new Vector3(1, -1, 0);
            screenCorner2 = camera.CameraPosition + distance * camera.CameraDirection + new Vector3(-1, 1, 0);
        }
        // tick: renders one frame
        public void Tick()
        {
            screen.Clear(0);
            screen.Print("hello world", 2, 2, 0xffffff);
            screen.Line(2, 20, 160, 20, 0xff0000);
        }

        public void Render()
        {
            // Dit rendert de hele scene. 
            int i = 0;
            for (int y = 0; y < 255; y++)
            {
                for (int x = 0; x < 255; x++)
                {
                    // De rays zijn opgeslagen in een array in camera. 
                    Ray ray = camera.SendRay(i);
                    Vector3 vector = Trace(ray);
                    // TO DO: zet de kleurvectoren in de screen.
                    i++;
                }
            }
        }

        // De trace functie van de slides.
        // TO DO: Recursie cappen.
        Vector3 Trace(Ray ray)
        {
            Intersection I = SearchIntersect(ray);
            if (I.p != null) return Vector3.Zero; // Zwart.
            // TO DO: isMirror bool of float bij Primitives.
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
            else
            {
                return DirectionIllumination(I) * I.p.color;
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
            
        }

        // TO DO: Dit per lightpoint doen en de waarden meegeven.
        Vector3 DirectIllumination(Intersection I)
        {
            Vector3 L = lightPos - I.i;
            float distance = L.Length;
            L *= (1.0f / distance);

            // TO DO: Check of het visible is door middel van shadow rays in de IsVisible methode. 
            if (!IsVisible(I, L, distance)) return Vector3.Zero; // Zwart.
            float attenuation = 1 / (distance * distance);

            // Dotproduct of the normal of the intersection and L.
            return lightColor * attenuation * (I.n.X * L.X) + (I.n.Y * L.Y) + (I.n.Z * L.Z); 

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
                    return p.Intersect(ray);
            }

            return new Intersection();
        }
    }

} // namespace Template