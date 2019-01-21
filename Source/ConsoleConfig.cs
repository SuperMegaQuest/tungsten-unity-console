using UnityEngine;

namespace HUDConsole {
	
	[CreateAssetMenu(menuName = "HUDConsole/Console Config Asset", fileName = "ConsoleConfig")]
	public class ConsoleConfig : ScriptableObject {
		[Header("History")]
		public ConsoleHistory _consoleHistory;
		[Tooltip("Set to -1 to not limit the number of logs stored")] public int _logHistoryCapacity = -1;
		[Tooltip("Set to -1 to not limit the number of commands stored")] public int _commandHistoryCapacity = -1;

		[Header("Core Commands")]
		public bool _enableCoreCommands = true;

		[Header("Unity Log Settings")]
		public bool _logUnityErrors = true;
		public bool _logUnityAsserts = true;
		public bool _logUnityWarnings = true;
		public bool _logUnityLogs = true;
		public bool _logUnityExceptions = true;

		[Header("Console View")]
		[Tooltip("Select which console view implementation to use.")]
		public ConsoleViewAbstract _consoleViewPrefab;
	}
	
}