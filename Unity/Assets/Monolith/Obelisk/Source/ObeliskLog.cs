using UnityEngine;
using UnityEngine.UI;

namespace Gruel.Obelisk {
	public class ObeliskLog : MonoBehaviour {

#region Properties
		public ConsoleLog ConsoleLog { get; private set; }
		public float Height => _rectTransform.sizeDelta.y;
#endregion Properties

#region Fields
		[SerializeField] private RectTransform _rectTransform;
		[SerializeField] private Image _background;
		[SerializeField] private Button _button;
		[SerializeField] private Text _text;
		[SerializeField] private GameObject _stackTraceGameObject;
		[SerializeField] private Image _stackTraceImage;

		private const float HEIGHT_INCREMENT = 20.0f;

		private ObeliskConsole _obeliskConsole;
		private ObeliskColorSet _colorSet;
#endregion Fields

#region Public Methods
		public void Init(ObeliskConsole obeliskConsole, ObeliskColorSet colorSet) {
			_obeliskConsole = obeliskConsole;
			_colorSet = colorSet;

			_stackTraceImage.color = _colorSet.IconColor;
			_button.onClick.AddListener(OnButtonClicked);
		}

		public void SetLog(ref ConsoleLog log) {
			ConsoleLog = log;

			if (log.CustomColor) {
				SetColors(log.TextColor, log.BgColor);
			} else {
				SetColors(log.LogType);
			}

			_text.text = log.LogString;

			if (string.IsNullOrEmpty(ConsoleLog.StackTrace) == false) {
				_stackTraceGameObject.SetActive(true);
			}
		}
#endregion Public Methods

#region Private Methods
		private void OnButtonClicked() {
			if (string.IsNullOrEmpty(ConsoleLog.StackTrace)) {
				return;
			}

			_obeliskConsole.OpenStackTraceForLog(ConsoleLog);
		}

		private void OnRectTransformDimensionsChange() {
			var heightMultiple = Mathf.Ceil(_text.preferredHeight / HEIGHT_INCREMENT);
			var newHeight = heightMultiple * HEIGHT_INCREMENT;

			_rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, newHeight);
		}

		private void SetColors(LogType logType) {
			var backgroundColor = _colorSet.GetLogBackgroundColor(logType);

			if ((transform.parent.childCount % 2) == 0) {
				backgroundColor += new Color(0.015f, 0.015f, 0.015f, 1.0f);
			}

			_background.color = backgroundColor;
			_text.color = _colorSet.GetLogTextColor(logType);
		}

		private void SetColors(Color textColor, Color backgroundColor) {
			_background.color = backgroundColor;
			_text.color = textColor;
		}
#endregion Private Methods

	}
}