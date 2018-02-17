 _    _ _    _ _____     _____                      _      
| |  | | |  | |  __ \   / ____|                    | |     
| |__| | |  | | |  | | | |     ___  _ __  ___  ___ | | ___ 
|  __  | |  | | |  | | | |    / _ \| '_ \/ __|/ _ \| |/ _ \
| |  | | |__| | |__| | | |___| (_) | | | \__ \ (_) | |  __/
|_|  |_|\____/|_____/   \_____\___/|_| |_|___/\___/|_|\___|
                              garrettshields.me/hudconsole/

----- ----- ----- -----
Adding HUD Console to your project.
----- ----- ----- -----
	To add the console to your project simply drag Console/Console into your scene.
	The console can be opened by pressing Grave '`' (The key above tab).

----- ----- ----- -----
Accessing the Console.
----- ----- ----- -----
	The console can be accessed by either typing out it's namespace for each use:
		HUDConsole.Console.Log("Hello");

	Or you can add it's namespace to the top of each file that will be accessing it.
		using HUDConsole;
		Console.Log("Hello");

----- ----- ----- -----
Logs.
----- ----- ----- -----
	HUD Console has the same log types as Unity.
		Error, Assert, Warning, Log, Exception.

	They can be used using the following commands:
		Console.Log("Hello");
		Console.LogWarning("Hello");
		Console.LogError("Hello");
		Console.LogAssert("Hello");
		Console.LogException("Hello");

	From code, you can also just use Console.Log and pass in the LogType as an argument.
		Console.Log("Hello", LogType.Warning);

	For logs of LogType.Log you can also specify custom colors for text, and background.
		Console.Log("Hello", Color.white, Color.black);

----- ----- ----- -----
Adding commands.
----- ----- ----- -----
	Commands can be added by using Console.AddCommand. It requires 3 arguments:
		1. A string that will be used as the key for accessing the command.
		2. The function you would like the command to execute.
		3. A string that will be displayed with the key when the "help" command is used.

	Functions that are to be added as commands must match the following signature:
	void MyFunction(string[] args)

	In the Example folder look at Scripts/Welcome.cs or Scripts/Graph.cs line 54.

----- ----- ----- -----
Built in commands.
----- ----- ----- -----
	The built in commands can be found in Console/Scripts/ConsoleCoreCommands.cs,
	and are added to the console in Console/Scripts/Console line 91.

	As everyones requirements of a console would range quite considerably
	I have purposefully only included a handful of commands, however I plan on adding more later.

----- ----- ----- -----
Creating new Color Sets.
----- ----- ----- -----
	New color sets can be created to customize the appearance of the console.

	1. Create a new ColorSet by right clicking in the Project tab and go to Create -> Console -> Obelisk ColorSet.
	2. Name the new ColorSet anything you like.
	3. Find Console/Obelisk/ObeliskConsole and change the ColorSet property to what ever ColorSet you like.

----- ----- ----- -----
Creating your own implementation of the Console View.
----- ----- ----- -----
	The default implementation of HUD Console's view is called Obelisk, but you can also create your own.

	These are the basic steps to creating your own view. (I may expand this later).
		1. Create a class that inherits from ConsoleViewAbstract.
		2. Override the virtual functions listed in ConsoleViewAbstract.
		3. Create a prefab with your new class as a component.
		4. Go to Console/Console and select your prefab in the ConsoleViewPrefab property.
		5. The rest is up to you! This is your view, make it how you like.