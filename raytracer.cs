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
                    screen.Plot(x, y, (int)FixColor(vector));
                    i++;
                }

            }
        }

        // De trace functie van de slides.
        // TO DO: Recursie cappen.
        Vector3 Trace(Ray ray)
        {
            Intersection I = SearchIntersect(ray);
            if (I.p == null) return Vector3.Zero; // Zwart.
            // TO DO: isMirror bool of float bij Primitives.
            /*
            if (I.p.isMirror())
            {
                // TO DO: Methode om een ray te reflecteren.
                return Trace(I, reflect(ray)) * I.p.color;
            }
            */
            // Dielectric means glass/any seethrough material, appearently...
            // TO DO: isDielectric bool of float bij Primitives.
            /*
            else if (I.p.isDielectric())
            {
                // TO DO: Fresnel formule toevoegen. 
                float f = Fresnel();
                // TO DO: Methode om een ray te refracteren.
                return (f * Trace(I, reflect(ray)) + (1 - f) * Trace(I, refract(ray, ))) * I.p.color;
            }
            */
            else
            {
                return DirectIllumination(I) * I.p.color;
            }

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
                    return p.Intersect(ray);
            }

            return new Intersection();
        }

        public float FixColor(Vector3 color)
        {
            return ((int)color.X << 16) + ((int)color.Y << 8) + (color.Z);
        }
    }

} // namespace Template