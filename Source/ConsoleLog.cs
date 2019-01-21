using UnityEngine;

namespace HUDConsole {
	
public struct ConsoleLog {
	public string logString { get; private set; }
	public string stackTrace { get; private set; }
	public LogType logType { get; private set; }
	public bool customColor { get; private set; }
	public Color textColor { get; private set; }
	public Color bgColor { get; private set; }

	public ConsoleLog(string logString, string stackTrace, LogType logType, bool customColor, Color textColor, Color bgColor) : this() {
		this.logString = logString;
		this.stackTrace = stackTrace;
		this.logType = logType;
		this.customColor = customColor;
		this.textColor = textColor;
		this.bgColor = bgColor;
	}
}
	
}