using UnityEngine;
using UnityEngine.UI;

namespace HUDConsole {
	public class ObeliskStackTrace : MonoBehaviour {
#region Public
		public void Open(ConsoleLog consoleLog) {
			m_stackTraceText.text = consoleLog.logString + "\n\n" + consoleLog.stackTrace;
			SetEnabled(true);
			m_scrollRect.normalizedPosition = new Vector2(0, 1);
		}
#endregion Public

#region Private
		// Main.
		private GameObject m_container;
		private Image m_containerBackgroundImage;
		private Outline m_outline;
		private Image m_resizeHandleBackgroundImage;
		private Image m_resizeHandleImage;

		// Titlebar.
		private Image m_titlebarBackgroundImage;
		private Image m_titlebarIconImage;
		private Text m_titlebarTitleText;
		private Button m_closeButton;
		private Image m_closeButtonBackgroundImage;
		private Image m_closeButtonIconImage;

		// Scrollbar.
		private ScrollRect m_scrollRect;
		private Scrollbar m_scrollbar;
		private Image m_scrollbarBackgroundImage;

		// Content.
		private Text m_stackTraceText;

		private void Awake() {
			GetComponents();
			ApplyColorSet();
		}

		private void GetComponents() {
			// Main.
			m_container = transform.Find("Container").gameObject;
			m_containerBackgroundImage = m_container.GetComponent<Image>();
			m_outline = m_container.GetComponent<Outline>();
			m_resizeHandleBackgroundImage = transform.Find("Container/Log/ResizeHandle").GetComponent<Image>();
			m_resizeHandleImage = transform.Find("Container/Log/ResizeHandle/Image").GetComponent<Image>();

			// Titlebar.
			m_titlebarBackgroundImage = transform.Find("Container/Titlebar").GetComponent<Image>();
			m_titlebarIconImage = transform.Find("Container/Titlebar/Icon").GetComponent<Image>();
			m_titlebarTitleText = transform.Find("Container/Titlebar/Title").GetComponent<Text>();

			m_closeButton = transform.Find("Container/Titlebar/Close").GetComponent<Button>();
			m_closeButton.onClick.AddListener(delegate { CloseButtonHandler(m_closeButton); });
			m_closeButtonBackgroundImage = transform.Find("Container/Titlebar/Close").GetComponent<Image>();
			m_closeButtonIconImage = transform.Find("Container/Titlebar/Close/Image").GetComponent<Image>();

			// Scrollbar.
			m_scrollRect = transform.Find("Container/Log").GetComponent<ScrollRect>();
			m_scrollbar = transform.Find("Container/Log/Scrollbar").GetComponent<Scrollbar>();
			m_scrollbarBackgroundImage = transform.Find("Container/Log/Scrollbar").GetComponent<Image>();

			// Content.
			m_stackTraceText = transform.Find("Container/Log/StackTrace").GetComponent<Text>();
		}

	#region State
		private void SetEnabled(bool enable) {
			m_container.SetActive(enable);
		}

		private void CloseButtonHandler(Button target) {
			SetEnabled(false);
		}
	#endregion State

	#region ColorSet
		private void ApplyColorSet() {
			// Main.
			m_containerBackgroundImage.color = ObeliskConsole.ColorSet.backgroundColor;
			m_outline.effectColor = ObeliskConsole.ColorSet.outlineColor;
			m_resizeHandleBackgroundImage.color = ObeliskConsole.ColorSet.inputContainerBackgroundColor;
			m_resizeHandleImage.color = ObeliskConsole.ColorSet.buttonColor;

			// Titlebar.
			m_titlebarBackgroundImage.color = ObeliskConsole.ColorSet.titlebarBackgroundColor;
			m_titlebarIconImage.color = ObeliskConsole.ColorSet.iconColor;
			m_titlebarTitleText.color = ObeliskConsole.ColorSet.titlebarTextColor;
			m_closeButtonBackgroundImage.color = ObeliskConsole.ColorSet.buttonColor;
			m_closeButtonIconImage.color = ObeliskConsole.ColorSet.iconColor;

			// Scrollbar.
			m_scrollbarBackgroundImage.color = ObeliskConsole.ColorSet.scrollbarBackgroundColor;

			var scrollbarColorBlock = new ColorBlock();
			scrollbarColorBlock.normalColor = ObeliskConsole.ColorSet.scrollbarSliderColor;
			scrollbarColorBlock.highlightedColor = ObeliskConsole.ColorSet.scrollbarSliderHighlightedColor;
			scrollbarColorBlock.pressedColor = ObeliskConsole.ColorSet.scrollbarSliderPressedColor;
			scrollbarColorBlock.colorMultiplier = 1.0f;
			m_scrollbar.colors = scrollbarColorBlock;

			// Content.
			m_stackTraceText.color = ObeliskConsole.ColorSet.stackTraceTextColor;
		}
	#endregion ColorSet
#endregion Private
	}
}