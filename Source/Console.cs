using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace HUDConsole {
	public class Console : MonoBehaviour {
		
#region Init
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void InstantiateConsole() {
			Instantiate(Resources.Load("Console"));
		}

		private void Awake() {
			// Setup instance.
			if (_instance == null) {
				_instance = this;
			} else {
				Debug.LogError("Two instances of HUDConsole detected.");
				Destroy(gameObject);
			}

			// Instantiate view.
			InstantiateView();
			
			// Set dont destroy on load to this object.
			DontDestroyOnLoad(gameObject);

			// Add core commands.
			if (_config._enableCoreCommands) {
				ConsoleCoreCommands.AddCoreCommands();
			}
		}

		private void Start() {
			UnityLogsStart();
		}

		private void OnDestroy() {
			UnityLogsOnDestroy();
		}
#endregion Init

#region Core
		[Header("Console Config")]
		[SerializeField] private ConsoleConfig _config;
		
		private static Console _instance = null;
		
		public static bool IsActive {
			get { return _instance != null && _instance._consoleView.IsActive; }
		}
		
		public static ConsoleHistory ConsoleHistory {
			get { return _instance._config._consoleHistory; }
		}
		
		/// <summary>
		/// Save console history to a log file and return the file's path.
		/// </summary>
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
		
		/// <summary>
		/// Copy console history to the clipboard (<see cref="GUIUtility.systemCopyBuffer"/>).
		/// </summary>
		public static void CopyHistoryToClipboard(bool stripRichText = false) {
			GUIUtility.systemCopyBuffer = GetHistoryString(stripRichText);
		}

		public static void SetHelpTextFormat(string helpTextFormat) {
			_helpTextFormat = helpTextFormat;
		}

		public static void PrintHelpText() {
			foreach (var command in _commands.Values.OrderBy(c => c.commandName)) {
				Log(string.Format(_helpTextFormat, command.commandName, command.helpText), LogType.Log, false);
			}
		}
		
		private static string _helpTextFormat = "{0} : {1}";
#endregion Core
		
#region View
		private ConsoleViewAbstract _consoleView;
		
		public static void ClearConsoleView() {
			_instance._consoleView.ClearConsoleView();
		}

		private void InstantiateView() {
			_consoleView = Instantiate(_config._consoleViewPrefab);
			_consoleView.transform.SetParent(transform, false);
		}
#endregion View

#region Commands
		private static Dictionary<string, ConsoleCommand> _commands = new Dictionary<string, ConsoleCommand>();

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
			catch (Exception) {
				
			}
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
		
		public static void AddCommand(string commandName, CommandHandler handler, string helpText) {
			_commands.Add(commandName.ToLowerInvariant(), new ConsoleCommand(commandName, handler, helpText));
		}

		public static void ExecuteCommand(string command) {
			ParseCommand(command);
		}
		
		public static List<ConsoleCommand> GetOrderedCommands() {
			return _commands.Values.OrderBy(c => c.commandName).ToList();
		}
#endregion Commands

#region Logs
		private static void CreateLog(
			string logString,LogType logType, bool doStackTrace, bool customColor, Color textColor, Color bgColor) {
			var stackTrace = doStackTrace ? new System.Diagnostics.StackTrace().ToString() : string.Empty;
			var newLog = new ConsoleLog(logString, stackTrace, logType, customColor, textColor, bgColor);
			ConsoleHistory.LogAdd(newLog);
		}

		private static void CreateLog(string logString, string stackTrace, LogType logType) {
			var newLog = new ConsoleLog(logString, stackTrace, logType, false, Color.white, Color.black);
			ConsoleHistory.LogAdd(newLog);
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
		
		public static List<ConsoleLog> GetHistoryConsoleLogs() {
			return ConsoleHistory.LogGetAll();
		}
		
		public static string GetHistoryString(bool stripRichText = false) {
			var history = GetHistoryConsoleLogs();
			var stringBuilder = new StringBuilder();

			foreach (ConsoleLog log in history) {
				stringBuilder.AppendLine(log.logString.Trim());
				if (log.stackTrace != "") { stringBuilder.AppendLine(log.stackTrace.Trim()); }

				stringBuilder.Append(Environment.NewLine);
			}

			return (stripRichText ? Regex.Replace(stringBuilder.ToString(), "<.*?>", string.Empty) : stringBuilder.ToString()).Trim();
		}
#endregion Logs

#region UnityLogs
		private void UnityLogsStart() {
			Application.logMessageReceived += HandleUnityLog;
		}

		private void UnityLogsOnDestroy() {
			Application.logMessageReceived -= HandleUnityLog;
		}

		private void HandleUnityLog(string logString, string stackTrace, LogType logType) {
			switch (logType) {
				case LogType.Error: {
					if (_config._logUnityErrors) {
						CreateLog(logString, stackTrace, logType);
					}

					break;
				}
				case LogType.Assert: {
					if (_config._logUnityAsserts) {
						CreateLog(logString, stackTrace, logType);
					}

					break;
				}
				case LogType.Warning: {
					if (_config._logUnityWarnings) {
						CreateLog(logString, stackTrace, logType);
					}

					break;
				}
				case LogType.Log: {
					if (_config._logUnityLogs) {
						CreateLog(logString, stackTrace, logType);
					}

					break;
				}
				case LogType.Exception: {
					if (_config._logUnityExceptions) {
						CreateLog(logString, stackTrace, logType);
					}

					break;
				}
			}
		}
#endregion UnityLogs
		
#region Utility
		/// <summary>
		/// Tests whether a string is a drive root. e.g. "D:\"
		/// </summary>
		private const string _regexDrivePath = @"^\w:(?:\\|\/)?$";

		/// <summary>
		/// Tests whether a string is a drive root. e.g. "D:\"
		/// </summary>
		private static readonly Regex _isDrivePath = new Regex(_regexDrivePath);
		
		/// <summary>
		/// Splits a string by spaces, unless surrounded by (unescaped) single or double quotes.
		/// </summary>
		const string _regexStringSplit = @"(""[^""\\]*(?:\\.[^""\\]*)*""|'[^'\\]*(?:\\.[^'\\]*)*'|[\S]+)+";

		/// <summary>
		/// Tests whether a string starts and ends with either double or single quotes (not a mix).
		/// </summary>
		const string _regexQuoteWrapped = @"^"".*""$|^'.*'$";

		/// <summary>
		/// Tests whether a string starts and ends with either double or single quotes (not a mix).
		/// </summary>
		static readonly Regex _isWrappedInQuotes = new Regex(_regexQuoteWrapped);
#endregion Utility
		
	}
}