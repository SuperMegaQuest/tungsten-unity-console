using System.Collections.Generic;
using UnityEngine;

namespace HUDConsole {
	[CreateAssetMenu(menuName = "Console/Obelisk ColorSet", fileName = "ObeliskColorSet_")]
	public class ObeliskColorSet : ScriptableObject {
#region Public
		// General.
		public Color iconBackgroundColor {
			get { return m_iconBackgroundColor; }
		}
		public Color iconColor {
			get { return m_iconColor; }
		}
		public Color iconDisabledColor{
			get { return m_iconDisabledColor; }
		}
		public Color buttonColor {
			get { return m_buttonColor; }
		}
		public Color stackTraceTextColor {
			get { return m_stackTraceTextColor; }
		}

		// Main.
		public Color backgroundColor {
			get { return m_backgroundColor; }
		}
		public Color outlineColor {
			get { return m_outlineColor; }
		}

		// Titlebar.
		public Color titlebarBackgroundColor {
			get { return m_titlebarBackgroundColor; }
		}
		public Color titlebarTextColor {
			get { return m_titlebarTextColor; }
		}

		// Scrollbar.
		public Color scrollbarBackgroundColor {
			get { return m_scrollbarBackgroundColor; }
		}
		public Color scrollbarSliderColor {
			get { return m_scrollbarSliderColor; }
		}
		public Color scrollbarSliderHighlightedColor { 
			get { return m_scrollbarSliderHighlightedColor; }
		}
		public Color scrollbarSliderPressedColor {
			get { return m_scrollbarSliderPressedColor; }
		}

		// Input.
		public Color inputContainerBackgroundColor {
			get { return m_inputContainerBackgroundColor; }
		}
		public Color inputTextColor {
			get { return m_inputTextColor; }
		}

		// Text.
		public Color LogTextColor(LogType logType) {
			return m_logTextColor[logType];
		}

		public Color LogBackgroundColor(LogType logType) {
			return m_logBackgroundColor[logType];
		}
#endregion Public

#region Private
		[Header("General")]
		[SerializeField] private Color m_iconBackgroundColor;
		[SerializeField] private Color m_iconColor;
		[SerializeField] private Color m_iconDisabledColor;
		[SerializeField] private Color m_buttonColor;
		[SerializeField] private Color m_stackTraceTextColor;

		[Header("Main")]
		[SerializeField] private Color m_backgroundColor;
		[SerializeField] private Color m_outlineColor;

		[Header("Titlebar")]
		[SerializeField] private Color m_titlebarBackgroundColor;
		[SerializeField] private Color m_titlebarTextColor;

		[Header("Scrollbar")]
		[SerializeField] private Color m_scrollbarBackgroundColor;
		[SerializeField] private Color m_scrollbarSliderColor;
		[SerializeField] private Color m_scrollbarSliderHighlightedColor;
		[SerializeField] private Color m_scrollbarSliderPressedColor;

		[Header("Input")]
		[SerializeField] private Color m_inputContainerBackgroundColor;
		[SerializeField] private Color m_inputTextColor;

		[Header("Log Text")]
		[SerializeField] private Color m_logTextErrorColor;
		[SerializeField] private Color m_logTextAssertColor;
		[SerializeField] private Color m_logTextWarningColor;
		[SerializeField] private Color m_logTextLogColor;
		[SerializeField] private Color m_logTextExceptionColor;

		[Header("Log Background")]
		[SerializeField] private Color m_logBackgroundErrorColor;
		[SerializeField] private Color m_logBackgroundAssertColor;
		[SerializeField] private Color m_logBackgroundWarningColor;
		[SerializeField] private Color m_logBackgroundLogColor;
		[SerializeField] private Color m_logBackgroundExceptionColor;

		private Dictionary<LogType, Color> m_logTextColor = new Dictionary<LogType, Color>();
		private Dictionary<LogType, Color> m_logBackgroundColor = new Dictionary<LogType, Color>();

		private void Awake() {
			m_logTextColor.Add(LogType.Error, m_logTextErrorColor);
			m_logTextColor.Add(LogType.Assert, m_logTextAssertColor);
			m_logTextColor.Add(LogType.Warning, m_logTextWarningColor);
			m_logTextColor.Add(LogType.Log, m_logTextLogColor);
			m_logTextColor.Add(LogType.Exception, m_logTextExceptionColor);

			m_logBackgroundColor.Add(LogType.Error, m_logBackgroundErrorColor);
			m_logBackgroundColor.Add(LogType.Assert, m_logBackgroundAssertColor);
			m_logBackgroundColor.Add(LogType.Warning, m_logBackgroundWarningColor);
			m_logBackgroundColor.Add(LogType.Log, m_logBackgroundLogColor);
			m_logBackgroundColor.Add(LogType.Exception, m_logBackgroundExceptionColor);
		}
#endregion Private
	}
}