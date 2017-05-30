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
        public Vector3 cameraPosition, cameraDirection;
        Vector3 screenCorner0;
        Vector3 screenCorner1;
        Vector3 screenCorner2;
        Vector3[] screen;
        Ray ray;

        public Camera(Vector3 position, Vector3 direction)
        {
            cameraPosition = position;
            cameraDirection = direction;
            screen = new Vector3[512 * 512];

            //corners van de camera
            int distance = 1;
            screenCorner0 = cameraPosition + distance * CameraDirection + new Vector3(-1, -1, 0);
            screenCorner1 = cameraPosition + distance * CameraDirection + new Vector3(1, -1, 0);
            screenCorner2 = cameraPosition + distance * CameraDirection + new Vector3(-1, 1, 0);

            ScreenSetup();
        }

        // Dit werkt nu alleen voor het kijken in de z-richting.
        // TO DO: Zorg dat het elke kant op kan kijken. 
        // TO DO: Zorg dat het op elke positie werkt.
        void ScreenSetup()
        {
            // Eerste 1 is de maximale x van de screen, 2e is de minimale x.
            float multiplier = (1 - -1) / 512.0000f;
            int i = 0;
            for(float y = 0; y < 512; y++)
            {
                for(float x = 0; x < 512; x++)
                {
                    // De 1tjes bij x en y zijn x1, ofwel minimale x van de screen. 
                    // De 1 bij z * de afstand tussen de camera en het scherm.
                    // Dit berekent de normals tussen de camera en de pixels van het scherm.
                    screen[i] = new Vector3(multiplier * x - 1.0000f, multiplier * -1.0000f * y + 1.0000f, 1.0000f) - cameraPosition;
                    screen[i].Normalize();
                    i++;
                }
                
            }
        }

        // Maakt een ray uit de lijst van normals per pixel.
        public Ray SendRay(int i)
        {
            // Momenteel is 5 de lengte van de ray als placeholder.
            return ray = new Ray(cameraPosition, screen[i], 100);
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

        public void CameraTransform(float x, float y, float z)
        {
            //nieuwe camerapositie
            cameraPosition = cameraPosition + new Vector3(0.05f * x, 0.05f * y, 0.05f * z);
            //screenCorner0 += new Vector3(0.05f * x, 0.05f * y, 0.05f * z);
            //screenCorner1 += new Vector3(0.05f * x, 0.05f * y, 0.05f * z);
            //screenCorner2 += new Vector3(0.05f * x, 0.05f * y, 0.05f * z);
        }
    }
}
