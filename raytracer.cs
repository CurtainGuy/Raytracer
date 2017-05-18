using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Template {

class Raytracer
{
	// member variables
	public Surface screen;

	// initialize
	public void Init()
	{
            // the camera from where you see the scene
            template.camera camera = new template.camera(new Vector3(0, 0, 0), new Vector3(0, 0, 1));

            // the screen where the rays are shot at
            screen = new Surface(512, 512);
        }
	// tick: renders one frame
	public void Tick()
	{
		screen.Clear( 0 );
		screen.Print( "hello world", 2, 2, 0xffffff );
        screen.Line(2, 20, 160, 20, 0xff0000);
	}
}

} // namespace Template