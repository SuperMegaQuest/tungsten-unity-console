using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace HUDConsole {
	public class Console : MonoBehaviour {
#region Public
		public static ConsoleHistory consoleHistory {
			get { return m_instance.m_consoleHistory; }
		}

		public static string helpTextFormat = "{0} : {1}";

		public static void AddCommand(string commandName, CommandHandler handler, string helpText) {
			m_commands.Add(commandName.ToLowerInvariant(), new ConsoleCommand(commandName, handler, helpText));
		}

		public static void ExecuteCommand(string command) {
			ParseCommand(command);
		}

		public static void Log(string logString, LogType logType = LogType.Log) {
			CreateLog(logString, logType, true, false, Color.white, Color.black);
		}

		public static void Log(string logString, Color textColor, Color bgColor) {
			CreateLog(logString, LogType.Log, true, true, textColor, bgColor);
		}

		public static void LogWarning(string logString) {
			CreateLog(logString, LogType.Warning, true, false, Color.white, Color.black);
		}

		public static void LogError(string logString) {
			CreateLog(logString, LogType.Error, true, false, Color.white, Color.black);
		}

		public static void LogAssert(string logString) {
			CreateLog(logString, LogType.Assert, true, false, Color.white, Color.black);
		}

		public static void LogException(string logString) {
			CreateLog(logString, LogType.Exception, true, false, Color.white, Color.black);
		}

		public static void ClearConsoleView() {
			m_instance.m_consoleView.ClearConsoleView();
		}

		public static void PrintHelpText() {
			foreach (var command in m_commands.Values.OrderBy(c => c.commandName)) {
				Log(string.Format(helpTextFormat, command.commandName, command.helpText));
			}
		}
#endregion Public

#region Private
		private static Console m_instance = null;

		[Header("History")]
		[SerializeField] private ConsoleHistory m_consoleHistory;

		[Header("Default Commands")]
		[SerializeField] private bool enableDefaultCommands = true;

		[Header("Unity Log Settings")]
		[SerializeField] private bool logUnityErrors = true;
		[SerializeField] private bool logUnityAsserts = true;
		[SerializeField] private bool logUnityWarnings = true;
		[SerializeField] private bool logUnityLogs = true;
		[SerializeField] private bool logUnityExceptions = true;

		[Header("Console View")]
		[Tooltip("Select which console view implementation to use.")]
		[SerializeField] private ConsoleViewAbstract consoleViewPrefab;
		private ConsoleViewAbstract m_consoleView;

		private void Awake() {
			if(m_instance == null) {
				m_instance = this;
			}
			else {
				Debug.LogError("Two instances of ConsoleController detected.");
				Destroy(gameObject);
			}

			// Instantiate view.
			m_consoleView = Instantiate(consoleViewPrefab);
			m_consoleView.transform.SetParent(transform, false);

			if (!enableDefaultCommands) { return; }

			// Add core commands.
			AddCommand("Echo", ConsoleCoreCommands.Echo, "Display message to console.");
			AddCommand("Console.Log", ConsoleCoreCommands.ConsoleLog, "Display message to console.");
			AddCommand("Console.LogWarning", ConsoleCoreCommands.ConsoleLogWarning, "Display warning message to console.");
			AddCommand("Console.LogError", ConsoleCoreCommands.ConsoleLogError, "Display error message to console.");
			AddCommand("Console.Clear", ConsoleCoreCommands.ConsoleClear, "Clear console.");
			AddCommand("Help", ConsoleCoreCommands.Help, "List of commands and their help text.");

			AddCommand("Debug.Log", ConsoleCoreCommands.DebugLog, "Logs message to the Unity Console.");
			AddCommand("Debug.LogWarning", ConsoleCoreCommands.DebugLogWarning, "A variant of Debug.Log that logs a warning message to the console.");
			AddCommand("Debug.LogError", ConsoleCoreCommands.DebugLogError, "A variant of Debug.Log that logs an error message to the console.");
			AddCommand("Debug.Break", ConsoleCoreCommands.DebugBreak, "Pauses the editor.");
		}

		private void Start() {
			UnityLogsStart();
		}

	#region Commands
		private static Dictionary<string, ConsoleCommand> m_commands = new Dictionary<string, ConsoleCommand>();

		private static void ParseCommand(string commandString) {
			consoleHistory.CommandHistoryAdd(commandString);

			commandString = commandString.Trim();

			string[] cmdSplit = commandString.Split(' ');

			string cmdName = cmdSplit[0];
			cmdName = cmdName.ToLower();
			string[] cmdArgs = new string[cmdSplit.Length - 1];
			for(int i = 1; i < cmdSplit.Length; i++) {
				cmdArgs[i - 1] = cmdSplit[i];
			}

			ConsoleLog newLog = new ConsoleLog("> " + commandString, "", LogType.Log, false, Color.white, Color.black);
			consoleHistory.LogAdd(newLog);

			try {
				m_commands[cmdName].handler(cmdArgs);
			}
			catch(KeyNotFoundException) {
				LogError(String.Format("Command \"{0}\" not found.", cmdName));
			}
			catch(Exception) {

			}
		}
	#endregion Commands

	#region Logs
		private static void CreateLog(string logString, LogType logType, bool doStackTrace, bool customColor, Color textColor, Color bgColor) {
			string stackTrace = "";
			if(doStackTrace) {
				stackTrace = new System.Diagnostics.StackTrace().ToString();
			}

			ConsoleLog newLog = new ConsoleLog(logString, stackTrace, logType, customColor, textColor, bgColor);
			consoleHistory.LogAdd(newLog);
		}

		private static void CreateLog(string logString, string stackTrace, LogType logType) {
			var newLog = new ConsoleLog(logString, stackTrace, logType, false, Color.white, Color.black);
			consoleHistory.LogAdd(newLog);
		}
	#endregion Logs

	#region UnityLogs
		private void UnityLogsStart() {
			Application.logMessageReceived += HandleUnityLog;
		}

		private void HandleUnityLog(string logString, string stackTrace, LogType logType) {
			switch(logType) {
				case LogType.Error: {
					if(logUnityErrors) {
						CreateLog(logString, stackTrace, logType);
					}
					break;
				}
				case LogType.Assert: {
					if(logUnityAsserts) {
						CreateLog(logString, stackTrace, logType);
					}
					break;
				}
				case LogType.Warning: {
					if(logUnityWarnings) {
						CreateLog(logString, stackTrace, logType);
					}
					break;
				}
				case LogType.Log: {
					if(logUnityLogs) {
						CreateLog(logString, stackTrace, logType);
					}
					break;
				}
				case LogType.Exception: {
					if(logUnityExceptions) {
						CreateLog(logString, stackTrace, logType);
					}
					break;
				}
			}
		}
	#endregion UnityLogs
#endregion Private
	}
}