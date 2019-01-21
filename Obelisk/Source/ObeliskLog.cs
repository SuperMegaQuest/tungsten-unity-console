using UnityEngine;
using UnityEngine.UI;

namespace HUDConsole {
public class ObeliskLog : MonoBehaviour {
	
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

	[SerializeField] private Image _background;
	[SerializeField] private Button _button;
	[SerializeField] private Text _text;
	[SerializeField] private GameObject _stackTraceGameObject;
	[SerializeField] private Image _stackTraceImage;

	private const float _heightIncrement = 20.0f;

#region Init
	private void Awake() {
		SetupComponents();
	}

	private void OnRectTransformDimensionsChange() {
		float heightMultiple = Mathf.Ceil(_text.preferredHeight / ObeliskLog._heightIncrement);
		float newHeight = heightMultiple * _heightIncrement;

		RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, newHeight);
	}

	private void SetupComponents() {
		RectTransform = GetComponent<RectTransform>();
		_button.onClick.AddListener(delegate { ButtonHandler(_button); });
		_stackTraceImage.color = ObeliskConsole.ColorSet.IconColor;
	}
#endregion Init

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
	
}
}