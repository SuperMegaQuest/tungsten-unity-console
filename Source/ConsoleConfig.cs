using UnityEngine;

namespace Gruel {
	[CreateAssetMenu(menuName = "Gruel/Console/Console Config Asset", fileName = "ConsoleConfig")]
	public class ConsoleConfig : ScriptableObject {
		
#region Properties
		public bool DontDestroyOnLoad => _dontDestroyOnLoad;

		public ConsoleHistory ConsoleHistory => _consoleHistory;

		public int LogHistoryCapacity => _logHistoryCapacity;

		public int CommandHistoryCapacity => _commandHistoryCapacity;

		public bool EnableCoreCommands => _enableCoreCommands;

		public bool LogUnityErrors => _logUnityErrors;
		public bool LogUnityAsserts => _logUnityAsserts;
		public bool LogUnityWarnings => _logUnityWarnings;
		public bool LogUnityLogs => _logUnityLogs;
		public bool LogUnityExceptions => _logUnityExceptions;

		public bool InstantiateView => _instantiateView;

		public ConsoleView ViewPrefab => _viewPrefab;
#endregion Properties

#region Fields
		[Header("Instantiation")]
		[SerializeField] private bool _dontDestroyOnLoad = true;
		
		[Header("History")]
		[SerializeField] private ConsoleHistory _consoleHistory;
		[Tooltip("Set to -1 to not limit the number of logs stored")]
		[SerializeField] private int _logHistoryCapacity = -1;
		[Tooltip("Set to -1 to not limit the number of commands stored")]
		[SerializeField] private int _commandHistoryCapacity = -1;

		[Header("Core Commands")]
		[SerializeField] private bool _enableCoreCommands = true;

		[Header("Unity Log Settings")]
		[SerializeField] private bool _logUnityErrors = true;
		[SerializeField] private bool _logUnityAsserts = true;
		[SerializeField] private bool _logUnityWarnings = true;
		[SerializeField] private bool _logUnityLogs = true;
		[SerializeField] private bool _logUnityExceptions = true;
		
		[Header("Console View")]
		[SerializeField] private bool _instantiateView = true;
		[Tooltip("Select which console view implementation to use.")]
		[SerializeField] private ConsoleView _viewPrefab;
#endregion Fields
		
	}
}