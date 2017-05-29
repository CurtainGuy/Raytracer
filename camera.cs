using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Template
{
    public class Camera
    {
        Vector3 cameraPosition, cameraDirection;
        Vector3[] screen;
        Ray ray;

        public Camera(Vector3 position, Vector3 direction)
        {
            cameraPosition = position;
            cameraDirection = direction;
            screen = new Vector3[512 * 512];
            
            ScreenSetup();
        }

        // Dit werkt nu alleen voor het kijken in de z-richting.
        // TO DO: Zorg dat het elke kant op kan kijken. 
        // TO DO: Zorg dat het op elke positie werkt.
        void ScreenSetup()
        {
            // Eerste 1 is de maximale x van de screen, 2e is de minimale x.
            float multiplier = (1 - -1) / 512;
            int i = 0;
            for(int y = 0; y < 512; y++)
            {
                for(int x = 0; x < 512; x++)
                {
                    // De 1tjes bij x en y zijn x1, ofwel minimale x van de screen. 
                    // De 1 bij z * de afstand tussen de camera en het scherm.
                    // Dit berekent de normals tussen de camera en de pixels van het scherm.
                    screen[i] = new Vector3(multiplier * x + 1, multiplier * y - 1, 1) - cameraPosition;
                    screen[i].Normalize();
                    i++;
                }
            }
        }

        // Maakt een ray uit de lijst van normals per pixel.
        public Ray SendRay(int i)
        {
            // Momenteel is 5 de lengte van de ray als placeholder.
            return ray = new Ray(cameraPosition, screen[i], int.MaxValue);
        }
        
        public Vector3 CameraPosition
        {
            get{ return cameraPosition; }
            set{ cameraPosition = value; }
        }

        public Vector3 CameraDirection
        {
            get { return cameraDirection; }
            set { cameraDirection = value; }
        }
    }
}
