using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Monolith.UGUI {
public class Console : ConsoleView {

#region Properties
    public override bool IsActive { get; protected set; }
#endregion Properties

#region Fields
    [Header("Main"), SerializeField,]
    private RectTransform _container;

    [SerializeField] private ListenForDimensionsChange _containerListener;
    [SerializeField] private Transform _obeliskLogPoolContainer;
    [SerializeField] private Image _containerBackgroundImage;
    [SerializeField] private Outline _outline;
    [SerializeField] private Image _resizeHandleImage;

    [Header("Titlebar"), SerializeField,]
    private Image _titlebarBackgroundImage;

    [SerializeField] private Image _titlebarIconImage;
    [SerializeField] private Text _titlebarTitleText;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Image _closeButtonBackgroundImage;
    [SerializeField] private Image _closeButtonIconImage;

    [Header("Filter"), SerializeField,]
    private FilterDropdown _filterDropdown;

    [Header("Log"), SerializeField,]
    private ScrollRect _scrollRect;

    [SerializeField] private RectTransform _logLayout;
    [SerializeField] private Scrollbar _scrollbar;
    [SerializeField] private Image _scrollbarBackgroundImage;

    [Header("Input"), SerializeField,]
    private Image _inputContainerBackgroundImage;

    [Header("Command"), SerializeField,]
    private RectTransform _commandContainer;

    [SerializeField] private Image _commandSymbolBackgroundImage;
    [SerializeField] private Image _commandSymbolImage;
    [SerializeField] private InputField _commandInputField;
    [SerializeField] private Image _commandInputFieldImage;
    [SerializeField] private Text _commandInputFieldText;

    [Header("Search"), SerializeField,]
    private RectTransform _searchContainer;

    [SerializeField] private Image _searchSymbolBackgroundImage;
    [SerializeField] private Image _searchSymbolImage;
    [SerializeField] private InputField _searchInputField;
    [SerializeField] private Image _searchInputFieldImage;
    [SerializeField] private Text _searchInputFieldText;

    [Header("Settings"), SerializeField,]
    private int _logViewHistoryMax = 64;

    [FormerlySerializedAs("_obeliskLogPrefab"),Header("Obelisk Prefabs"), SerializeField,]
    private Log logPrefab;

    [FormerlySerializedAs("_obeliskStackTracePrefab"),SerializeField] private StackTrace stackTracePrefab;

    [Header("Obelisk Color Set"), Tooltip("Color set objects.\nMust be in same order as DefaultViewColorSetName enum."),
     SerializeField,]
    private ColorSet _colorSetAsset;

    private readonly List<Log> _logViewHistory = new();
    private readonly Queue<Log> _obeliskLogPool = new();
    private StackTrace _stackTrace;
    private ColorSet _instantiatedColorSet;

    private int _commandHistoryDelta;
#endregion Fields

#region Public Methods
    public override void ClearConsoleView() {
        base.ClearConsoleView();

        for (var i = 0; i < _logViewHistory.Count; i++) {
            PoolLog(_logViewHistory[i]);
        }

        _logViewHistory.Clear();

        ResizeLogLayout();
    }

    public void OpenStackTraceForLog(ConsoleLog consoleLog) {
        _stackTrace.Open(consoleLog);
    }
#endregion Public Methods

#region Private Methods
    protected override void Awake() {
        base.Awake();

        _instantiatedColorSet = Instantiate(_colorSetAsset);

        // Main.
        _containerListener.AddDimensionsChangedListener(OnContainerRectTransformDimensionsChange);

        // Titlebar.
        _closeButton.onClick.AddListener(() => CloseButtonHandler(_closeButton));

        // Filter.
        _filterDropdown.AddFilterChangedListener(FilterUpdated);
        _filterDropdown.ColorSet = _instantiatedColorSet;

        // Search.
        _searchInputField.onValueChanged.AddListener(evt => SearchInputFieldUpdated(_searchInputField));

        // Fill log pool.
        FillPool();

        // Apply color set to main window.
        ApplyColorSet();

        // StackTrace window.
        _stackTrace = Instantiate(stackTracePrefab, transform.parent, false);
        _stackTrace.ColorSet = _instantiatedColorSet;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.BackQuote)) {
            ToggleState();
        }

        if (_container.gameObject.activeSelf
         && EventSystem.current.currentSelectedGameObject == _commandInputField.gameObject) {
            // Submit command.
            if (Input.GetKeyDown(KeyCode.Return)
             || Input.GetKeyDown(KeyCode.KeypadEnter)) {
                CommandSubmit();
            }

            // Previous Command +
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                CommandSetPrevious(1);
            }

            // Previous Command -
            if (Input.GetKeyDown(KeyCode.DownArrow)) {
                CommandSetPrevious(-1);
            }

            if (Input.GetKeyDown(KeyCode.Tab)) {
                CommandAutoComplete();
            }
        }
    }

    private void OnContainerRectTransformDimensionsChange() {
        ResizeLogLayout();
        ResizeInputContainers();
    }

    private void ApplyColorSet() {
        // Main.
        _containerBackgroundImage.color = _instantiatedColorSet.BackgroundColor;
        _outline.effectColor = _instantiatedColorSet.OutlineColor;
        _resizeHandleImage.color = _instantiatedColorSet.ButtonColor;

        // Titlebar.
        _titlebarBackgroundImage.color = _instantiatedColorSet.TitlebarBackgroundColor;
        _titlebarIconImage.color = _instantiatedColorSet.IconColor;
        _titlebarTitleText.color = _instantiatedColorSet.TitlebarTextColor;
        _closeButtonBackgroundImage.color = _instantiatedColorSet.ButtonColor;
        _closeButtonIconImage.color = _instantiatedColorSet.IconColor;

        // Scrollbar.
        _scrollbarBackgroundImage.color = _instantiatedColorSet.ScrollbarBackgroundColor;

        var scrollbarColorBlock = new ColorBlock {
            normalColor = _instantiatedColorSet.ScrollbarSliderColor,
            highlightedColor = _instantiatedColorSet.ScrollbarSliderHighlightedColor,
            pressedColor = _instantiatedColorSet.ScrollbarSliderPressedColor,
            colorMultiplier = 1.0f,
        };
        _scrollbar.colors = scrollbarColorBlock;

        // Input
        _inputContainerBackgroundImage.color = _instantiatedColorSet.InputContainerBackgroundColor;

        // Command.
        _commandSymbolBackgroundImage.color = _instantiatedColorSet.IconBackgroundColor;
        _commandSymbolImage.color = _instantiatedColorSet.IconColor;
        _commandInputFieldImage.color = _instantiatedColorSet.ButtonColor;
        _commandInputFieldText.color = _instantiatedColorSet.InputTextColor;

        // Search.
        _searchSymbolBackgroundImage.color = _instantiatedColorSet.IconBackgroundColor;
        _searchSymbolImage.color = _instantiatedColorSet.IconColor;
        _searchInputFieldImage.color = _instantiatedColorSet.ButtonColor;
        _searchInputFieldText.color = _instantiatedColorSet.InputTextColor;
    }

    private void ToggleState() {
        SetEnabled(_container.gameObject.activeSelf == false);
    }

    private void SetEnabled(bool enable) {
        _container.gameObject.SetActive(enable);

        _scrollRect.normalizedPosition = Vector2.zero;
        _commandInputField.text = "";

        if (enable) {
            _commandInputField.ActivateInputField();
        }

        IsActive = enable;
    }

    private void CloseButtonHandler(Button target) {
        ToggleState();
    }

    protected override void OnConsoleLogHistoryChanged() {
        base.OnConsoleLogHistoryChanged();

        var obeliskLog = _obeliskLogPool.Dequeue();
        obeliskLog.transform.SetParent(_logLayout, false);

        var consoleLog = Monolith.Console.ConsoleHistory.LatestLog;
        obeliskLog.SetLog(ref consoleLog);

        AddLogViewHistory(obeliskLog);
    }

    private void AddLogViewHistory(Log log) {
        if (_logViewHistory.Count >= _logViewHistoryMax) {
            PoolLog(_logViewHistory[0]);
            _logViewHistory.RemoveAt(0);
        }

        _logViewHistory.Add(log);

        ResizeLogLayout();

        _scrollRect.normalizedPosition = Vector2.zero;
    }

    private void FillPool() {
        for (var i = 0; i < _logViewHistoryMax + 1; i++) {
            var log = Instantiate(logPrefab, _obeliskLogPoolContainer, false);
            log.Init(this, _instantiatedColorSet);
            _obeliskLogPool.Enqueue(log);
        }
    }

    private void PoolLog(Log log) {
        log.transform.SetParent(_obeliskLogPoolContainer, false);
        _obeliskLogPool.Enqueue(log);
    }

    private void ResizeLogLayout() {
        var logLayoutSizeCache = _logLayout.sizeDelta;
        var logLayoutPosCache = _logLayout.localPosition;
        var minHeight = _container.sizeDelta.y - 40.0f;
        var newHeight = 0.0f;

        // Resize LogLayout so it can properly contain all the logs.
        for (var i = 0; i < _logViewHistory.Count; i++) {
            newHeight += _logViewHistory[i].Height;
        }

        _logLayout.sizeDelta = new Vector2(_logLayout.sizeDelta.x, Mathf.Clamp(newHeight, minHeight, 4096.0f));

        // Offset LogLayout relative to the size it just increased by.
        var sizeDifferenceY = _logLayout.sizeDelta.y - logLayoutSizeCache.y;
        var newPosY = logLayoutPosCache.y + sizeDifferenceY * 0.5f;
        _logLayout.localPosition = new Vector3(logLayoutPosCache.x, newPosY, 0.0f);
    }

    private void FilterUpdated() {
        for (int i = 0, n = _logViewHistory.Count; i < n; i++) {
            switch (_logViewHistory[i].ConsoleLog.LogType) {
                case LogType.Error: {
                    _logViewHistory[i].gameObject.SetActive(!_filterDropdown.GetFilterSetting(LogType.Error));
                    break;
                }

                case LogType.Assert: {
                    _logViewHistory[i].gameObject.SetActive(!_filterDropdown.GetFilterSetting(LogType.Assert));
                    break;
                }

                case LogType.Warning: {
                    _logViewHistory[i].gameObject.SetActive(!_filterDropdown.GetFilterSetting(LogType.Warning));
                    break;
                }

                case LogType.Log: {
                    _logViewHistory[i].gameObject.SetActive(!_filterDropdown.GetFilterSetting(LogType.Log));
                    break;
                }

                case LogType.Exception: {
                    _logViewHistory[i].gameObject.SetActive(!_filterDropdown.GetFilterSetting(LogType.Exception));
                    break;
                }
            }
        }
    }

    private void ResizeInputContainers() {
        var sizeDelta = _container.sizeDelta;
        _commandContainer.sizeDelta = new Vector2(sizeDelta.x * 0.68359375f, 20f);
        _searchContainer.sizeDelta = new Vector2(sizeDelta.x * 0.3125f, 20f);

        _commandContainer.anchoredPosition = new Vector2(_commandContainer.sizeDelta.x * 0.5f, 0f);
        _searchContainer.anchoredPosition = new Vector2(-_searchContainer.sizeDelta.x * 0.5f, 0f);
    }

    private void SearchInputFieldUpdated(InputField inputField) {
        Search(inputField.text);
    }

    private void Search(string searchString) {
        if (string.IsNullOrEmpty(searchString)) {
            for (var i = 0; i < _logViewHistory.Count; i++) {
                _logViewHistory[i].gameObject.SetActive(true);
            }

            return;
        }

        for (var i = 0; i < _logViewHistory.Count; i++) {
            var searchStringLower = searchString.ToLowerInvariant();
            var logStringLower = _logViewHistory[i].ConsoleLog.LogString.ToLowerInvariant();

            _logViewHistory[i].gameObject.SetActive(logStringLower.Contains(searchStringLower));
        }
    }

    private void CommandSubmit() {
        if (string.IsNullOrEmpty(_commandInputField.text)) {
            return;
        }

        Monolith.Console.ExecuteCommand(_commandInputField.text);
        _commandInputField.text = "";
        _commandInputField.ActivateInputField();
        _commandHistoryDelta = 0;
    }

    private void CommandSetPrevious(int direction) {
        var commandHistoryCount = Monolith.Console.ConsoleHistory.CommandHistoryCount;

        if (commandHistoryCount > 0) {
            // Calculate commandHistory Delta.
            // This is the distance from the present, into the history of commands.
            // Needs to be between 0, and commandHistoryCount.
            _commandHistoryDelta = Mathf.Clamp(_commandHistoryDelta + direction, 0, commandHistoryCount);

            // Calculate history index.
            // This is the index of the command we will be accessing.
            // Needs to be between 0, and commandHistoryCount - 1.
            var index = Mathf.Clamp(commandHistoryCount - 1 - (_commandHistoryDelta - 1), 0, commandHistoryCount - 1);

            if (_commandHistoryDelta == 0) {
                _commandInputField.text = "";
            }
            else {
                _commandInputField.text = Monolith.Console.ConsoleHistory.GetCommandHistoryWithIndex(index);
                _commandInputField.caretPosition = _commandInputField.text.Length;
            }
        }
    }

    private void CommandAutoComplete() {
        var input = _commandInputField.text.Trim();

        if (string.IsNullOrEmpty(input)) {
            return;
        }

        var commands = Monolith.Console.GetOrderedCommands().Where(command =>
                                                                       command.CommandName.StartsWith(
                                                                           input, StringComparison.CurrentCultureIgnoreCase))
                               .ToList();

        switch (commands.Count) {
            case 0: {
                return;
            }

            case 1: {
                _commandInputField.text = commands[0].CommandName;
                _commandInputField.MoveTextEnd(false);

                return;
            }

            default: {
                LogAvailableAutoCompleteCommands(commands);
                break;
            }
        }
    }

    private static void LogAvailableAutoCompleteCommands(List<ConsoleCommand> commands) {
        var stringBuilder = new StringBuilder();

        foreach (var command in commands) {
            stringBuilder.Append($"{command.CommandName}\t\t");
        }

        Monolith.Console.Log(stringBuilder.ToString(), LogType.Log, false);
    }
#endregion Private Methods

}
}