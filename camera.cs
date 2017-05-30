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
        public Vector3 screenCorner0;
        public Vector3 screenCorner1;
        public Vector3 screenCorner2;
        public Vector3[] screen;
        public float screendistance;
        Ray ray;

        public Camera(Vector3 position, Vector3 direction, float fov)
        {
            cameraPosition = position;
            cameraDirection = direction;
            screen = new Vector3[512 * 512];

            float fieldofview = (float)(fov * Math.PI / 360);
            screendistance = (float)(1 / (Math.Tan(fieldofview)));

            //corners van de camera
            screenCorner0 = cameraPosition + screendistance * CameraDirection + new Vector3(-1, -1, 0);
            screenCorner1 = cameraPosition + screendistance * CameraDirection + new Vector3(1, -1, 0);
            screenCorner2 = cameraPosition + screendistance * CameraDirection + new Vector3(-1, 1, 0);

            ScreenSetup();
        }

        // Dit werkt nu alleen voor het kijken in de z-richting.
        // TO DO: Zorg dat het elke kant op kan kijken. 
        // TO DO: Zorg dat het op elke positie werkt.
        void ScreenSetup()
        {
            
            float multiplierx = (screenCorner1.X - screenCorner2.X) / 512.0000f;
            float multipliery = (screenCorner1.Y - screenCorner2.Y) / 512.0000f;
            float multiplierz = (screenCorner1.Z - screenCorner2.Z) / 512.0000f;
            int i = 0;
            for(float y = 0; y < 512; y++)
            {
                for(float x = 0; x < 512; x++)
                {
                    // De 1 bij z * de afstand tussen de camera en het scherm.
                    // Dit berekent de normals tussen de camera en de pixels van het scherm.
                    screen[i] = new Vector3(multiplierx * x - screenCorner1.X, multipliery *  y - screenCorner1.Y, 1) - cameraPosition;
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
            cameraPosition = cameraPosition + new Vector3(x / 48, y / 48, z / 48);
            //screenCorner0 += new Vector3(0.05f * x, 0.05f * y, 0.05f * z);
            //screenCorner1 += new Vector3(0.05f * x, 0.05f * y, 0.05f * z);
            //screenCorner2 += new Vector3(0.05f * x, 0.05f * y, 0.05f * z);
        }
    }
}
