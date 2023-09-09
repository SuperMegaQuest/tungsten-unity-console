using System;

using UnityEngine;

namespace Monolith {

[Serializable]
public struct ConsoleLog {

#region Public
    public readonly string logString;
    public readonly string stackTrace;
    public readonly LogType logType;
    public readonly bool customColor;
    public readonly Color textColor;
    public readonly Color bgColor;

    public ConsoleLog(
    string logString,
    string stackTrace,
    LogType logType,
    bool customColor,
    Color textColor,
    Color bgColor) {
        this.logString = logString;
        this.stackTrace = stackTrace;
        this.logType = logType;
        this.customColor = customColor;
        this.textColor = textColor;
        this.bgColor = bgColor;
    }
#endregion Public

}
}