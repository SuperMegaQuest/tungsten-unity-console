using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Gruel {
	public class Console : MonoBehaviour {
		
#region Properties
		public static bool IsActive => _instance != null && _instance._consoleView.IsActive;

		public static ConsoleHistory ConsoleHistory => _instance._config.ConsoleHistory;

		public static string HelpTextFormat {
			get => _instance._helpTextFormat;
			set => _instance._helpTextFormat = value;
		}
#endregion Properties

#region Fields
		[Header("Console Config")]
		[SerializeField] private ConsoleConfig _config;

		/// <summary>
		/// Tests whether a string is a drive root. e.g. "D:\"
		/// </summary>
		private const string RegexDrivePath = @"^\w:(?:\\|\/)?$";

		/// <summary>
		/// Tests whether a string is a drive root. e.g. "D:\"
		/// </summary>
		private static readonly Regex IsDrivePath = new Regex(RegexDrivePath);
		
		/// <summary>
		/// Splits a string by spaces, unless surrounded by (unescaped) single or double quotes.
		/// </summary>
		private const string RegexStringSplit = @"(""[^""\\]*(?:\\.[^""\\]*)*""|'[^'\\]*(?:\\.[^'\\]*)*'|[\S]+)+";

		/// <summary>
		/// Tests whether a string starts and ends with either double or single quotes (not a mix).
		/// </summary>
		private const string RegexQuoteWrapped = @"^"".*""$|^'.*'$";

		/// <summary>
		/// Tests whether a string starts and ends with either double or single quotes (not a mix).
		/// </summary>
		private static readonly Regex IsWrappedInQuotes = new Regex(RegexQuoteWrapped);
		
		private static Console _instance;
		
		private ConsoleView _consoleView;
		private string _helpTextFormat = "{0} : {1}";
		
		private static Dictionary<string, ConsoleCommand> _commands = new Dictionary<string, ConsoleCommand>();
#endregion Fields

#region Public Methods
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
		
		public static void AddCommand(string commandName, CommandHandler handler, string helpText) {
			_commands.Add(commandName.ToLowerInvariant(), new ConsoleCommand(commandName, handler, helpText));
		}

		public static void ExecuteCommand(string command) {
			ParseCommand(command);
		}
		
		public static List<ConsoleCommand> GetOrderedCommands() {
			return _commands.Values.OrderBy(c => c.CommandName).ToList();
		}
		
		public static List<ConsoleLog> GetHistoryConsoleLogs() {
			return ConsoleHistory.LogHistory;
		}
		
		public static string GetHistoryString(bool stripRichText = false) {
			var history = GetHistoryConsoleLogs();
			var stringBuilder = new StringBuilder();

			foreach (ConsoleLog log in history) {
				stringBuilder.AppendLine(log.LogString.Trim());
				if (log.StackTrace != "") { stringBuilder.AppendLine(log.StackTrace.Trim()); }

				stringBuilder.Append(Environment.NewLine);
			}

			return (stripRichText ? Regex.Replace(stringBuilder.ToString(), "<.*?>", string.Empty) : stringBuilder.ToString()).Trim();
		}
		
		/// <summary>
		/// Copy console history to the clipboard (<see cref="GUIUtility.systemCopyBuffer"/>).
		/// </summary>
		public static void CopyHistoryToClipboard(bool stripRichText = false) {
			GUIUtility.systemCopyBuffer = GetHistoryString(stripRichText);
		}
		
		public static void ClearConsoleView() {
			_instance._consoleView.ClearConsoleView();
		}

		public static void PrintHelpText() {
			foreach (var command in _commands.Values.OrderBy(c => c.CommandName)) {
				Log(string.Format(_instance._helpTextFormat, command.CommandName, command.HelpText), LogType.Log, false);
			}
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

			if (IsDrivePath.IsMatch(path)) {
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
#endregion Public Methods

#region Private Methods
		private void Awake() {
			// Setup instance.
			if (_instance == null) {
				_instance = this;
			} else {
				Debug.LogError("Console: There is already an instance of Console!");
				Destroy(gameObject);
			}

			// Instantiate view.
			InstantiateView();
			
			// Set dont destroy on load to this object.
			if (_config.DontDestroyOnLoad) {
				DontDestroyOnLoad(gameObject);
			}

			// Add core commands.
			if (_config.EnableCoreCommands) {
				ConsoleCoreCommands.AddCoreCommands();
			}
		}

		private void Start() {
			Application.logMessageReceived += HandleUnityLog;
		}

		private void OnDestroy() {
			Application.logMessageReceived -= HandleUnityLog;
		}
		
		private void InstantiateView() {
			if (_config.InstantiateView) {
				_consoleView = Instantiate(_config.ViewPrefab);
				_consoleView.transform.SetParent(transform, false);
			}
		}
		
		private static void CreateLog(
			string logString,LogType logType, bool doStackTrace, bool customColor, Color textColor, Color bgColor) {
			var stackTrace = doStackTrace ? new System.Diagnostics.StackTrace().ToString() : string.Empty;
			var newLog = new ConsoleLog(logString, stackTrace, logType, customColor, textColor, bgColor);
			ConsoleHistory.AddLog(newLog);
		}

		private static void CreateLog(string logString, string stackTrace, LogType logType) {
			var newLog = new ConsoleLog(logString, stackTrace, logType, false, Color.white, Color.black);
			ConsoleHistory.AddLog(newLog);
		}
		
		private static void ParseCommand(string commandString) {
			ConsoleHistory.AddCommandHistory(commandString);

			commandString = commandString.Trim();

			List<string> cmdSplit = ParseArguments(commandString);

			string cmdName = cmdSplit[0].ToLower();
			cmdSplit.RemoveAt(0);

			ConsoleLog newLog = new ConsoleLog("> " + commandString, "", LogType.Log, false, Color.white, Color.black);
			ConsoleHistory.AddLog(newLog);

			try {
				_commands[cmdName].Handler(cmdSplit.ToArray());
			}
			catch (KeyNotFoundException) {
				LogError(String.Format("Command \"{0}\" not found.", cmdName));
			}
			catch (Exception) {
				
			}
		}

		private static List<string> ParseArguments(string commandString) {
			var args = new List<string>();

			foreach (Match match in Regex.Matches(commandString, RegexStringSplit)) {
				string value = match.Value.Trim();

				if (IsWrappedInQuotes.IsMatch(value)) { value = value.Substring(1, value.Length - 2); }

				args.Add(value);
			}

			return args;
		}
		
		private void HandleUnityLog(string logString, string stackTrace, LogType logType) {
			switch (logType) {
				case LogType.Error:
					if (_config.LogUnityErrors == false) {
						return;
					}
					break;
				case LogType.Assert:
					if (_config.LogUnityAsserts == false) {
						return;
					}
					break;
				case LogType.Warning:
					if (_config.LogUnityWarnings == false) {
						return;
					}
					break;
				case LogType.Log:
					if (_config.LogUnityLogs == false) {
						return;
					}
					break;
				case LogType.Exception:
					if (_config.LogUnityExceptions == false) {
						return;
					}
					break;
			}
			
			CreateLog(logString, stackTrace, logType);
			
		}
#endregion Private Methods
		
	}
}