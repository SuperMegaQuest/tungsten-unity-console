using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace HUDConsole {
	public class Console : MonoBehaviour {
#region Public
		public static bool IsActive {
			get { return _instance != null && _instance._consoleView.IsActive; }
		}

		public static ConsoleHistory ConsoleHistory {
			get { return _instance._consoleHistory; }
		}

		public static void AddCommand(string commandName, CommandHandler handler, string helpText) {
			_commands.Add(commandName.ToLowerInvariant(), new ConsoleCommand(commandName, handler, helpText));
		}

		public static void ExecuteCommand(string command) {
			ParseCommand(command);
		}

		public static void Log(string logString, LogType logType = LogType.Log, bool doStackTrace = true) {
			CreateLog(logString, logType, doStackTrace, false, Color.white, Color.black);
		}

		public static void Log(string logString, Color textColor, Color bgColor, bool doStackTrace = true) {
			CreateLog(logString, LogType.Log, doStackTrace, true, textColor, bgColor);
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

		public static List<ConsoleCommand> GetOrderedCommands() {
			return _commands.Values.OrderBy(c => c.commandName).ToList();
		}

		public static List<ConsoleLog> GetHistoryConsoleLogs() {
			return ConsoleHistory.LogGetAll();
		}

		public static string GetHistoryString(bool stripRichText = false) {
			List<ConsoleLog> history = GetHistoryConsoleLogs();
			StringBuilder stringBuilder = new StringBuilder();

			foreach (ConsoleLog log in history) {
				stringBuilder.AppendLine(log.logString.Trim());
				if (log.stackTrace != "") { stringBuilder.AppendLine(log.stackTrace.Trim()); }

				stringBuilder.Append(Environment.NewLine);
			}

			return (stripRichText ? Regex.Replace(stringBuilder.ToString(), "<.*?>", string.Empty) : stringBuilder.ToString()).Trim();
		}

		/// <summary>Save console history to a log file and return the file's path.</summary>
		public static string SaveHistoryToLogFile(string path = "", string prefix = "console", bool stripRichText = false) {
			path = path.Trim();

			if (path == string.Empty) {
				path = Application.persistentDataPath;
			} else if (path.EndsWith(":")) {
				path += "\\";
			} else if (path.EndsWith("/")) {
				path = path.Replace("/", "\\");
			}

			if (_isDrivePath.IsMatch(path)) {
				if (Directory.GetLogicalDrives().All(drive => !drive.Equals(path, StringComparison.CurrentCultureIgnoreCase))) {
					LogError(string.Format("Drive not found: {0}", path));

					throw new Exception("Drive not found");
				}

				path = string.Format("{0}:", path[0]);
			} else if (Directory.Exists(Path.GetDirectoryName(path)) == false) {
				LogError(string.Format("Directory not found: {0}", path));

				throw new Exception("Directory not found");
			}

			path = string.Format("{0}/{1}_{2:yyyy-MM-dd_HH-mm-ss}.log", path, prefix, DateTime.Now);
			File.WriteAllText(path, GetHistoryString(stripRichText));

			return path.Replace("\\", "/");
		}

		/// <summary>Copy console history to the clipboard (<see cref="GUIUtility.systemCopyBuffer"/>).</summary>
		public static void CopyHistoryToClipboard(bool stripRichText = false) {
			GUIUtility.systemCopyBuffer = GetHistoryString(stripRichText);
		}

		public static void ClearConsoleView() {
			_instance._consoleView.ClearConsoleView();
		}

		public static void SetHelpTextFormat(string helpTextFormat) {
			_helpTextFormat = helpTextFormat;
		}

		public static void PrintHelpText() {
			foreach (var command in _commands.Values.OrderBy(c => c.commandName)) {
				Log(string.Format(_helpTextFormat, command.commandName, command.helpText), LogType.Log, false);
			}
		}
#endregion Public

#region Private
		private static Console _instance = null;

		private static string _helpTextFormat = "{0} : {1}";

		/// <summary>Tests whether a string is a drive root. e.g. "D:\"</summary>
		const string _regexDrivePath = @"^\w:(?:\\|\/)?$";

		/// <summary>Tests whether a string is a drive root. e.g. "D:\"</summary>
		static readonly Regex _isDrivePath = new Regex(_regexDrivePath);

		[Header("History")]
		[SerializeField] private ConsoleHistory _consoleHistory;

		[Header("Default Commands")]
		[SerializeField] private bool _enableDefaultCommands = true;

		[Header("Unity Log Settings")]
		[SerializeField] private bool _logUnityErrors = true;

		[SerializeField] private bool _logUnityAsserts = true;

		[SerializeField] private bool _logUnityWarnings = true;

		[SerializeField] private bool _logUnityLogs = true;

		[SerializeField] private bool _logUnityExceptions = true;

		[Header("Console View")]
		[Tooltip("Select which console view implementation to use.")]
		[SerializeField] private ConsoleViewAbstract _consoleViewPrefab;

		private ConsoleViewAbstract _consoleView;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void InstantiateConsole() {
			Instantiate(Resources.Load("Console"));
		}

		private void Awake() {
			if (_instance == null) {
				_instance = this;
			} else {
				Debug.LogError("Two instances of ConsoleController detected.");
				Destroy(gameObject);
			}

			// Instantiate view.
			_consoleView = Instantiate(_consoleViewPrefab);
			_consoleView.transform.SetParent(transform, false);

			if (_enableDefaultCommands == false) { return; }

			// Add core commands.
			AddCommand("Echo", ConsoleCoreCommands.Echo, "Display message to console.");
			AddCommand("Console.Log", ConsoleCoreCommands.ConsoleLog, "Display message to console.");
			AddCommand("Console.LogWarning", ConsoleCoreCommands.ConsoleLogWarning, "Display warning message to console.");
			AddCommand("Console.LogError", ConsoleCoreCommands.ConsoleLogError, "Display error message to console.");
			AddCommand("Console.Save", ConsoleCoreCommands.ConsoleSave, "Save console to log file.");
			AddCommand("Console.Copy", ConsoleCoreCommands.ConsoleCopy, "Copy console to clipboard.");
			AddCommand("Console.Clear", ConsoleCoreCommands.ConsoleClear, "Clear console.");
			AddCommand("Help", ConsoleCoreCommands.Help, "List of commands and their help text");

			AddCommand("Debug.Log", ConsoleCoreCommands.DebugLog, "Logs message to the Unity Console.");
			AddCommand("Debug.LogWarning", ConsoleCoreCommands.DebugLogWarning, "A variant of Debug.Log that logs a warning message to the console.");
			AddCommand("Debug.LogError", ConsoleCoreCommands.DebugLogError, "A variant of Debug.Log that logs an error message to the console.");
			AddCommand("Debug.Break", ConsoleCoreCommands.DebugBreak, "Pauses the editor.");
		}

		private void Start() {
			UnityLogsStart();
		}

#region Commands
		private static Dictionary<string, ConsoleCommand> _commands = new Dictionary<string, ConsoleCommand>();

		/// <summary>Splits a string by spaces, unless surrounded by (unescaped) single or double quotes.</summary>
		const string _regexStringSplit = @"(""[^""\\]*(?:\\.[^""\\]*)*""|'[^'\\]*(?:\\.[^'\\]*)*'|[\S]+)+";

		/// <summary>Tests whether a string starts and ends with either double or single quotes (not a mix).</summary>
		const string _regexQuoteWrapped = @"^"".*""$|^'.*'$";

		/// <summary>Tests whether a string starts and ends with either double or single quotes (not a mix).</summary>
		static readonly Regex _isWrappedInQuotes = new Regex(_regexQuoteWrapped);

		private static void ParseCommand(string commandString) {
			ConsoleHistory.CommandHistoryAdd(commandString);

			commandString = commandString.Trim();

			List<string> cmdSplit = ParseArguments(commandString);

			string cmdName = cmdSplit[0].ToLower();
			cmdSplit.RemoveAt(0);

			ConsoleLog newLog = new ConsoleLog("> " + commandString, "", LogType.Log, false, Color.white, Color.black);
			ConsoleHistory.LogAdd(newLog);

			try {
				_commands[cmdName].handler(cmdSplit.ToArray());
			}
			catch (KeyNotFoundException) {
				LogError(String.Format("Command \"{0}\" not found.", cmdName));
			}
			catch (Exception) { }
		}

		private static List<string> ParseArguments(string commandString) {
			var args = new List<string>();

			foreach (Match match in Regex.Matches(commandString, _regexStringSplit)) {
				string value = match.Value.Trim();

				if (_isWrappedInQuotes.IsMatch(value)) { value = value.Substring(1, value.Length - 2); }

				args.Add(value);
			}

			return args;
		}
#endregion Commands

#region Logs
		private static void CreateLog(string logString, LogType logType, bool doStackTrace, bool customColor, Color textColor, Color bgColor) {
			var stackTrace = "";
			if (doStackTrace) {
				stackTrace = new System.Diagnostics.StackTrace().ToString();
			}

			ConsoleLog newLog = new ConsoleLog(logString, stackTrace, logType, customColor, textColor, bgColor);
			ConsoleHistory.LogAdd(newLog);
		}

		private static void CreateLog(string logString, string stackTrace, LogType logType) {
			var newLog = new ConsoleLog(logString, stackTrace, logType, false, Color.white, Color.black);
			ConsoleHistory.LogAdd(newLog);
		}
#endregion Logs

#region UnityLogs
		private void UnityLogsStart() {
			Application.logMessageReceived += HandleUnityLog;
		}

		private void HandleUnityLog(string logString, string stackTrace, LogType logType) {
			switch (logType) {
				case LogType.Error: {
					if (_logUnityErrors) {
						CreateLog(logString, stackTrace, logType);
					}

					break;
				}
				case LogType.Assert: {
					if (_logUnityAsserts) {
						CreateLog(logString, stackTrace, logType);
					}

					break;
				}
				case LogType.Warning: {
					if (_logUnityWarnings) {
						CreateLog(logString, stackTrace, logType);
					}

					break;
				}
				case LogType.Log: {
					if (_logUnityLogs) {
						CreateLog(logString, stackTrace, logType);
					}

					break;
				}
				case LogType.Exception: {
					if (_logUnityExceptions) {
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