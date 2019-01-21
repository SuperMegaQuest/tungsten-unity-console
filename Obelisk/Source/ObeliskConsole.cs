using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HUDConsole {
public class ObeliskConsole : ConsoleViewAbstract {
	
	public override bool IsActive { get; protected set; }

	public override void ClearConsoleView() {
		base.ClearConsoleView();

		for (int i = 0, n = _logViewHistory.Count; i < n; i++) {
			PoolLog(_logViewHistory[i]);
		}

		_logViewHistory.Clear();

		ResizeLogLayout();
	}

	public static void OpenStackTraceForLog(ConsoleLog consoleLog) {
		_obeliskStackTrace.Open(consoleLog);
	}

	public static ObeliskColorSet ColorSet { get; private set; }

	private static ObeliskConsole _instance = null;

	[Header("Main")]
	[SerializeField] private RectTransform _container;
	[SerializeField] private ListenForDimensionsChange _containerListener;
	[SerializeField] private Transform _obeliskLogPoolContainer;
	[SerializeField] private Image _containerBackgroundImage;
	[SerializeField] private Outline _outline;
	[SerializeField] private Image _resizeHandleImage;

	[Header("Titlebar")]
	[SerializeField] private Image _titlebarBackgroundImage;
	[SerializeField] private Image _titlebarIconImage;
	[SerializeField] private Text _titlebarTitleText;
	[SerializeField] private Button _closeButton;
	[SerializeField] private Image _closeButtonBackgroundImage;
	[SerializeField] private Image _closeButtonIconImage;

	[Header("Filter")]
	[SerializeField] private ObeliskFilterDropdown _filterDropdown;

	[Header("Log")]
	[SerializeField] private ScrollRect _scrollRect;
	[SerializeField] private RectTransform _logLayout;
	[SerializeField] private Scrollbar _scrollbar;
	[SerializeField] private Image _scrollbarBackgroundImage;

	[Header("Input")]
	[SerializeField] private Image _inputContainerBackgroundImage;

	[Header("Command")]
	[SerializeField] private RectTransform _commandContainer;
	[SerializeField] private Image _commandSymbolBackgroundImage;
	[SerializeField] private Image _commandSymbolImage;
	[SerializeField] private InputField _commandInputField;
	[SerializeField] private Image _commandInputFieldImage;
	[SerializeField] private Text _commandInputFieldText;

	[Header("Search")]
	[SerializeField] private RectTransform _searchContainer;
	[SerializeField] private Image _searchSymbolBackgroundImage;
	[SerializeField] private Image _searchSymbolImage;
	[SerializeField] private InputField _searchInputField;
	[SerializeField] private Image _searchInputFieldImage;
	[SerializeField] private Text _searchInputFieldText;

	private List<ObeliskLog> _logViewHistory = new List<ObeliskLog>();

	[SerializeField] private int _logViewHistoryMax = 64;

	private Queue<ObeliskLog> _obeliskLogPool = new Queue<ObeliskLog>();

	[Header("Obelisk Prefabs")]
	[SerializeField] private ObeliskLog _obeliskLogPrefab;

	[SerializeField] private ObeliskStackTrace _obeliskStackTracePrefab;

	private static ObeliskStackTrace _obeliskStackTrace;

	[Header("Color Set")]
	[Tooltip("Color set objects.\nMust be in same order as DefaultViewColorSetName enum.")]
	[SerializeField] private ObeliskColorSet _colorSet;

	
#region Init
	protected override void Awake() {
		base.Awake();

		// Setup instance.
		if (_instance == null) {
			_instance = this;
		} else {
			Debug.LogError("[Console] Two instances of ObeliskConsole detected.");
			Destroy(gameObject);
		}

		ColorSet = Instantiate(_colorSet);

		SetupComponents();
		LogAwake();
		ApplyColorSet();
	}

	private void Start() {
		_obeliskStackTrace = Instantiate(_obeliskStackTracePrefab);
		_obeliskStackTrace.transform.SetParent(transform.parent, false);
	}

	private void Update() {
		StateUpdate();
		CommandUpdate();
	}
	
	private void OnContainerRectTransformDimensionsChange() {
		ResizeLogLayout();
		ResizeInputContainers();
	}
	
	private void SetupComponents() {
		// Main.
		_containerListener.SubscribeToDimensionsChange(OnContainerRectTransformDimensionsChange);

		// Titlebar.
		_closeButton.onClick.AddListener(delegate { CloseButtonHandler(_closeButton); });

		// Filter.
		_filterDropdown.SubscribeToFilterChanges(FilterUpdated);

		// Search.
		_searchInputField.onValueChanged.AddListener(delegate { SearchInputFieldUpdated(_searchInputField); });
	}
	

#endregion Init

#region State
	private void StateUpdate() {
		if (Input.GetKeyDown(KeyCode.BackQuote)) {
			ToggleState();
		}
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
#endregion State

#region Log
	private void LogAwake() {
		FillPool();
	}

	protected override void OnConsoleLogHistoryChanged() {
		base.OnConsoleLogHistoryChanged();

		ObeliskLog obeliskLog = _obeliskLogPool.Dequeue();
		obeliskLog.transform.SetParent(_logLayout, false);

		ConsoleLog consoleLog = Console.ConsoleHistory.LogGetLatest();
		obeliskLog.SetLog(ref consoleLog);

		AddLogViewHistory(obeliskLog);
	}

	private void AddLogViewHistory(ObeliskLog obeliskLog) {
		if (_logViewHistory.Count >= _logViewHistoryMax) {
			PoolLog(_logViewHistory[0]);
			_logViewHistory.RemoveAt(0);
		}

		_logViewHistory.Add(obeliskLog);

		ResizeLogLayout();

		_scrollRect.normalizedPosition = Vector2.zero;
	}

	private void FillPool() {
		for (var i = 0; i < _logViewHistoryMax + 1; i++) {
			ObeliskLog log = Instantiate(_obeliskLogPrefab);
			log.transform.SetParent(_obeliskLogPoolContainer, false);

			_obeliskLogPool.Enqueue(log);
		}
	}

	private void PoolLog(ObeliskLog obeliskLog) {
		obeliskLog.transform.SetParent(_obeliskLogPoolContainer, false);
		_obeliskLogPool.Enqueue(obeliskLog);
	}

	private void ResizeLogLayout() {
		Vector2 logLayoutSizeCache = _logLayout.sizeDelta;
		Vector3 logLayoutPosCache = _logLayout.localPosition;
		float minHeight = _container.sizeDelta.y - 40.0f;
		float newHeight = 0.0f;

		// Resize LogLayout so it can properly contain all the logs.
		for (int i = 0, n = _logViewHistory.Count; i < n; i++) {
			newHeight += _logViewHistory[i].RectTransform.sizeDelta.y;
		}

		_logLayout.sizeDelta = new Vector2(_logLayout.sizeDelta.x, Mathf.Clamp(newHeight, minHeight, 4096.0f));

		// Offset LogLayout relative to the size it just increased by.
		float newPosY = 0.0f;
		float sizeDifferenceY = _logLayout.sizeDelta.y - logLayoutSizeCache.y;
		newPosY = logLayoutPosCache.y + (sizeDifferenceY * 0.5f);
		_logLayout.localPosition = new Vector3(logLayoutPosCache.x, newPosY, 0.0f);
	}
#endregion Log

#region Filter
	private void FilterUpdated() {
		for (int i = 0, n = _logViewHistory.Count; i < n; i++) {
			switch (_logViewHistory[i].ConsoleLog.logType) {
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
#endregion Filter

#region Input
	private void ResizeInputContainers() {
		_commandContainer.sizeDelta = new Vector2(_container.sizeDelta.x * 0.68359375f, 20f);
		_searchContainer.sizeDelta = new Vector2(_container.sizeDelta.x * 0.3125f, 20f);

		_commandContainer.anchoredPosition = new Vector2(_commandContainer.sizeDelta.x * 0.5f, 0f);
		_searchContainer.anchoredPosition = new Vector2(-_searchContainer.sizeDelta.x * 0.5f, 0f);
	}
#endregion Input

#region Search
	private void SearchInputFieldUpdated(InputField inputField) {
		Search(inputField.text);
	}

	private void Search(string searchString) {
		if (string.IsNullOrEmpty(searchString)) {
			for (int i = 0, n = _logViewHistory.Count; i < n; i++) {
				_logViewHistory[i].gameObject.SetActive(true);
			}

			return;
		}

		for (int i = 0, n = _logViewHistory.Count; i < n; i++) {
			string searchStringLower = searchString.ToLowerInvariant();
			string logStringLower = _logViewHistory[i].ConsoleLog.logString.ToLowerInvariant();

			_logViewHistory[i].gameObject.SetActive(logStringLower.Contains(searchStringLower));
		}
	}
#endregion Search

#region Command
	private int _commandHistoryDelta = 0;

	private void CommandUpdate() {
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

	private void CommandSubmit() {
		if (string.IsNullOrEmpty(_commandInputField.text) == false) {
			Console.ExecuteCommand(_commandInputField.text);
			_commandInputField.text = "";
			_commandInputField.ActivateInputField();
			_commandHistoryDelta = 0;
		}
	}

	private void CommandSetPrevious(int direction) {
		int commandHistoryCount = Console.ConsoleHistory.CommandHistoryCount();
		int index = 0;

		if (commandHistoryCount > 0) {
			// Calculate commandHistory Delta.
			// This is the distance from the present, into the history of commands.
			// Needs to be between 0, and commandHistoryCount.
			_commandHistoryDelta = Mathf.Clamp(_commandHistoryDelta + direction, 0, commandHistoryCount);

			// Calculate history index.
			// This is the index of the command we will be accessing.
			// Needs to be between 0, and commandHistoryCount - 1.
			index = Mathf.Clamp((commandHistoryCount - 1) - (_commandHistoryDelta - 1), 0, commandHistoryCount - 1);

			if (_commandHistoryDelta == 0) {
				_commandInputField.text = "";
			} else {
				_commandInputField.text = Console.ConsoleHistory.CommandHistoryGet(index);
				_commandInputField.caretPosition = _commandInputField.text.Length;
			}
		}
	}

	private void CommandAutoComplete() {
		string input = _commandInputField.text.Trim();

		if (input == "") {
			return;
		}

		List<ConsoleCommand> commands = Console.GetOrderedCommands().Where(command => command.commandName.StartsWith(input, StringComparison.CurrentCultureIgnoreCase)).ToList();

		switch (commands.Count) {
			case 0: {
				return;
			}

			case 1: {
				_commandInputField.text = commands[0].commandName;
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
		StringBuilder stringBuilder = new StringBuilder();

		foreach (ConsoleCommand command in commands) {
			stringBuilder.Append(command.commandName + "\t\t");
		}

		Console.Log(stringBuilder.ToString(), LogType.Log, false);
	}
#endregion Command

#region ColorSet
	private void ApplyColorSet() {
		// Main.
		_containerBackgroundImage.color = ColorSet.BackgroundColor;
		_outline.effectColor = ColorSet.OutlineColor;
		_resizeHandleImage.color = ColorSet.ButtonColor;

		// Titlebar.
		_titlebarBackgroundImage.color = ColorSet.TitlebarBackgroundColor;
		_titlebarIconImage.color = ColorSet.IconColor;
		_titlebarTitleText.color = ColorSet.TitlebarTextColor;
		_closeButtonBackgroundImage.color = ColorSet.ButtonColor;
		_closeButtonIconImage.color = ColorSet.IconColor;

		// Scrollbar.
		_scrollbarBackgroundImage.color = ColorSet.ScrollbarBackgroundColor;

		var scrollbarColorBlock = new ColorBlock();
		scrollbarColorBlock.normalColor = ColorSet.ScrollbarSliderColor;
		scrollbarColorBlock.highlightedColor = ColorSet.ScrollbarSliderHighlightedColor;
		scrollbarColorBlock.pressedColor = ColorSet.ScrollbarSliderPressedColor;
		scrollbarColorBlock.colorMultiplier = 1.0f;
		_scrollbar.colors = scrollbarColorBlock;

		// Input
		_inputContainerBackgroundImage.color = ColorSet.InputContainerBackgroundColor;

		// Command.
		_commandSymbolBackgroundImage.color = ColorSet.IconBackgroundColor;
		_commandSymbolImage.color = ColorSet.IconColor;
		_commandInputFieldImage.color = ColorSet.ButtonColor;
		_commandInputFieldText.color = ColorSet.InputTextColor;

		// Search.
		_searchSymbolBackgroundImage.color = ColorSet.IconBackgroundColor;
		_searchSymbolImage.color = ColorSet.IconColor;
		_searchInputFieldImage.color = ColorSet.ButtonColor;
		_searchInputFieldText.color = ColorSet.InputTextColor;
	}
#endregion ColorSet
	
}
}