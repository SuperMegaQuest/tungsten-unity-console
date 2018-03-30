using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HUDConsole {
	public class ObeliskLog : MonoBehaviour {
#region Public
		public ConsoleLog consoleLog {
			get; private set;
		}

		public RectTransform rectTransform {
			get; private set;
		}

		public void SetLog(ref ConsoleLog log) {
			consoleLog = log;

			if(log.customColor) {
				SetColors(log.textColor, log.bgColor);
			}
			else {
				SetColors(log.logType);
			}

			m_text.text = log.logString;

			if(string.IsNullOrEmpty(consoleLog.stackTraceString) == false) {
				m_stackTraceGameObject.SetActive(true);
			}
		}
#endregion Public

#region Private
		private Image m_background;
		private Button m_button;
		private Text m_text;
		private GameObject m_stackTraceGameObject;
		private Image m_stackTraceImage;

		private const float m_heightIncrement = 20.0f;

		private void Awake() {
			GetComponents();
		}

		private void OnRectTransformDimensionsChange() {
			float heightMultiple = Mathf.Ceil(m_text.preferredHeight / ObeliskLog.m_heightIncrement);
			float newHeight = heightMultiple * m_heightIncrement;

			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, newHeight);
		}

		private void GetComponents() {
			rectTransform = GetComponent<RectTransform>();

			m_background = transform.Find("Button").GetComponent<Image>();
			m_text = transform.Find("Text").GetComponent<Text>();

			m_button = transform.Find("Button").GetComponent<Button>();
			m_button.onClick.AddListener(delegate { ButtonHandler(m_button); });
			m_stackTraceGameObject = transform.Find("Image_StackTrace").gameObject;
			m_stackTraceImage = m_stackTraceGameObject.GetComponent<Image>();
			m_stackTraceImage.color = ObeliskConsole.colorSet.iconColor;
		}

		private void SetColors(LogType logType) {
			var newBackgroundColor = ObeliskConsole.colorSet.LogBackgroundColor(logType);

			if(transform.parent.childCount % 2 == 0) {
				newBackgroundColor += new Color(0.015f, 0.015f, 0.015f, 1.0f);
			}

			m_background.color = newBackgroundColor;
			m_text.color = ObeliskConsole.colorSet.LogTextColor(logType);
		}

		private void SetColors(Color textColor, Color backgroundColor) {
			m_background.color = backgroundColor;
			m_text.color = textColor;
		}

		private void ButtonHandler(Button target) {
			if (consoleLog.stackTraceString == "") { return; }

			ObeliskConsole.OpenStackTraceForLog(consoleLog);
		}
#endregion Private
	}
}