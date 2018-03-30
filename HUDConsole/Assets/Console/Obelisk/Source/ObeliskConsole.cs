using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HUDConsole {
	public class ObeliskConsole : ConsoleViewAbstract {
#region Public
		public override bool isActive { get; protected set; }

		public override void ClearConsoleView() {
			base.ClearConsoleView();

			for(int i = 0; i < m_logViewHistory.Count; i++) {
				PoolLog(m_logViewHistory[i]);
			}
			m_logViewHistory.Clear();

			ResizeLogLayout();
		}

		public static void OpenStackTraceForLog(ConsoleLog consoleLog) {
			m_obeliskStackTrace.Open(consoleLog);
		}

		public static ObeliskColorSet colorSet {
			get; private set;
		}
#endregion Public

#region Private
		private static ObeliskConsole m_instance = null;

		// Main.
		private RectTransform m_container;
		private ListenForDimensionsChange m_containerListener;
		private Transform m_obeliskLogPoolContainer;
		private Image m_containerBackgroundImage;
		private Outline m_outline;
		private Image m_resizeHandleImage;

		// Titlebar.
		private Image m_titlebarBackgroundImage;
		private Image m_titlebarIconImage;
		private Text m_titlebarTitleText;
		private Button m_closeButton;
		private Image m_closeButtonBackgroundImage;
		private Image m_closeButtonIconImage;

		// Filter.
		private ObeliskFilterDropdown m_filterDropdown;

		// Log.
		private RectTransform m_logLayout;
		private Scrollbar m_scrollbar;
		private Image m_scrollbarBackgroundImage;

		// Input.
		private Image m_inputContainerBackgroundImage;

		// Command.
		private RectTransform m_commandContainer;
		private Image m_commandSymbolBackgroundImage;
		private Image m_commandSymbolImage;
		private InputField m_commandInputField;
		private Image m_commandInputFieldImage;
		private Text m_commandInputFieldText;

		// Search.
		private RectTransform m_searchContainer;
		private Image m_searchSymbolBackgroundImage;
		private Image m_searchSymbolImage;
		private InputField m_searchInputField;
		private Image m_searchInputFieldImage;
		private Text m_searchInputFieldText;

		private List<ObeliskLog> m_logViewHistory = new List<ObeliskLog>();

		[SerializeField] private int m_logViewHistoryMax = 64;
		private Queue<ObeliskLog> m_obeliskLogPool = new Queue<ObeliskLog>();

		[Header("Obelisk Prefabs")]
		[SerializeField] private ObeliskLog m_obeliskLogPrefab;
		[SerializeField] private ObeliskStackTrace m_obeliskStackTracePrefab;
		private static ObeliskStackTrace m_obeliskStackTrace;

		[Header("Color Set")]
		[Tooltip("Color set objects.\nMust be in same order as DefaultViewColorSetName enum.")]
		[SerializeField] private ObeliskColorSet m_colorSet;

		protected override void Awake() {
			base.Awake();

			if(m_instance == null) {
				m_instance = this;
			}
			else {
				Debug.LogError("Two instances of ObeliskConsole detected.");
				Destroy(gameObject);
			}

			colorSet = Instantiate(m_colorSet);

			GetComponents();
			LogAwake();
			ApplyColorSet();
		}

		private void Start() {
			m_obeliskStackTrace = Instantiate(m_obeliskStackTracePrefab);
			m_obeliskStackTrace.transform.SetParent(transform.parent, false);
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
			m_container = transform.Find("Container").GetComponent<RectTransform>();
			m_containerListener = m_container.GetComponent<ListenForDimensionsChange>();
			m_obeliskLogPoolContainer = transform.Find("Container/LogPool");
			m_containerListener.SubscribeToDimensionsChange(OnContainerRectTransformDimensionsChange);
			m_containerBackgroundImage = m_container.GetComponent<Image>();
			m_outline = m_container.GetComponent<Outline>();
			m_resizeHandleImage = transform.Find("Container/Input/Search/ResizeHandle/Image").GetComponent<Image>();

			// Titlebar.
			m_titlebarBackgroundImage = transform.Find("Container/Titlebar").GetComponent<Image>();
			m_titlebarIconImage = transform.Find("Container/Titlebar/Icon").GetComponent<Image>();
			m_titlebarTitleText = transform.Find("Container/Titlebar/Title").GetComponent<Text>();
			m_closeButton = transform.Find("Container/Titlebar/Menu/Close").GetComponent<Button>();
			m_closeButton.onClick.AddListener(delegate { CloseButtonHandler(m_closeButton); });
			m_closeButtonBackgroundImage = transform.Find("Container/Titlebar/Menu/Close").GetComponent<Image>();
			m_closeButtonIconImage = transform.Find("Container/Titlebar/Menu/Close/Image").GetComponent<Image>();

			// Filter.
			m_filterDropdown = transform.Find("Container/Titlebar/Menu/Filter").GetComponent<ObeliskFilterDropdown>();
			m_filterDropdown.SubscribeToFilterChanges(FilterUpdated);

			// Log.
			m_logLayout = transform.Find("Container/Log/LogLayout").GetComponent<RectTransform>();
			m_scrollbar = transform.Find("Container/Log/Scrollbar").GetComponent<Scrollbar>();
			m_scrollbarBackgroundImage = transform.Find("Container/Log/Scrollbar").GetComponent<Image>();

			// Input container.
			m_inputContainerBackgroundImage = transform.Find("Container/Input").GetComponent<Image>();
			m_commandContainer = transform.Find("Container/Input/Command").GetComponent<RectTransform>();
			m_searchContainer = transform.Find("Container/Input/Search").GetComponent<RectTransform>();

			// Command.
			m_commandSymbolBackgroundImage = transform.Find("Container/Input/Command/Symbol").GetComponent<Image>();
			m_commandSymbolImage = transform.Find("Container/Input/Command/Symbol/Image").GetComponent<Image>();
			m_commandInputField = transform.Find("Container/Input/Command/InputField").GetComponent<InputField>();
			m_commandInputFieldImage = transform.Find("Container/Input/Command/InputField").GetComponent<Image>();
			m_commandInputFieldText = transform.Find("Container/Input/Command/InputField/Text").GetComponent<Text>();

			// Search.
			m_searchSymbolBackgroundImage = transform.Find("Container/Input/Search/Symbol").GetComponent<Image>();
			m_searchSymbolImage = transform.Find("Container/Input/Search/Symbol/Image").GetComponent<Image>();
			m_searchInputField = transform.Find("Container/Input/Search/InputField").GetComponent<InputField>();
			m_searchInputField.onValueChanged.AddListener(delegate { SearchInputFieldUpdated(m_searchInputField); });
			m_searchInputFieldImage = transform.Find("Container/Input/Search/InputField").GetComponent<Image>();
			m_searchInputFieldText = transform.Find("Container/Input/Search/InputField/Text").GetComponent<Text>();
		}

	#region State
		private void StateUpdate() {
			if(Input.GetKeyDown(KeyCode.BackQuote)) {
				ToggleState();
			}
		}

		private void ToggleState() {
			SetEnabled(!m_container.gameObject.activeSelf);
		}

		private void SetEnabled(bool enable) {
			m_container.gameObject.SetActive(enable);

			if(enable) {
				m_commandInputField.ActivateInputField();
			}

			isActive = enable;
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

			ObeliskLog obeliskLog = m_obeliskLogPool.Dequeue();
			obeliskLog.transform.SetParent(m_logLayout, false);
			
			ConsoleLog consoleLog = Console.consoleHistory.LogGetLatest();
			obeliskLog.SetLog(ref consoleLog);

			AddLogViewHistory(obeliskLog);
		}

		private void AddLogViewHistory(ObeliskLog obeliskLog) {
			if(m_logViewHistory.Count >= m_logViewHistoryMax) {
				PoolLog(m_logViewHistory[0]);
				m_logViewHistory.RemoveAt(0);
			}

			m_logViewHistory.Add(obeliskLog);

			ResizeLogLayout();
		}

		private void FillPool() {
			for(int i = 0; i < m_logViewHistoryMax + 1; i++) {
				ObeliskLog log = Instantiate(m_obeliskLogPrefab);
				log.transform.SetParent(m_obeliskLogPoolContainer, false);

				m_obeliskLogPool.Enqueue(log);
			}
		}

		private void PoolLog(ObeliskLog obeliskLog) {
			obeliskLog.transform.SetParent(m_obeliskLogPoolContainer, false);
			m_obeliskLogPool.Enqueue(obeliskLog);
		}

		private void ResizeLogLayout() {
			Vector2 logLayoutSizeCache = m_logLayout.sizeDelta;
			Vector3 logLayoutPosCache = m_logLayout.localPosition;
			float minHeight = m_container.sizeDelta.y - 40.0f;
			float newHeight = 0.0f;

			// Resize LogLayout so it can properly contain all the logs.
			for(var i = 0; i < m_logViewHistory.Count; i++) {
				newHeight += m_logViewHistory[i].rectTransform.sizeDelta.y;
			}
			m_logLayout.sizeDelta = new Vector2(m_logLayout.sizeDelta.x, Mathf.Clamp(newHeight, minHeight, 4096.0f));

			// Offset LogLayout relative to the size it just increased by.
			float newPosY = 0.0f;
			float sizeDifferenceY = m_logLayout.sizeDelta.y - logLayoutSizeCache.y;
			newPosY = logLayoutPosCache.y + (sizeDifferenceY * 0.5f);
			m_logLayout.localPosition = new Vector3(logLayoutPosCache.x, newPosY, 0.0f);
		}
	#endregion Log

	#region Filter
		private void FilterUpdated() {
			for(var i = 0; i < m_logViewHistory.Count; i++) {
				switch(m_logViewHistory[i].consoleLog.logType) {
					case LogType.Error: {
						m_logViewHistory[i].gameObject.SetActive(!m_filterDropdown.GetFilterSetting(LogType.Error));
						break;
					}

					case LogType.Assert: {
						m_logViewHistory[i].gameObject.SetActive(!m_filterDropdown.GetFilterSetting(LogType.Assert));
						break;
					}

					case LogType.Warning: {
						m_logViewHistory[i].gameObject.SetActive(!m_filterDropdown.GetFilterSetting(LogType.Warning));
						break;
					}

					case LogType.Log: {
						m_logViewHistory[i].gameObject.SetActive(!m_filterDropdown.GetFilterSetting(LogType.Log));
						break;
					}

					case LogType.Exception: {
						m_logViewHistory[i].gameObject.SetActive(!m_filterDropdown.GetFilterSetting(LogType.Exception));
						break;
					}
				}
			}
		}
	#endregion Filter

	#region Input
		private void ResizeInputContainers() {
			m_commandContainer.sizeDelta = new Vector2(m_container.sizeDelta.x * 0.68359375f, 20f);
			m_searchContainer.sizeDelta = new Vector2(m_container.sizeDelta.x * 0.3125f, 20f);

			m_commandContainer.anchoredPosition = new Vector2(m_commandContainer.sizeDelta.x * 0.5f, 0f);
			m_searchContainer.anchoredPosition = new Vector2(-m_searchContainer.sizeDelta.x * 0.5f, 0f);
		}
	#endregion Input

	#region Search
		private void SearchInputFieldUpdated(InputField inputField) {
			Search(inputField.text);
		}

		private void Search(string searchString) {
			if(string.IsNullOrEmpty(searchString)) {
				for(var i = 0; i < m_logViewHistory.Count; i++) {
					m_logViewHistory[i].gameObject.SetActive(true);
				}

				return;
			}

			for(var i = 0; i < m_logViewHistory.Count; i++) {
				string searchStringLower = searchString.ToLowerInvariant();
				string logStringLower = m_logViewHistory[i].consoleLog.logString.ToLowerInvariant();

				m_logViewHistory[i].gameObject.SetActive(logStringLower.Contains(searchStringLower));
			}
		}
	#endregion Search

	#region Command
		private int m_commandHistoryDelta = 0;

		private void CommandUpdate() {
			if(m_container.gameObject.activeSelf
			&& EventSystem.current.currentSelectedGameObject == m_commandInputField.gameObject) {
				// Submit command.
				if(Input.GetKeyDown(KeyCode.Return)
				|| Input.GetKeyDown(KeyCode.KeypadEnter)) {
					CommandSubmit();
				}

				// Previous Command +
				if(Input.GetKeyDown(KeyCode.UpArrow)) {
					CommandSetPrevious(1);
				}

				// Previous Command -
				if(Input.GetKeyDown(KeyCode.DownArrow)) {
					CommandSetPrevious(-1);
				}
			}
		}

		private void CommandSubmit() {
			if(string.IsNullOrEmpty(m_commandInputField.text) == false) {
				Console.ExecuteCommand(m_commandInputField.text);
				m_commandInputField.text = "";
				m_commandInputField.ActivateInputField();
				m_commandHistoryDelta = 0;
			}
		}

		private void CommandSetPrevious(int direction) {
			int commandHistoryCount = Console.consoleHistory.CommandHistoryCount();
			int index = 0;

			if(commandHistoryCount > 0) {
				// Calculate commandHistory Delta.
				// This is the distance from the present, into the history of commands.
				// Needs to be between 0, and commandHistoryCount.
				m_commandHistoryDelta = Mathf.Clamp(m_commandHistoryDelta + direction, 0, commandHistoryCount);

				// Calculate history index.
				// This is the index of the command we will be accessing.
				// Needs to be between 0, and commandHistoryCount - 1.
				index = Mathf.Clamp((commandHistoryCount - 1) - (m_commandHistoryDelta - 1), 0, commandHistoryCount - 1);

				if(m_commandHistoryDelta == 0) {
					m_commandInputField.text = "";
				}
				else {
					m_commandInputField.text = Console.consoleHistory.CommandHistoryGet(index);
					m_commandInputField.caretPosition = m_commandInputField.text.Length;
				}
			}
		}
	#endregion Command

	#region ColorSet
		private void ApplyColorSet() {
			// Main.
			m_containerBackgroundImage.color = colorSet.backgroundColor;
			m_outline.effectColor = colorSet.outlineColor;
			m_resizeHandleImage.color = colorSet.buttonColor;

			// Titlebar.
			m_titlebarBackgroundImage.color = colorSet.titlebarBackgroundColor;
			m_titlebarIconImage.color = colorSet.iconColor;
			m_titlebarTitleText.color = colorSet.titlebarTextColor;
			m_closeButtonBackgroundImage.color = colorSet.buttonColor;
			m_closeButtonIconImage.color = colorSet.iconColor;

			// Scrollbar.
			m_scrollbarBackgroundImage.color = colorSet.scrollbarBackgroundColor;

			ColorBlock scrollbarColorBlock = new ColorBlock();
			scrollbarColorBlock.normalColor = colorSet.scrollbarSliderColor;
			scrollbarColorBlock.highlightedColor = colorSet.scrollbarSliderHighlightedColor;
			scrollbarColorBlock.pressedColor = colorSet.scrollbarSliderPressedColor;
			scrollbarColorBlock.colorMultiplier = 1.0f;
			m_scrollbar.colors = scrollbarColorBlock;

			// Input
			m_inputContainerBackgroundImage.color = colorSet.inputContainerBackgroundColor;

			// Command.
			m_commandSymbolBackgroundImage.color = colorSet.iconBackgroundColor;
			m_commandSymbolImage.color = colorSet.iconColor;
			m_commandInputFieldImage.color = colorSet.buttonColor;
			m_commandInputFieldText.color = colorSet.inputTextColor;

			// Search.
			m_searchSymbolBackgroundImage.color = colorSet.iconBackgroundColor;
			m_searchSymbolImage.color = colorSet.iconColor;
			m_searchInputFieldImage.color = colorSet.buttonColor;
			m_searchInputFieldText.color = colorSet.inputTextColor;
		}
	#endregion ColorSet
#endregion Private
	}
}