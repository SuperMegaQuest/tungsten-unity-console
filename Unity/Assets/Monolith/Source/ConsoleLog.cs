using UnityEngine;

namespace Monolith {
public class ConsoleLog {

#region Constructor
    public ConsoleLog(string logString, string stackTrace, LogType logType, bool customColor, Color textColor,
                      Color bgColor) {
        LogString = logString;
        StackTrace = stackTrace;
        LogType = logType;
        CustomColor = customColor;
        TextColor = textColor;
        BgColor = bgColor;
    }
#endregion Constructor

#region Properties
    public string LogString { get; }
    public string StackTrace { get; }
    public LogType LogType { get; }
    public bool CustomColor { get; }
    public Color TextColor { get; }
    public Color BgColor { get; }
#endregion Properties

}
}