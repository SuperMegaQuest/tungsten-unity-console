using UnityEngine;

namespace Gruel {
	public abstract class ConsoleView : MonoBehaviour {

#region Properties
		public abstract bool IsActive { get; protected set; }
#endregion Properties

#region Public Methods
		public virtual void ClearConsoleView() { }
#endregion Public Methods

#region Private Methods
		protected virtual void Awake() {
			Console.ConsoleHistory.AddLogAddedListener(OnConsoleLogHistoryChanged);
		}

		protected virtual void OnDestroy() {
			Console.ConsoleHistory.RemoveLogAddedListener(OnConsoleLogHistoryChanged);
		}
		
		protected virtual void OnConsoleLogHistoryChanged() { }
#endregion Private Methods
		
	}
}