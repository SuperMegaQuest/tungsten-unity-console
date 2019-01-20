using System;
using System.Collections.Generic;
using UnityEngine;

namespace HUDConsole {
	[CreateAssetMenu(menuName = "HUDConsole/Console History Asset", fileName = "ConsoleHistory")]
	public class ConsoleHistory : ScriptableObject {
#region Public
#region Command
		public void CommandHistoryAdd(string commandString) {
			if (_commandHistory.Count >= _commandHistoryMax) {
				_commandHistory.RemoveAt(0);
			}

			_commandHistory.Add(commandString);
		}

		public string CommandHistoryGet(int index) {
			return _commandHistory[index];
		}

		public int CommandHistoryCount() {
			return _commandHistory.Count;
		}
#endregion Command

#region Log
		public void LogAdd(ConsoleLog consoleLog) {
			_logHistory.Add(consoleLog);
			logHistoryChanged();
		}

		public ConsoleLog LogGetLatest() {
			return _logHistory[_logHistory.Count - 1];
		}

		public List<ConsoleLog> LogGetAll() {
			return _logHistory;
		}

		public void LogAddListener(Action callback) {
			logHistoryChanged += callback;
		}

		public void LogRemoveListener(Action callback) {
			logHistoryChanged -= callback;
		}
#endregion Log
#endregion Public

#region Private
#region Command
		[SerializeField]
		private int _commandHistoryMax = 32;

		private List<string> _commandHistory = new List<string>();
#endregion Command

#region Log
		private Action logHistoryChanged;
		private List<ConsoleLog> _logHistory = new List<ConsoleLog>();
#endregion Log
#endregion Private
	}
}