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
            //
        }

        // Dit werkt nu alleen voor een richting.
        void ScreenSetup()
        {
            // Eerste 1 is de maximale x, 2e is de minimale x.
            float multiplier = (1 - -1) / 512;
            int i = 0;
            for(int y = 0; y < 512; y++)
            {
                for(int x = 0; x < 512; x++)
                {
                    // De 1tjes bij x en y zijn x1, ofwel minimale x. De 1 bij z is de distance.
                    screen[i] = new Vector3(multiplier * x + 1, multiplier * y - 1, 1) - cameraPosition;
                    screen[i].Normalize();
                }
            }
        }

        public Ray SendRay(int i)
        {
            // 5 is de lengte van de ray.
            return ray = new Ray(cameraPosition, screen[i], 5);
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
