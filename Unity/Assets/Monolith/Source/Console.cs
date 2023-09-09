using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Monolith {
public class Console : MonoBehaviour {

#region Properties
    public static bool IsActive => _instance != null;

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
    private const string REGEX_DRIVE_PATH = @"^\w:(?:\\|\/)?$";

    /// <summary>
    /// Tests whether a string is a drive root. e.g. "D:\"
    /// </summary>
    private static readonly Regex IsDrivePath = new(REGEX_DRIVE_PATH);

    /// <summary>
    /// Splits a string by spaces, unless surrounded by (unescaped) single or double quotes.
    /// </summary>
    private const string REGEX_STRING_SPLIT = @"(""[^""\\]*(?:\\.[^""\\]*)*""|'[^'\\]*(?:\\.[^'\\]*)*'|[\S]+)+";

    /// <summary>
    /// Tests whether a string starts and ends with either double or single quotes (not a mix).
    /// </summary>
    private const string REGEX_QUOTE_WRAPPED = @"^"".*""$|^'.*'$";

    /// <summary>
    /// Tests whether a string starts and ends with either double or single quotes (not a mix).
    /// </summary>
    private static readonly Regex IsWrappedInQuotes = new(REGEX_QUOTE_WRAPPED);

    private static Console _instance;

    private ConsoleView[] _views;

    private string _helpTextFormat = "{0} : {1}";

    private static readonly Dictionary<string, ConsoleCommand> _commands = new();
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
        return _commands.Values.OrderBy(c => c.commandName).ToList();
    }

    public static List<ConsoleLog> GetHistoryConsoleLogs() {
        return ConsoleHistory.LogHistory;
    }

    public static string GetHistoryString(bool stripRichText = false) {
        var history = GetHistoryConsoleLogs();
        var stringBuilder = new StringBuilder();

        foreach (var log in history) {
            stringBuilder.AppendLine(log.logString.Trim());
            if (log.stackTrace != "") {
                stringBuilder.AppendLine(log.stackTrace.Trim());
            }

            stringBuilder.Append(Environment.NewLine);
        }

        return (stripRichText
            ? Regex.Replace(stringBuilder.ToString(), "<.*?>", string.Empty)
            : stringBuilder.ToString()).Trim();
    }

    /// <summary>
    ///     Copy console history to the clipboard (<see cref="GUIUtility.systemCopyBuffer" />).
    /// </summary>
    public static void CopyHistoryToClipboard(bool stripRichText = false) {
        GUIUtility.systemCopyBuffer = GetHistoryString(stripRichText);
    }

    public static void ClearConsoleView() {
        for(int i = 0; i < _instance._views.Length; i++) {
            _instance._views[i].ClearConsoleView();
        }
    }

    public static void PrintHelpText() {
        foreach (var command in _commands.Values.OrderBy(c => c.commandName)) {
            Log(string.Format(_instance._helpTextFormat, command.commandName, command.helpText), LogType.Log, false);
        }
    }

    /// <summary>
    ///     Save console history to a log file and return the file's path.
    /// </summary>
    public static string SaveHistoryToLogFile(string path = "", string prefix = "console", bool stripRichText = false) {
        path = path.Trim();

        if (path == string.Empty) {
            path = Application.persistentDataPath;
        }
        else if (path.EndsWith(":")) {
            path += "\\";
        }
        else if (path.EndsWith("/")) {
            path = path.Replace("/", "\\");
        }

        if (IsDrivePath.IsMatch(path)) {
            if (Directory.GetLogicalDrives()
                         .All(drive => !drive.Equals(path, StringComparison.CurrentCultureIgnoreCase))) {
                LogError($"Drive not found: {path}");

                throw new Exception("Drive not found");
            }

            path = string.Format("{0}:", path[0]);
        }
        else if (Directory.Exists(Path.GetDirectoryName(path)) == false) {
            LogError($"Directory not found: {path}");

            throw new Exception("Directory not found");
        }

        path = $"{path}/{prefix}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log";
        File.WriteAllText(path, GetHistoryString(stripRichText));

        return path.Replace("\\", "/");
    }
#endregion Public Methods

#region Private Methods
    private void Awake() {
        // Setup instance.
        if (_instance == null) {
            _instance = this;
        }
        else {
            Debug.LogError("[Tungsten] [Console] there is already an instance of Console!");
            Destroy(gameObject);
        }

        // instantiate front-ends
        InstantiateViews();

        // Set dont destroy on load to this object.
        if (_config.SurviveSceneChanges) {
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

    private void InstantiateViews() {
        _views = new ConsoleView[_config.Views.Length];
        for(int i = 0; i < _config.Views.Length; i++) {
            _views[i] = Instantiate(_config.Views[i], transform, false);
        }
    }

    private static void CreateLog(
        string logString, LogType logType, bool doStackTrace, bool customColor, Color textColor, Color bgColor) {
        var stackTrace = doStackTrace ? new StackTrace().ToString() : string.Empty;
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

        var cmdName = cmdSplit[0].ToLower();
        cmdSplit.RemoveAt(0);

        var newLog = new ConsoleLog("> " + commandString, "", LogType.Log, false, Color.white, Color.black);
        ConsoleHistory.AddLog(newLog);

        try {
            _commands[cmdName].handler(cmdSplit.ToArray());
        }
        catch (KeyNotFoundException) {
            LogError($"Command \"{cmdName}\" not found.");
        }
        catch (Exception) { }
    }

    private static List<string> ParseArguments(string commandString) {
        List<string> args = new();

        foreach (Match match in Regex.Matches(commandString, REGEX_STRING_SPLIT)) {
            var value = match.Value.Trim();

            if (IsWrappedInQuotes.IsMatch(value)) {
                value = value.Substring(1, value.Length - 2);
            }

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
            default:
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