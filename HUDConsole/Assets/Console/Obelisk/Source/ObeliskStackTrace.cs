using UnityEngine;
using UnityEngine.UI;

namespace HUDConsole {
	public class ObeliskStackTrace : MonoBehaviour {
#region Public
		public void Open(ConsoleLog consoleLog) {
			m_stackTraceText.text = consoleLog.logString + "\n\n" + consoleLog.stackTraceString;
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
			m_containerBackgroundImage.color = ObeliskConsole.colorSet.backgroundColor;
			m_outline.effectColor = ObeliskConsole.colorSet.outlineColor;
			m_resizeHandleBackgroundImage.color = ObeliskConsole.colorSet.inputContainerBackgroundColor;
			m_resizeHandleImage.color = ObeliskConsole.colorSet.buttonColor;

			// Titlebar.
			m_titlebarBackgroundImage.color = ObeliskConsole.colorSet.titlebarBackgroundColor;
			m_titlebarIconImage.color = ObeliskConsole.colorSet.iconColor;
			m_titlebarTitleText.color = ObeliskConsole.colorSet.titlebarTextColor;
			m_closeButtonBackgroundImage.color = ObeliskConsole.colorSet.buttonColor;
			m_closeButtonIconImage.color = ObeliskConsole.colorSet.iconColor;

			// Scrollbar.
			m_scrollbarBackgroundImage.color = ObeliskConsole.colorSet.scrollbarBackgroundColor;

			ColorBlock scrollbarColorBlock = new ColorBlock();
			scrollbarColorBlock.normalColor = ObeliskConsole.colorSet.scrollbarSliderColor;
			scrollbarColorBlock.highlightedColor = ObeliskConsole.colorSet.scrollbarSliderHighlightedColor;
			scrollbarColorBlock.pressedColor = ObeliskConsole.colorSet.scrollbarSliderPressedColor;
			scrollbarColorBlock.colorMultiplier = 1.0f;
			m_scrollbar.colors = scrollbarColorBlock;

			// Content.
			m_stackTraceText.color = ObeliskConsole.colorSet.stackTraceTextColor;
		}
	#endregion ColorSet
#endregion Private
	}
}