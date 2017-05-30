using System;
using System.IO;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Template
{
	public class OpenTKApp : GameWindow
	{
		static int screenID;
		static Raytracer rayTracer;
		static bool terminated = false;
		protected override void OnLoad( EventArgs e )
		{
			// called upon app init
			GL.ClearColor( Color.Black );
			GL.Enable( EnableCap.Texture2D );
			GL.Disable( EnableCap.DepthTest );
			GL.Hint( HintTarget.PerspectiveCorrectionHint, HintMode.Nicest );
			ClientSize = new Size( 1024, 512 );
			rayTracer = new Raytracer();
			rayTracer.screen = new Surface( Width, Height );
            rayTracer.Init();
            Sprite.target = rayTracer.screen;
			screenID = rayTracer.screen.GenTexture();
		}
		protected override void OnUnload( EventArgs e )
		{
			// called upon app close
			GL.DeleteTextures( 1, ref screenID );
			Environment.Exit( 0 ); // bypass wait for key on CTRL-F5
		}
		protected override void OnResize( EventArgs e )
		{
			// called upon window resize
			GL.Viewport(0, 0, Width, Height);
			GL.MatrixMode( MatrixMode.Projection );
			GL.LoadIdentity();
			GL.Ortho( -1.0, 1.0, -1.0, 1.0, 0.0, 4.0 );
		}
		protected override void OnUpdateFrame( FrameEventArgs e )
		{
			// called once per frame; app logic
			var keyboard = OpenTK.Input.Keyboard.GetState();
			if (keyboard[OpenTK.Input.Key.Escape]) this.Exit();
            if (keyboard[Key.Left]) //beweeg het scherm naar links
                { rayTracer.CameraX -= 1; rayTracer.camera.CameraTransform(-1, 0, 0); rayTracer.screen.Clear(0); }
            if(keyboard[Key.Right]) //beweeg het scherm naar rechts
                { rayTracer.CameraX += 1; rayTracer.camera.CameraTransform(1, 0, 0); rayTracer.screen.Clear(0); }
            if(keyboard[Key.Up]) //beweeg het scherm naar boven (Z- as)
                { rayTracer.CameraZ -= 1; rayTracer.camera.CameraTransform(0, 0, 1); rayTracer.screen.Clear(0); }
            if(keyboard[Key.Down]) //beweeg het scherm naar beneden (Z- as)
                { rayTracer.CameraZ += 1; rayTracer.camera.CameraTransform(0, 0, -1); rayTracer.screen.Clear(0); }
            if(keyboard[Key.KeypadPlus]) //beweeg het scherm naar boven (Y-as)
                { rayTracer.camera.CameraTransform(0, 1, 0); rayTracer.screen.Clear(0); }
            if(keyboard[Key.KeypadMinus]) //beweeg het scherm naar beneden (Y-as)
                { rayTracer.camera.CameraTransform(0, -1, 0); rayTracer.screen.Clear(0); }
            if (keyboard[Key.Enter]) //druk op Enter om het beeld te renderen
            {
                rayTracer.Render();
            }
		}
		protected override void OnRenderFrame( FrameEventArgs e )
		{
			// called once per frame; render
		    rayTracer.Tick();
			if (terminated) 
			{
				Exit();
				return;
			}
			// convert Game.screen to OpenGL texture
			GL.BindTexture( TextureTarget.Texture2D, screenID );
			GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 
						   rayTracer.screen.width, rayTracer.screen.height, 0, 
						   OpenTK.Graphics.OpenGL.PixelFormat.Bgra, 
						   PixelType.UnsignedByte, rayTracer.screen.pixels 
						 );
			// clear window contents
			GL.Clear( ClearBufferMask.ColorBufferBit );
			// setup camera
			GL.MatrixMode( MatrixMode.Modelview );
			GL.LoadIdentity();
			GL.MatrixMode( MatrixMode.Projection );
			GL.LoadIdentity();
			// draw screen filling quad
			GL.Begin( PrimitiveType.Quads );
			GL.TexCoord2( 0.0f, 1.0f ); GL.Vertex2( -1.0f, -1.0f );
			GL.TexCoord2( 1.0f, 1.0f ); GL.Vertex2(  1.0f, -1.0f );
			GL.TexCoord2( 1.0f, 0.0f ); GL.Vertex2(  1.0f,  1.0f );
			GL.TexCoord2( 0.0f, 0.0f ); GL.Vertex2( -1.0f,  1.0f );
			GL.End();
			// tell OpenTK we're done rendering
			SwapBuffers();
		}

		public static void Main( string[] args ) 
		{ 
			// entry point
			using (OpenTKApp app = new OpenTKApp()) { app.Run( 30.0, 0.0 ); }
		}
	}
}