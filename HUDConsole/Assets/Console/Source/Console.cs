using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace HUDConsole {
	public class Console : MonoBehaviour {
#region Public
		public static bool IsActive {
			get { return m_instance != null && m_instance.m_consoleView.IsActive; }
		}

		public static ConsoleHistory ConsoleHistory {
			get { return m_instance.m_consoleHistory; }
		}

		public static void AddCommand(string commandName, CommandHandler handler, string helpText) {
			m_commands.Add(commandName.ToLowerInvariant(), new ConsoleCommand(commandName, handler, helpText));
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
			return m_commands.Values.OrderBy(c => c.commandName).ToList();
		}

		public static List<ConsoleLog> GetHistoryConsoleLogs() {
			return ConsoleHistory.LogGetAll();
		}

		public static string GetHistoryString(bool stripRichText = false) {
			List<ConsoleLog> history = GetHistoryConsoleLogs();
			StringBuilder stringBuilder = new StringBuilder();

			foreach (ConsoleLog log in history) {
				stringBuilder.AppendLine(log.logString.Trim());
				if(log.stackTrace != "") { stringBuilder.AppendLine(log.stackTrace.Trim()); }
				stringBuilder.Append(Environment.NewLine);
			}

			return (stripRichText ? Regex.Replace(stringBuilder.ToString(), "<.*?>", string.Empty) : stringBuilder.ToString()).Trim();
		}

		/// <summary>Save console history to a log file in <see cref="Application.persistentDataPath"/> and return the file's path.</summary>
		public static string SaveHistoryToLogFile(string prefix = "console", bool stripRichText = false) {
			string path = string.Format("{0}/{1}_{2:yyyy-MM-dd_HH-mm-ss}.log", Application.persistentDataPath, prefix, DateTime.Now);
			File.WriteAllText(path, GetHistoryString(stripRichText));
			return path;
		}

		/// <summary>Copy console history to the clipboard (<see cref="GUIUtility.systemCopyBuffer"/>).</summary>
		public static void CopyHistoryToClipboard(bool stripRichText = false) {
			GUIUtility.systemCopyBuffer = GetHistoryString(stripRichText);
		}

		public static void ClearConsoleView() {
			m_instance.m_consoleView.ClearConsoleView();
		}

		public static void SetHelpTextFormat(string helpTextFormat) {
			m_helpTextFormat = helpTextFormat;
		}

		public static void PrintHelpText() {
			foreach (var command in m_commands.Values.OrderBy(c => c.commandName)) {
				Log(string.Format(m_helpTextFormat, command.commandName, command.helpText), LogType.Log, false);
			}
		}
#endregion Public

#region Private
		private static Console m_instance = null;

		private static string m_helpTextFormat = "{0} : {1}";

		[Header("History")]
		[SerializeField] private ConsoleHistory m_consoleHistory;

		[Header("Default Commands")]
		[SerializeField] private bool m_enableDefaultCommands = true;

		[Header("Unity Log Settings")]
		[SerializeField] private bool m_logUnityErrors = true;
		[SerializeField] private bool m_logUnityAsserts = true;
		[SerializeField] private bool m_logUnityWarnings = true;
		[SerializeField] private bool m_logUnityLogs = true;
		[SerializeField] private bool m_logUnityExceptions = true;

		[Header("Console View")]
		[Tooltip("Select which console view implementation to use.")]
		[SerializeField] private ConsoleViewAbstract m_consoleViewPrefab;
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
			m_consoleView = Instantiate(m_consoleViewPrefab);
			m_consoleView.transform.SetParent(transform, false);

			if (m_enableDefaultCommands == false) { return; }

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
		private static Dictionary<string, ConsoleCommand> m_commands = new Dictionary<string, ConsoleCommand>();

		/// <summary>Splits a string by spaces, unless surrounded by (unescaped) single or double quotes.</summary>
		const string m_regexStringSplit = @"(""[^""\\]*(?:\\.[^""\\]*)*""|'[^'\\]*(?:\\.[^'\\]*)*'|[\S]+)+";
		/// <summary>Tests whether a string starts and ends with either double or single quotes (not a mix).</summary>
		const string m_regexQuoteWrapped = @"^"".*""$|^'.*'$";
		/// <summary>Tests whether a string starts and ends with either double or single quotes (not a mix).</summary>
		static readonly Regex m_isWrappedInQuotes = new Regex(m_regexQuoteWrapped);

		private static void ParseCommand(string commandString) {
			ConsoleHistory.CommandHistoryAdd(commandString);

			commandString = commandString.Trim();

			List<string> cmdSplit = ParseArguments(commandString);

			string cmdName = cmdSplit[0].ToLower();
			cmdSplit.RemoveAt(0);

			ConsoleLog newLog = new ConsoleLog("> " + commandString, "", LogType.Log, false, Color.white, Color.black);
			ConsoleHistory.LogAdd(newLog);

			try {
				m_commands[cmdName].handler(cmdSplit.ToArray());
			}
			catch(KeyNotFoundException) {
				LogError(String.Format("Command \"{0}\" not found.", cmdName));
			}
			catch(Exception) {

			}
		}

		static List<string> ParseArguments(string commandString) {
			List<string> args = new List<string>();

			foreach (Match match in Regex.Matches(commandString, m_regexStringSplit)) {
				string value = match.Value.Trim();

				if (m_isWrappedInQuotes.IsMatch(value)) { value = value.Substring(1, value.Length - 2); }

				args.Add(Regex.Unescape(value));
			}

			return args;
		}
	#endregion Commands

	#region Logs
		private static void CreateLog(string logString, LogType logType, bool doStackTrace, bool customColor, Color textColor, Color bgColor) {
			var stackTrace = "";
			if(doStackTrace) {
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
			switch(logType) {
				case LogType.Error: {
					if(m_logUnityErrors) {
						CreateLog(logString, stackTrace, logType);
					}
					break;
				}
				case LogType.Assert: {
					if(m_logUnityAsserts) {
						CreateLog(logString, stackTrace, logType);
					}
					break;
				}
				case LogType.Warning: {
					if(m_logUnityWarnings) {
						CreateLog(logString, stackTrace, logType);
					}
					break;
				}
				case LogType.Log: {
					if(m_logUnityLogs) {
						CreateLog(logString, stackTrace, logType);
					}
					break;
				}
				case LogType.Exception: {
					if(m_logUnityExceptions) {
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