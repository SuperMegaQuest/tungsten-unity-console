using UnityEngine;

namespace Gruel {
public static class ConsoleCoreCommands {

#region Public Methods
    public static void AddCoreCommands() {
        Console.AddCommand("Console.Log", ConsoleLog, "Display message to console.");
        Console.AddCommand("Console.LogWarning", ConsoleLogWarning, "Display warning message to console.");
        Console.AddCommand("Console.LogError", ConsoleLogError, "Display error message to console.");
        Console.AddCommand("Console.Save", ConsoleSave, "Save console to log file.");
        Console.AddCommand("Console.Copy", ConsoleCopy, "Copy console to clipboard.");
        Console.AddCommand("Console.Clear", ConsoleClear, "Clear console.");
        Console.AddCommand("Help", Help, "List of commands and their help text");

        Console.AddCommand("Debug.Log", DebugLog, "Logs message to the Unity Console.");
        Console.AddCommand("Debug.LogWarning", DebugLogWarning,
                           "A variant of Debug.Log that logs a warning message to the console.");
        Console.AddCommand("Debug.LogError", DebugLogError,
                           "A variant of Debug.Log that logs an error message to the console.");
        Console.AddCommand("Debug.Break", DebugBreak, "Pauses the editor.");
    }
#endregion Public Methods

#region Private Methods
    private static void ConsoleLog(string[] args) {
        var output = "";
        for (int i = 0, n = args.Length; i < n; i++) {
            output += args[i] + " ";
        }

        Console.Log(output);
    }

    private static void ConsoleLogWarning(string[] args) {
        var output = "";
        for (int i = 0, n = args.Length; i < n; i++) {
            output += args[i] + " ";
        }

        Console.LogWarning(output);
    }

    private static void ConsoleLogError(string[] args) {
        var output = "";
        for (int i = 0, n = args.Length; i < n; i++) {
            output += args[i] + " ";
        }

        Console.LogError(output);
    }

    private static void ConsoleSave(string[] args) {
        Console.Log($"Saved to {Console.SaveHistoryToLogFile(args == null || args.Length == 0 ? "" : args[0])}");
    }

    private static void ConsoleCopy(string[] args) {
        Console.CopyHistoryToClipboard();
    }

    private static void ConsoleClear(string[] args) {
        Console.ClearConsoleView();
    }

    private static void Help(string[] args) {
        Console.PrintHelpText();
    }

    private static void DebugLog(string[] args) {
        var output = "";
        for (int i = 0, n = args.Length; i < n; i++) {
            output += $"{args[i]} ";
        }

        Debug.Log(output);
    }

    private static void DebugLogWarning(string[] args) {
        var output = "";
        for (int i = 0, n = args.Length; i < n; i++) {
            output += $"{args[i]} ";
        }

        Debug.LogWarning(output);
    }

    private static void DebugLogError(string[] args) {
        var output = "";
        for (int i = 0, n = args.Length; i < n; i++) {
            output += $"{args[i]} ";
        }

        Debug.LogError(output);
    }

    private static void DebugBreak(string[] args) {
        Debug.Break();
    }
#endregion Private Methods

}
}