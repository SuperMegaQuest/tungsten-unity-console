using UnityEngine;

namespace HUDConsole {
	public struct ConsoleLog { 
		public ConsoleLog(string logString, string stackTrace, LogType logType, bool customColor, Color textColor, Color bgColor) {
			this.logString = logString;
			this.stackTraceString = stackTrace;
			this.logType = logType;
			this.customColor = customColor;
			this.textColor = textColor;
			this.bgColor = bgColor;
		}

		public string logString {
			get; private set;
		}

		public string stackTraceString {
			get; private set;
		}

		public LogType logType {
			get; private set;
		}

		public bool customColor {
			get; private set;
		}

		public Color textColor {
			get; private set;
		}

		public Color bgColor {
			get; private set;
		}
	}
}