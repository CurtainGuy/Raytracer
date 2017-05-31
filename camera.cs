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
        // Vectoren die nodig zijn bij het berekenen van de camera en screen.
        public Vector3 cameraPosition, cameraDirection;
        public Vector3 screenCorner0;
        public Vector3 screenCorner1;
        public Vector3 screenCorner2;
        public Vector3[] screen;
        public float screendistance;

        public Camera(Vector3 position, Vector3 direction, float fov)
        {
            cameraPosition = position;
            cameraDirection = direction;

            // Maakt een scherm aan.
            screen = new Vector3[512 * 512];

            // Berekent de afstand van het scherm met de FoV
            float fieldofview = (float)(fov * Math.PI / 360);
            screendistance = (float)(1 / (Math.Tan(fieldofview)));

            // Hoeken van de camera. note: corner2 is linksonder.
            screenCorner0 = cameraPosition + screendistance * CameraDirection + new Vector3(-1, -1, 0);
            screenCorner1 = cameraPosition + screendistance * CameraDirection + new Vector3(1, -1, 0);
            screenCorner2 = cameraPosition + screendistance * CameraDirection + new Vector3(-1, 1, 0);

            // Zet het scherm op. 
            ScreenSetup();
        }


        void ScreenSetup()
        {
            // De grootte van één pixel is multiplierx bij multipliery.
            float multiplierx = (screenCorner1.X - screenCorner2.X) / 512.0000f;
            float multipliery = (screenCorner1.Y - screenCorner2.Y) / 512.0000f;
            int i = 0;
            for(float y = 0; y < 512; y++)
            {
                for(float x = 0; x < 512; x++)
                {
                    // De 1 bij z * de afstand tussen de camera en het scherm.
                    // Dit berekent de unit vectors tussen de camera en de pixels van het scherm.
                    // Dit zal de directionvectoren worden van de rays.
                    screen[i] = new Vector3(multiplierx * x - screenCorner1.X, multipliery *  y - screenCorner1.Y, 1) - cameraPosition;
                    screen[i].Normalize();
                    i++;
                }
                
            }
        }

        // Maakt een ray uit de lijst van normals per pixel.
        public Ray SendRay(int i)
        {
            // Een ray is 100 lang.
            return new Ray(cameraPosition, screen[i], 100);
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
            // Veranderd de camerapositie met de vergroting 1 op 48. 
            cameraPosition = cameraPosition + new Vector3(x / 48, y / 48, z / 48);
        }
    }
}
