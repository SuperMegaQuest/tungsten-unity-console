using System;
using System.Collections.Generic;
using UnityEngine;

namespace HUDConsole {
	[CreateAssetMenu(menuName = "Console/Console History Asset", fileName = "ConsoleHistory")]
	public class ConsoleHistory : ScriptableObject {
#region Public
	#region Command
		public void CommandHistoryAdd(string commandString) {
			if(m_commandHistory.Count >= m_commandHistoryMax) {
				m_commandHistory.RemoveAt(0);
			}

			m_commandHistory.Add(commandString);
		}

		public string CommandHistoryGet(int index) {
			return m_commandHistory[index];
		}

		public int CommandHistoryCount() {
			return m_commandHistory.Count;
		}
	#endregion Command

	#region Log
		public void LogAdd(ConsoleLog consoleLog) {
			m_logHistory.Add(consoleLog);
			logHistoryChanged();
		}

		public ConsoleLog LogGetLatest() {
			return m_logHistory[m_logHistory.Count - 1];
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
		[SerializeField] private int m_commandHistoryMax = 32;
		private List<string> m_commandHistory = new List<string>();
	#endregion Command

	#region Log
		private Action logHistoryChanged;
		private List<ConsoleLog> m_logHistory = new List<ConsoleLog>(); 
	#endregion Log
#endregion Private
	}
}