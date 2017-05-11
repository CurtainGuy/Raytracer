using System;
using System.IO;

namespace Template {

class Game
{
	// member variables
	public Surface screen;
	// initialize
	public void Init()
	{
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