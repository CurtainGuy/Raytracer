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
        public Surface screen;
        Vector3 cameraPosition, cameraDirection;
        public Vector3 screenCorner0, screenCorner1, screenCorner2;
        // distance from camera to screen (change FOV by changing distance)
        public int distance = 1;

        public Camera(Vector3 position, Vector3 direction)
        {
            cameraPosition = position;
            cameraDirection = direction;

            // the screen where the rays are shot at
            screen = new Surface(512, 512);

            // the corners of the screen
            screenCorner0 = cameraPosition + distance * cameraDirection + new Vector3(-1, -1, 0);
            screenCorner1 = cameraPosition + distance * cameraDirection + new Vector3(1, -1, 0);
            screenCorner2 = cameraPosition + distance * cameraDirection + new Vector3(-1, 1, 0);
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
