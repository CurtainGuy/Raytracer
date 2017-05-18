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

        public Camera(Vector3 position, Vector3 direction)
        {
            cameraPosition = position;
            cameraDirection = direction;
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
