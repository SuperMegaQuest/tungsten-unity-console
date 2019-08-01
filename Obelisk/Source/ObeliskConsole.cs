using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gruel.Obelisk {
	public class ObeliskConsole : ConsoleView {

#region Properties
		public override bool IsActive { get; protected set; }
#endregion Properties

#region Fields
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
		
		[Header("Settings")]
		[SerializeField] private int _logViewHistoryMax = 64;
		
		[Header("Obelisk Prefabs")]
		[SerializeField] private ObeliskLog _obeliskLogPrefab;
		[SerializeField] private ObeliskStackTrace _obeliskStackTracePrefab;
		
		[Header("Obelisk Color Set")]
		[Tooltip("Color set objects.\nMust be in same order as DefaultViewColorSetName enum.")]
		[SerializeField] private ObeliskColorSet _colorSetAsset;
		
//		private static ObeliskConsole _instance;

		private List<ObeliskLog> _logViewHistory = new List<ObeliskLog>();
		private Queue<ObeliskLog> _obeliskLogPool = new Queue<ObeliskLog>();
		private ObeliskStackTrace _obeliskStackTrace;
		private ObeliskColorSet _instantiatedColorSet;
		
		private int _commandHistoryDelta;
#endregion Fields

#region Public Methods
		public override void ClearConsoleView() {
			base.ClearConsoleView();

			for (int i = 0, n = _logViewHistory.Count; i < n; i++) {
				PoolLog(_logViewHistory[i]);
			}

			_logViewHistory.Clear();

			ResizeLogLayout();
		}

		public void OpenStackTraceForLog(ConsoleLog consoleLog) {
			_obeliskStackTrace.Open(consoleLog);
		}
#endregion Public Methods

#region Private Methods
		protected override void Awake() {
			base.Awake();

			_instantiatedColorSet = Instantiate(_colorSetAsset);

			// Main.
			_containerListener.AddDimensionsChangedListener(OnContainerRectTransformDimensionsChange);

			// Titlebar.
			_closeButton.onClick.AddListener(delegate { CloseButtonHandler(_closeButton); });

			// Filter.
			_filterDropdown.AddFilterChangedListener(FilterUpdated);
			_filterDropdown.ColorSet = _instantiatedColorSet;

			// Search.
			_searchInputField.onValueChanged.AddListener(delegate { SearchInputFieldUpdated(_searchInputField); });
			
			// Fill log pool.
			FillPool();
			
			// Apply color set to main window.
			ApplyColorSet();
			
			// StackTrace window.
			_obeliskStackTrace = Instantiate(_obeliskStackTracePrefab, transform.parent, false);
			_obeliskStackTrace.ColorSet = _instantiatedColorSet;
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

			var scrollbarColorBlock = new ColorBlock();
			scrollbarColorBlock.normalColor = _instantiatedColorSet.ScrollbarSliderColor;
			scrollbarColorBlock.highlightedColor = _instantiatedColorSet.ScrollbarSliderHighlightedColor;
			scrollbarColorBlock.pressedColor = _instantiatedColorSet.ScrollbarSliderPressedColor;
			scrollbarColorBlock.colorMultiplier = 1.0f;
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

			ObeliskLog obeliskLog = _obeliskLogPool.Dequeue();
			obeliskLog.transform.SetParent(_logLayout, false);

			ConsoleLog consoleLog = Console.ConsoleHistory.LatestLog;
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
				var log = Instantiate(_obeliskLogPrefab, _obeliskLogPoolContainer, false);
				log.Init(this, _instantiatedColorSet);
				_obeliskLogPool.Enqueue(log);
			}
		}

		private void PoolLog(ObeliskLog obeliskLog) {
			obeliskLog.transform.SetParent(_obeliskLogPoolContainer, false);
			_obeliskLogPool.Enqueue(obeliskLog);
		}

		private void ResizeLogLayout() {
			var logLayoutSizeCache = _logLayout.sizeDelta;
			var logLayoutPosCache = _logLayout.localPosition;
			var minHeight = _container.sizeDelta.y - 40.0f;
			var newHeight = 0.0f;

			// Resize LogLayout so it can properly contain all the logs.
			for (int i = 0, n = _logViewHistory.Count; i < n; i++) {
				newHeight += _logViewHistory[i].Height;
			}

			_logLayout.sizeDelta = new Vector2(_logLayout.sizeDelta.x, Mathf.Clamp(newHeight, minHeight, 4096.0f));

			// Offset LogLayout relative to the size it just increased by.
			float newPosY;
			float sizeDifferenceY = _logLayout.sizeDelta.y - logLayoutSizeCache.y;
			newPosY = logLayoutPosCache.y + (sizeDifferenceY * 0.5f);
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
			_commandContainer.sizeDelta = new Vector2(_container.sizeDelta.x * 0.68359375f, 20f);
			_searchContainer.sizeDelta = new Vector2(_container.sizeDelta.x * 0.3125f, 20f);

			_commandContainer.anchoredPosition = new Vector2(_commandContainer.sizeDelta.x * 0.5f, 0f);
			_searchContainer.anchoredPosition = new Vector2(-_searchContainer.sizeDelta.x * 0.5f, 0f);
		}
		
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
				string logStringLower = _logViewHistory[i].ConsoleLog.LogString.ToLowerInvariant();

				_logViewHistory[i].gameObject.SetActive(logStringLower.Contains(searchStringLower));
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
			int commandHistoryCount = Console.ConsoleHistory.CommandHistoryCount;
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
					_commandInputField.text = Console.ConsoleHistory.GetCommandHistoryWithIndex(index);
					_commandInputField.caretPosition = _commandInputField.text.Length;
				}
			}
		}

		private void CommandAutoComplete() {
			string input = _commandInputField.text.Trim();

			if (input == "") {
				return;
			}

			List<ConsoleCommand> commands = Console.GetOrderedCommands().Where(command => command.CommandName.StartsWith(input, StringComparison.CurrentCultureIgnoreCase)).ToList();

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
			StringBuilder stringBuilder = new StringBuilder();

			foreach (ConsoleCommand command in commands) {
				stringBuilder.Append(command.CommandName + "\t\t");
			}

			Console.Log(stringBuilder.ToString(), LogType.Log, false);
		}
#endregion Private Methods
		
	}
}