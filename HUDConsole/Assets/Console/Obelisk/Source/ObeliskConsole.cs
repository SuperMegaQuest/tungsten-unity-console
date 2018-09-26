using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HUDConsole {
	public class ObeliskConsole : ConsoleViewAbstract {
#region Public
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
#endregion Public

#region Private
		private static ObeliskConsole _instance = null;

		// Main.
		private RectTransform _container;
		private ListenForDimensionsChange _containerListener;
		private Transform _obeliskLogPoolContainer;
		private Image _containerBackgroundImage;
		private Outline _outline;
		private Image _resizeHandleImage;

		// Titlebar.
		private Image _titlebarBackgroundImage;
		private Image _titlebarIconImage;
		private Text _titlebarTitleText;
		private Button _closeButton;
		private Image _closeButtonBackgroundImage;
		private Image _closeButtonIconImage;

		// Filter.
		private ObeliskFilterDropdown _filterDropdown;

		// Log.
		private ScrollRect _scrollRect;
		private RectTransform _logLayout;
		private Scrollbar _scrollbar;
		private Image _scrollbarBackgroundImage;

		// Input.
		private Image _inputContainerBackgroundImage;

		// Command.
		private RectTransform _commandContainer;
		private Image _commandSymbolBackgroundImage;
		private Image _commandSymbolImage;
		private InputField _commandInputField;
		private Image _commandInputFieldImage;
		private Text _commandInputFieldText;

		// Search.
		private RectTransform _searchContainer;
		private Image _searchSymbolBackgroundImage;
		private Image _searchSymbolImage;
		private InputField _searchInputField;
		private Image _searchInputFieldImage;
		private Text _searchInputFieldText;

		private List<ObeliskLog> _logViewHistory = new List<ObeliskLog>();

		[SerializeField]
		private int _logViewHistoryMax = 64;

		private Queue<ObeliskLog> _obeliskLogPool = new Queue<ObeliskLog>();

		[Header("Obelisk Prefabs")]
		[SerializeField] private ObeliskLog _obeliskLogPrefab;

		[SerializeField] private ObeliskStackTrace _obeliskStackTracePrefab;

		private static ObeliskStackTrace _obeliskStackTrace;

		[Header("Color Set")]
		[Tooltip("Color set objects.\nMust be in same order as DefaultViewColorSetName enum.")]
		[SerializeField] private ObeliskColorSet _colorSet;

		protected override void Awake() {
			base.Awake();

			if (_instance == null) {
				_instance = this;
			} else {
				Debug.LogError("[Console] Two instances of ObeliskConsole detected.");
				Destroy(gameObject);
			}

			ColorSet = Instantiate(_colorSet);

			GetComponents();
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

		private void GetComponents() {
			// Main.
			_container = transform.Find("Container").GetComponent<RectTransform>();
			_containerListener = _container.GetComponent<ListenForDimensionsChange>();
			_obeliskLogPoolContainer = transform.Find("Container/LogPool");
			_containerListener.SubscribeToDimensionsChange(OnContainerRectTransformDimensionsChange);
			_containerBackgroundImage = _container.GetComponent<Image>();
			_outline = _container.GetComponent<Outline>();
			_resizeHandleImage = transform.Find("Container/Input/Search/ResizeHandle/Image").GetComponent<Image>();

			// Titlebar.
			_titlebarBackgroundImage = transform.Find("Container/Titlebar").GetComponent<Image>();
			_titlebarIconImage = transform.Find("Container/Titlebar/Icon").GetComponent<Image>();
			_titlebarTitleText = transform.Find("Container/Titlebar/Title").GetComponent<Text>();
			_closeButton = transform.Find("Container/Titlebar/Menu/Close").GetComponent<Button>();
			_closeButton.onClick.AddListener(delegate { CloseButtonHandler(_closeButton); });
			_closeButtonBackgroundImage = transform.Find("Container/Titlebar/Menu/Close").GetComponent<Image>();
			_closeButtonIconImage = transform.Find("Container/Titlebar/Menu/Close/Image").GetComponent<Image>();

			// Filter.
			_filterDropdown = transform.Find("Container/Titlebar/Menu/Filter").GetComponent<ObeliskFilterDropdown>();
			_filterDropdown.SubscribeToFilterChanges(FilterUpdated);

			// Log.
			_scrollRect = transform.Find("Container/Log").GetComponent<ScrollRect>();
			_logLayout = transform.Find("Container/Log/LogLayout").GetComponent<RectTransform>();
			_scrollbar = transform.Find("Container/Log/Scrollbar").GetComponent<Scrollbar>();
			_scrollbarBackgroundImage = transform.Find("Container/Log/Scrollbar").GetComponent<Image>();

			// Input container.
			_inputContainerBackgroundImage = transform.Find("Container/Input").GetComponent<Image>();
			_commandContainer = transform.Find("Container/Input/Command").GetComponent<RectTransform>();
			_searchContainer = transform.Find("Container/Input/Search").GetComponent<RectTransform>();

			// Command.
			_commandSymbolBackgroundImage = transform.Find("Container/Input/Command/Symbol").GetComponent<Image>();
			_commandSymbolImage = transform.Find("Container/Input/Command/Symbol/Image").GetComponent<Image>();
			_commandInputField = transform.Find("Container/Input/Command/InputField").GetComponent<InputField>();
			_commandInputFieldImage = transform.Find("Container/Input/Command/InputField").GetComponent<Image>();
			_commandInputFieldText = transform.Find("Container/Input/Command/InputField/Text").GetComponent<Text>();

			// Search.
			_searchSymbolBackgroundImage = transform.Find("Container/Input/Search/Symbol").GetComponent<Image>();
			_searchSymbolImage = transform.Find("Container/Input/Search/Symbol/Image").GetComponent<Image>();
			_searchInputField = transform.Find("Container/Input/Search/InputField").GetComponent<InputField>();
			_searchInputField.onValueChanged.AddListener(delegate { SearchInputFieldUpdated(_searchInputField); });
			_searchInputFieldImage = transform.Find("Container/Input/Search/InputField").GetComponent<Image>();
			_searchInputFieldText = transform.Find("Container/Input/Search/InputField/Text").GetComponent<Text>();
		}

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
			for (var i = 0; i < _logViewHistory.Count; i++) {
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
#endregion Private
	}
}