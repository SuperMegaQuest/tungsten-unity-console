using UnityEngine;
using UnityEngine.UI;

namespace HUDConsole {
	public class ObeliskStackTrace : MonoBehaviour {
#region Public
		public void Open(ConsoleLog consoleLog) {
			_stackTraceText.text = consoleLog.logString + "\n\n" + consoleLog.stackTrace;
			SetEnabled(true);
			_scrollRect.normalizedPosition = new Vector2(0, 1);
		}
#endregion Public

#region Private
		// Main.
		private GameObject _container;
		private Image _containerBackgroundImage;
		private Outline _outline;
		private Image _resizeHandleBackgroundImage;
		private Image _resizeHandleImage;

		// Titlebar.
		private Image _titlebarBackgroundImage;
		private Image _titlebarIconImage;
		private Text _titlebarTitleText;
		private Button _closeButton;
		private Image _closeButtonBackgroundImage;
		private Image _closeButtonIconImage;

		// Scrollbar.
		private ScrollRect _scrollRect;
		private Scrollbar _scrollbar;
		private Image _scrollbarBackgroundImage;

		// Content.
		private Text _stackTraceText;

		private void Awake() {
			GetComponents();
			ApplyColorSet();
		}

		private void GetComponents() {
			// Main.
			_container = transform.Find("Container").gameObject;
			_containerBackgroundImage = _container.GetComponent<Image>();
			_outline = _container.GetComponent<Outline>();
			_resizeHandleBackgroundImage = transform.Find("Container/Log/ResizeHandle").GetComponent<Image>();
			_resizeHandleImage = transform.Find("Container/Log/ResizeHandle/Image").GetComponent<Image>();

			// Titlebar.
			_titlebarBackgroundImage = transform.Find("Container/Titlebar").GetComponent<Image>();
			_titlebarIconImage = transform.Find("Container/Titlebar/Icon").GetComponent<Image>();
			_titlebarTitleText = transform.Find("Container/Titlebar/Title").GetComponent<Text>();

			_closeButton = transform.Find("Container/Titlebar/Close").GetComponent<Button>();
			_closeButton.onClick.AddListener(delegate { CloseButtonHandler(_closeButton); });
			_closeButtonBackgroundImage = transform.Find("Container/Titlebar/Close").GetComponent<Image>();
			_closeButtonIconImage = transform.Find("Container/Titlebar/Close/Image").GetComponent<Image>();

			// Scrollbar.
			_scrollRect = transform.Find("Container/Log").GetComponent<ScrollRect>();
			_scrollbar = transform.Find("Container/Log/Scrollbar").GetComponent<Scrollbar>();
			_scrollbarBackgroundImage = transform.Find("Container/Log/Scrollbar").GetComponent<Image>();

			// Content.
			_stackTraceText = transform.Find("Container/Log/StackTrace").GetComponent<Text>();
		}

#region State
		private void SetEnabled(bool enable) {
			_container.SetActive(enable);
		}

		private void CloseButtonHandler(Button target) {
			SetEnabled(false);
		}
#endregion State

#region ColorSet
		private void ApplyColorSet() {
			// Main.
			_containerBackgroundImage.color = ObeliskConsole.ColorSet.BackgroundColor;
			_outline.effectColor = ObeliskConsole.ColorSet.OutlineColor;
			_resizeHandleBackgroundImage.color = ObeliskConsole.ColorSet.InputContainerBackgroundColor;
			_resizeHandleImage.color = ObeliskConsole.ColorSet.ButtonColor;

			// Titlebar.
			_titlebarBackgroundImage.color = ObeliskConsole.ColorSet.TitlebarBackgroundColor;
			_titlebarIconImage.color = ObeliskConsole.ColorSet.IconColor;
			_titlebarTitleText.color = ObeliskConsole.ColorSet.TitlebarTextColor;
			_closeButtonBackgroundImage.color = ObeliskConsole.ColorSet.ButtonColor;
			_closeButtonIconImage.color = ObeliskConsole.ColorSet.IconColor;

			// Scrollbar.
			_scrollbarBackgroundImage.color = ObeliskConsole.ColorSet.ScrollbarBackgroundColor;

			var scrollbarColorBlock = new ColorBlock();
			scrollbarColorBlock.normalColor = ObeliskConsole.ColorSet.ScrollbarSliderColor;
			scrollbarColorBlock.highlightedColor = ObeliskConsole.ColorSet.ScrollbarSliderHighlightedColor;
			scrollbarColorBlock.pressedColor = ObeliskConsole.ColorSet.ScrollbarSliderPressedColor;
			scrollbarColorBlock.colorMultiplier = 1.0f;
			_scrollbar.colors = scrollbarColorBlock;

			// Content.
			_stackTraceText.color = ObeliskConsole.ColorSet.StackTraceTextColor;
		}
#endregion ColorSet
#endregion Private
	}
}