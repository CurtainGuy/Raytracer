using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace template
{
    public class camera
    {
        Vector3 cameraPosition, cameraDirection;

        public camera(Vector3 position, Vector3 direction)
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
