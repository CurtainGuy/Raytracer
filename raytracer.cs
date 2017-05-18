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
        Camera camera;
        Surface screen;
        // Lijsten
        // List<Primitive> primitives;
        List<LightSource> lightsources;


        // distance from camera to screen (change FOV by changing distance)
        public int distance = 1;
        // initialize
        public void Init()
        {
            // Initialiseert lijsten.
            //primitives = new List<Primitive>();
            lightsources = new List<LightSource>();


            // the camera from where you see the scene
            camera = new Camera(new Vector3(0, 0, 0), new Vector3(0, 0, 1));
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
        // tick: renders one frame
        public void Tick()
        {
            camera.screen.Clear(0);
            camera.screen.Print("hello world", 2, 2, 0xffffff);
            camera.screen.Line(2, 20, 160, 20, 0xff0000);
        }
    }

} // namespace Template