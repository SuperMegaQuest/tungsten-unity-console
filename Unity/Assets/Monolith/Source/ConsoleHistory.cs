using System;
using System.Collections.Generic;

using UnityEngine;

namespace Monolith {
[CreateAssetMenu(menuName = "Monolith/Console/Console History Asset", fileName = "ConsoleHistory")]
public class ConsoleHistory : ScriptableObject {

#region Properties
    public List<ConsoleLog> LogHistory { get; } = new();

    public ConsoleLog LatestLog => LogHistory[^1];

    public int CommandHistoryCount => _commandHistory.Count;
#endregion Properties

#region Fields
    [SerializeField] private ConsoleConfig _config;

    private Action _logHistoryChanged;
    private readonly List<string> _commandHistory = new();
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
         && LogHistory.Count >= _config.LogHistoryCapacity) {
            LogHistory.RemoveAt(0);
        }

        LogHistory.Add(consoleLog);
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