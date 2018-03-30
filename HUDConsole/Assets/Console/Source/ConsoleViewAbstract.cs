using UnityEngine;

namespace HUDConsole {
	public abstract class ConsoleViewAbstract : MonoBehaviour {
#region Public
		public abstract bool isActive { get; protected set; }

		public virtual void ClearConsoleView() {

		}
#endregion Public

#region Private
		protected virtual void Awake() {
			Console.consoleHistory.LogAddListener(OnConsoleLogHistoryChanged);
		}

		protected virtual void OnDestroy() {
			Console.consoleHistory.LogRemoveListener(OnConsoleLogHistoryChanged);
		}

		protected virtual void OnConsoleLogHistoryChanged() {
			
		}
#endregion Private
	}
}