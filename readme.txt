

Usage instructions:

Include the API(PluginTaskbar.cs) in your project.

Create a 30x30 icon or set of 30x30 icons

Load your icon/icons as Texture2D or the relevant Texture subclass

use the PluginTaskbar namespace
	using PluginTaskbar;

// implement the interface ITaskbarModule
	ExampleClass : Part, ITaskbarModule


// define the functions of the interface

	public Texture TaskbarIcon()
	{
		return yourIcon;
	}

	public void Clicked(bool leftClick)
	{
		if(leftClick)
			Debug.Log("LEFT CLICKED!");
		else
			Debug.Log("RIGHT CLICKED!");
	}	

	// this isn't used yet, but go ahead and add it
	
	public string TooltipText()
	{ 
		return yourTooltipText; 
	}


// define a few things

	// the key for your module, this needs to be unique per toolbar icon
	private static string yourModuleName = "yourModuleName";
	
	// the hook class, you must define one of these for each icon
	private TaskbarHooker taskbarHook;		


// create an instance of the TaskbarHooker, use this because your class implements
// the interface ITaskbarModule
	
	taskbarHook = new TaskbarHooker(this, yourModuleName);


// start the hook when your part loads, this returns a boolean so if you have a
// secondary icon or display method you can use it instead

	// returns true if taskbar is installed and successfully hooked
	taskbarHook.Start();


// stop the hook when the part dies, very important, this also returns a boolean
// so you can handle things alternatively if it fails

	// returns true if taskbar is installed and successfully unhooked
	taskbarHook.Start();



Enjoy!
