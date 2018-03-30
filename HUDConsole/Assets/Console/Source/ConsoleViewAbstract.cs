using UnityEngine;

namespace HUDConsole {
	public abstract class ConsoleViewAbstract : MonoBehaviour {
#region Public
		public abstract bool IsActive { get; protected set; }

		public virtual void ClearConsoleView() {

		}
#endregion Public

#region Private
		protected virtual void Awake() {
			Console.ConsoleHistory.LogAddListener(OnConsoleLogHistoryChanged);
		}

		protected virtual void OnDestroy() {
			Console.ConsoleHistory.LogRemoveListener(OnConsoleLogHistoryChanged);
		}

		protected virtual void OnConsoleLogHistoryChanged() {
			
		}
#endregion Private
	}
}