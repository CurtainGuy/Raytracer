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
        public Surface screen;
        public Vector3 screenCorner0, screenCorner1, screenCorner2;

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
            Camera camera = new Camera(new Vector3(0, 0, 0), new Vector3(0, 0, 1));

            // the screen where the rays are shot at
            screen = new Surface(512, 512);
            // the corners of the screen
            screenCorner0 = camera.CameraPosition + distance * camera.CameraDirection + new Vector3(-1, -1, 0);
            screenCorner1 = camera.CameraPosition + distance * camera.CameraDirection + new Vector3(1, -1, 0);
            screenCorner2 = camera.CameraPosition + distance * camera.CameraDirection + new Vector3(-1, 1, 0);
        }
        // tick: renders one frame
        public void Tick()
        {
            screen.Clear(0);
            screen.Print("hello world", 2, 2, 0xffffff);
            screen.Line(2, 20, 160, 20, 0xff0000);
        }
    }

} // namespace Template