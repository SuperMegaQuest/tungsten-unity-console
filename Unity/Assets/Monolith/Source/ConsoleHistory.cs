using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gruel {
	[CreateAssetMenu(menuName = "Gruel/Console/Console History Asset", fileName = "ConsoleHistory")]
	public class ConsoleHistory : ScriptableObject {

#region Properties
		public List<ConsoleLog> LogHistory {
			get => _logHistory;
		}

		public ConsoleLog LatestLog {
			get => _logHistory[_logHistory.Count - 1];
		}

		public int CommandHistoryCount {
			get => _commandHistory.Count;
		}
#endregion Properties

#region Fields
		[SerializeField] private ConsoleConfig _config;

		private Action _logHistoryChanged;
		private List<ConsoleLog> _logHistory = new List<ConsoleLog>();
		private List<string> _commandHistory = new List<string>();
#endregion Fields

#region Public Methods
		public void AddLogAddedListener(Action callback) {
			_logHistoryChanged += callback;
		}

		public void RemoveLogAddedListener(Action callback) {
			_logHistoryChanged -= callback;
		}

		public void AddLog(ConsoleLog consoleLog) {
			if (_config.LogHistoryCapacity != -1
			&& _logHistory.Count >= _config.LogHistoryCapacity) {
				_logHistory.RemoveAt(0);
			}

			_logHistory.Add(consoleLog);
			_logHistoryChanged?.Invoke();
		}

		public void AddCommandHistory(string commandString) {
			if (_config.CommandHistoryCapacity != -1
			&& _commandHistory.Count >= _config.CommandHistoryCapacity) {
				_commandHistory.RemoveAt(0);
			}

			_commandHistory.Add(commandString);
		}

		public string GetCommandHistoryWithIndex(int index) {
			return _commandHistory[index];
		}
#endregion Public Methods

	}
}