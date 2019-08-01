using UnityEngine;
using UnityEngine.UI;

namespace Gruel.Obelisk {
	public class ObeliskStackTrace : MonoBehaviour {
		
#region Properties
		public ObeliskColorSet ColorSet {
			get => _colorSet;
			set {
				_colorSet = value;
				ColorSetChanged();
			}
		}	
#endregion Properties
		
#region Fields
		[Header("Main")]
		[SerializeField] private GameObject _container;
		[SerializeField] private Image _containerBackgroundImage;
		[SerializeField] private Outline _outline;
		[SerializeField] private Image _resizeHandleBackgroundImage;
		[SerializeField] private Image _resizeHandleImage;

		[Header("Titlebar")]
		[SerializeField] private Image _titlebarBackgroundImage;
		[SerializeField] private Image _titlebarIconImage;
		[SerializeField] private Text _titlebarTitleText;
		[SerializeField] private Button _closeButton;
		[SerializeField] private Image _closeButtonBackgroundImage;
		[SerializeField] private Image _closeButtonIconImage;

		[Header("Scrollbar")]
		[SerializeField] private ScrollRect _scrollRect;
		[SerializeField] private Scrollbar _scrollbar;
		[SerializeField] private Image _scrollbarBackgroundImage;

		[Header("Content")]
		[SerializeField] private Text _stackTraceText;

		private ObeliskColorSet _colorSet;
#endregion Fields

#region Public Methods
		public void Open(ConsoleLog consoleLog) {
			_stackTraceText.text = consoleLog.LogString + "\n\n" + consoleLog.StackTrace;
			SetEnabled(true);
			_scrollRect.normalizedPosition = new Vector2(0, 1);
		}	
#endregion Public Methods

#region Private Methods
		private void Awake() {
			_closeButton.onClick.AddListener(OnCloseButtonClicked);
		}
		
		private void SetEnabled(bool enable) {
			_container.SetActive(enable);
		}

		private void OnCloseButtonClicked() {
			SetEnabled(false);
		}
		
		private void ColorSetChanged() {
			// Main.
			_containerBackgroundImage.color = _colorSet.BackgroundColor;
			_outline.effectColor = _colorSet.OutlineColor;
			_resizeHandleBackgroundImage.color = _colorSet.InputContainerBackgroundColor;
			_resizeHandleImage.color = _colorSet.ButtonColor;

			// Titlebar.
			_titlebarBackgroundImage.color = _colorSet.TitlebarBackgroundColor;
			_titlebarIconImage.color = _colorSet.IconColor;
			_titlebarTitleText.color = _colorSet.TitlebarTextColor;
			_closeButtonBackgroundImage.color = _colorSet.ButtonColor;
			_closeButtonIconImage.color = _colorSet.IconColor;

			// Scrollbar.
			_scrollbarBackgroundImage.color = _colorSet.ScrollbarBackgroundColor;

			var scrollbarColorBlock = new ColorBlock();
			scrollbarColorBlock.normalColor = _colorSet.ScrollbarSliderColor;
			scrollbarColorBlock.highlightedColor = _colorSet.ScrollbarSliderHighlightedColor;
			scrollbarColorBlock.pressedColor = _colorSet.ScrollbarSliderPressedColor;
			scrollbarColorBlock.colorMultiplier = 1.0f;
			_scrollbar.colors = scrollbarColorBlock;

			// Content.
			_stackTraceText.color = _colorSet.StackTraceTextColor;
		}
#endregion Private Methods
		
	}
}