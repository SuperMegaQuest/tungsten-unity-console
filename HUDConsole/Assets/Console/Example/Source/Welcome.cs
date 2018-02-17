using UnityEngine;
using HUDConsole;

public class Welcome : MonoBehaviour {
	private void Start() {
		Console.Log("Welcome to the HUD Console Demo :)");
		Console.Log("The graph example is a tutorial from catlikecoding.com, so please check it out.");
		Console.Log("Type \"help\" to get a list of available commands.");
		Console.Log("There is a readme file located at Console/readme.txt");
	}
}