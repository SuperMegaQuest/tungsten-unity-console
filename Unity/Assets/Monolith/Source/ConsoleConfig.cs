using UnityEngine;

namespace Monolith {
[CreateAssetMenu(menuName = "Monolith/Console/Console Config Asset", fileName = "ConsoleConfig")]
public class ConsoleConfig : ScriptableObject {

#region Public
    public bool SurviveSceneChanges => _surviveSceneChanges;
    public ConsoleHistory ConsoleHistory => _consoleHistory;
    public int LogHistoryCapacity => _logHistoryCapacity;
    public int CommandHistoryCapacity => _commandHistoryCapacity;
    public bool EnableCoreCommands => _enableCoreCommands;
    public bool LogUnityErrors => _logUnityErrors;
    public bool LogUnityAsserts => _logUnityAsserts;
    public bool LogUnityWarnings => _logUnityWarnings;
    public bool LogUnityLogs => _logUnityLogs;
    public bool LogUnityExceptions => _logUnityExceptions;
    public ConsoleView[] Views => _views;
#endregion Public

#region Private
    [Header("Instantiation")]
    [SerializeField] private bool _surviveSceneChanges = true;

    [Header("History")]
    [SerializeField] private ConsoleHistory _consoleHistory;

    [Tooltip("Set to -1 to not limit the number of logs stored")]
    [SerializeField] private int _logHistoryCapacity = -1;

    [Tooltip("Set to -1 to not limit the number of commands stored")]
    [SerializeField] private int _commandHistoryCapacity = -1;

    [Header("Core Commands")]
    [SerializeField] private bool _enableCoreCommands = true;

    [Header("Unity Log Settings")]
    [SerializeField] bool _logUnityErrors = true;

    [SerializeField] private bool _logUnityAsserts = true;
    [SerializeField] private bool _logUnityWarnings = true;
    [SerializeField] private bool _logUnityLogs = true;
    [SerializeField] private bool _logUnityExceptions = true;

    [Header("Views")]
    [Tooltip("Select which console view implementations to use")]
    [SerializeField] private ConsoleView[] _views;
#endregion Private

}
}