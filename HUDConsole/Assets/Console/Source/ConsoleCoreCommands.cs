using UnityEngine;

namespace HUDConsole {
	public class ConsoleCoreCommands {
#region Console
		public static void Echo(string[] args) {
			string output = "";
			for(int i = 0; i < args.Length; i++) {
				output += args[i] + " ";
			}

			Console.Log(output);
		}

		public static void ConsoleLog(string[] args) {
			string output = "";
			for(int i = 0; i < args.Length; i++) {
				output += args[i] + " ";
			}

			Console.Log(output);
		}

		public static void ConsoleLogWarning(string[] args) {
			string output = "";
			for(int i = 0; i < args.Length; i++) {
				output += args[i] + " ";
			}

			Console.LogWarning(output);
		}

		public static void ConsoleLogError(string[] args) {
			string output = "";
			for(int i = 0; i < args.Length; i++) {
				output += args[i] + " ";
			}

			Console.LogError(output);
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
			for(int i = 0; i < args.Length; i++) {
				output += args[i] + " ";
			}

			Debug.Log(output);
		}

		public static void DebugLogWarning(string[] args) {
			string output = "";
			for(int i = 0; i < args.Length; i++) {
				output += args[i] + " ";
			}

			Debug.LogWarning(output);
		}

		public static void DebugLogError(string[] args) {
			string output = "";
			for(int i = 0; i < args.Length; i++) {
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