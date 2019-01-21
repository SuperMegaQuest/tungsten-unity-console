using System;
using System.Collections.Generic;
using UnityEngine;

namespace HUDConsole {
	[CreateAssetMenu(menuName = "HUDConsole/Console History Asset", fileName = "ConsoleHistory")]
	public class ConsoleHistory : ScriptableObject {
		
		[SerializeField] private ConsoleConfig _config;
		
#region Command
		private List<string> _commandHistory = new List<string>();
		
		public void CommandHistoryAdd(string commandString) {
			if (_config._commandHistoryCapacity != -1
			&& _commandHistory.Count >= _config._commandHistoryCapacity) {
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
		private Action logHistoryChanged;
		private List<ConsoleLog> _logHistory = new List<ConsoleLog>();
		
		public void LogAdd(ConsoleLog consoleLog) {
			if (_config._logHistoryCapacity != -1
			&& _logHistory.Count >= _config._logHistoryCapacity) {
				_logHistory.RemoveAt(0);
			}
			
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

	}
}