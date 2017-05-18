using System;
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
        Camera camera;
        // initialize
        public void Init()
        {
            // the camera from where you see the scene
            camera = new Camera(new Vector3(0, 0, 0), new Vector3(0, 0, 1));
        }
        // tick: renders one frame
        public void Tick()
        {
        }

        public void Render()
        {
            Ray ray = new Ray(new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0);
            for (int x = 0; x < camera.screen.width; x++)
                for (int y = 0; y < camera.screen.height; y++)
                {
                    //shoot rays at screen
                }
        }
    }

} // namespace Template