using UnityEngine;

namespace HUDConsole {
	public class ConsoleCoreCommands {
#region Init		
		public static void AddCoreCommands() {
			Console.AddCommand("Console.Log", ConsoleCoreCommands.ConsoleLog, "Display message to console.");
			Console.AddCommand("Console.LogWarning", ConsoleCoreCommands.ConsoleLogWarning, "Display warning message to console.");
			Console.AddCommand("Console.LogError", ConsoleCoreCommands.ConsoleLogError, "Display error message to console.");
			Console.AddCommand("Console.Save", ConsoleCoreCommands.ConsoleSave, "Save console to log file.");
			Console.AddCommand("Console.Copy", ConsoleCoreCommands.ConsoleCopy, "Copy console to clipboard.");
			Console.AddCommand("Console.Clear", ConsoleCoreCommands.ConsoleClear, "Clear console.");
			Console.AddCommand("Help", ConsoleCoreCommands.Help, "List of commands and their help text");

			Console.AddCommand("Debug.Log", ConsoleCoreCommands.DebugLog, "Logs message to the Unity Console.");
			Console.AddCommand("Debug.LogWarning", ConsoleCoreCommands.DebugLogWarning, "A variant of Debug.Log that logs a warning message to the console.");
			Console.AddCommand("Debug.LogError", ConsoleCoreCommands.DebugLogError, "A variant of Debug.Log that logs an error message to the console.");
			Console.AddCommand("Debug.Break", ConsoleCoreCommands.DebugBreak, "Pauses the editor.");
		}
#endregion Init		

#region Console
		public static void ConsoleLog(string[] args) {
			string output = "";
			for (int i = 0, n = args.Length; i < n; i++) {
				output += args[i] + " ";
			}

			Console.Log(output);
		}

		public static void ConsoleLogWarning(string[] args) {
			string output = "";
			for (int i = 0, n = args.Length; i < n; i++) {
				output += args[i] + " ";
			}

			Console.LogWarning(output);
		}

		public static void ConsoleLogError(string[] args) {
			string output = "";
			for (int i = 0, n = args.Length; i < n; i++) {
				output += args[i] + " ";
			}

			Console.LogError(output);
		}

		public static void ConsoleSave(string[] args) {
			Console.Log(string.Format("Saved to {0}", Console.SaveHistoryToLogFile((args == null || args.Length == 0) ? "" : args[0])));
		}

		public static void ConsoleCopy(string[] args) {
			Console.CopyHistoryToClipboard();
		}

		public static void ConsoleClear(string[] args) {
			Console.ClearConsoleView();
		}

		public static void Help(string[] args) {
			Console.PrintHelpText();
		}
#endregion Console

#region Debug
		public static void DebugLog(string[] args) {
			string output = "";
			for (int i = 0, n = args.Length; i < n; i++) {
				output += args[i] + " ";
			}

			Debug.Log(output);
		}

		public static void DebugLogWarning(string[] args) {
			string output = "";
			for (int i = 0, n = args.Length; i < n; i++) {
				output += args[i] + " ";
			}

			Debug.LogWarning(output);
		}

		public static void DebugLogError(string[] args) {
			string output = "";
			for (int i = 0, n = args.Length; i < n; i++) {
				output += args[i] + " ";
			}

			Debug.LogError(output);
		}

		public static void DebugBreak(string[] args) {
			Debug.Break();
		}
#endregion Debug
	}
}