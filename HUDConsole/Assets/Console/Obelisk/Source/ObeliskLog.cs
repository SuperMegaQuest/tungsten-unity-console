using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HUDConsole {
	public class ObeliskLog : MonoBehaviour {
#region Public
		public ConsoleLog ConsoleLog { get; private set; }

		public RectTransform RectTransform { get; private set; }

		public void SetLog(ref ConsoleLog log) {
			ConsoleLog = log;

			if (log.customColor) {
				SetColors(log.textColor, log.bgColor);
			} else {
				SetColors(log.logType);
			}

			_text.text = log.logString;

			if (string.IsNullOrEmpty(ConsoleLog.stackTrace) == false) {
				_stackTraceGameObject.SetActive(true);
			}
		}
#endregion Public

#region Private
		private Image _background;
		private Button _button;
		private Text _text;
		private GameObject _stackTraceGameObject;
		private Image _stackTraceImage;

		private const float _heightIncrement = 20.0f;

		private void Awake() {
			GetComponents();
		}

		private void OnRectTransformDimensionsChange() {
			float heightMultiple = Mathf.Ceil(_text.preferredHeight / ObeliskLog._heightIncrement);
			float newHeight = heightMultiple * _heightIncrement;

			RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, newHeight);
		}

		private void GetComponents() {
			RectTransform = GetComponent<RectTransform>();

			_background = transform.Find("Button").GetComponent<Image>();
			_text = transform.Find("Text").GetComponent<Text>();

			_button = transform.Find("Button").GetComponent<Button>();
			_button.onClick.AddListener(delegate { ButtonHandler(_button); });
			_stackTraceGameObject = transform.Find("Image_StackTrace").gameObject;
			_stackTraceImage = _stackTraceGameObject.GetComponent<Image>();
			_stackTraceImage.color = ObeliskConsole.ColorSet.IconColor;
		}

		private void SetColors(LogType logType) {
			var newBackgroundColor = ObeliskConsole.ColorSet.LogBackgroundColor(logType);

			if (transform.parent.childCount % 2 == 0) {
				newBackgroundColor += new Color(0.015f, 0.015f, 0.015f, 1.0f);
			}

			_background.color = newBackgroundColor;
			_text.color = ObeliskConsole.ColorSet.LogTextColor(logType);
		}

		private void SetColors(Color textColor, Color backgroundColor) {
			_background.color = backgroundColor;
			_text.color = textColor;
		}

		private void ButtonHandler(Button target) {
			if (ConsoleLog.stackTrace == "") { return; }

			ObeliskConsole.OpenStackTraceForLog(ConsoleLog);
		}
#endregion Private
	}
}