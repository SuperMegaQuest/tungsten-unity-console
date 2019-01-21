using UnityEngine;

namespace HUDConsole {
public abstract class ConsoleViewAbstract : MonoBehaviour {
	
	public abstract bool IsActive { get; protected set; }

	public virtual void ClearConsoleView() { }

	protected virtual void Awake() {
		Console.ConsoleHistory.LogAddListener(OnConsoleLogHistoryChanged);
	}

	protected virtual void OnDestroy() {
		Console.ConsoleHistory.LogRemoveListener(OnConsoleLogHistoryChanged);
	}

	protected virtual void OnConsoleLogHistoryChanged() { }
	
}
}