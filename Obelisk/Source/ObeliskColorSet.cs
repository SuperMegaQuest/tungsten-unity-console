using System.Collections.Generic;
using UnityEngine;

namespace HUDConsole {
	[CreateAssetMenu(menuName = "Console/Obelisk ColorSet", fileName = "ObeliskColorSet_")]
	public class ObeliskColorSet : ScriptableObject {
#region Public
		// General.
		public Color IconBackgroundColor {
			get { return _iconBackgroundColor; }
		}

		public Color IconColor {
			get { return _iconColor; }
		}

		public Color IconDisabledColor {
			get { return _iconDisabledColor; }
		}

		public Color ButtonColor {
			get { return _buttonColor; }
		}

		public Color StackTraceTextColor {
			get { return _stackTraceTextColor; }
		}

		// Main.
		public Color BackgroundColor {
			get { return _backgroundColor; }
		}

		public Color OutlineColor {
			get { return _outlineColor; }
		}

		// Titlebar.
		public Color TitlebarBackgroundColor {
			get { return _titlebarBackgroundColor; }
		}

		public Color TitlebarTextColor {
			get { return _titlebarTextColor; }
		}

		// Scrollbar.
		public Color ScrollbarBackgroundColor {
			get { return _scrollbarBackgroundColor; }
		}

		public Color ScrollbarSliderColor {
			get { return _scrollbarSliderColor; }
		}

		public Color ScrollbarSliderHighlightedColor {
			get { return _scrollbarSliderHighlightedColor; }
		}

		public Color ScrollbarSliderPressedColor {
			get { return _scrollbarSliderPressedColor; }
		}

		// Input.
		public Color InputContainerBackgroundColor {
			get { return _inputContainerBackgroundColor; }
		}

		public Color InputTextColor {
			get { return _inputTextColor; }
		}

		// Text.
		public Color LogTextColor(LogType logType) {
			return _logTextColor[logType];
		}

		public Color LogBackgroundColor(LogType logType) {
			return _logBackgroundColor[logType];
		}
#endregion Public

#region Private
		[Header("General")]
		[SerializeField] private Color _iconBackgroundColor;
		[SerializeField] private Color _iconColor;
		[SerializeField] private Color _iconDisabledColor;
		[SerializeField] private Color _buttonColor;
		[SerializeField] private Color _stackTraceTextColor;

		[Header("Main")]
		[SerializeField] private Color _backgroundColor;
		[SerializeField] private Color _outlineColor;

		[Header("Titlebar")]
		[SerializeField] private Color _titlebarBackgroundColor;
		[SerializeField] private Color _titlebarTextColor;

		[Header("Scrollbar")]
		[SerializeField] private Color _scrollbarBackgroundColor;
		[SerializeField] private Color _scrollbarSliderColor;
		[SerializeField] private Color _scrollbarSliderHighlightedColor;
		[SerializeField] private Color _scrollbarSliderPressedColor;

		[Header("Input")]
		[SerializeField] private Color _inputContainerBackgroundColor;
		[SerializeField] private Color _inputTextColor;

		[Header("Log Text")]
		[SerializeField] private Color _logTextErrorColor;
		[SerializeField] private Color _logTextAssertColor;
		[SerializeField] private Color _logTextWarningColor;
		[SerializeField] private Color _logTextLogColor;
		[SerializeField] private Color _logTextExceptionColor;

		[Header("Log Background")]
		[SerializeField]
		private Color _logBackgroundErrorColor;

		[SerializeField] private Color _logBackgroundAssertColor;
		[SerializeField] private Color _logBackgroundWarningColor;
		[SerializeField] private Color _logBackgroundLogColor;
		[SerializeField] private Color _logBackgroundExceptionColor;

		private Dictionary<LogType, Color> _logTextColor = new Dictionary<LogType, Color>();
		private Dictionary<LogType, Color> _logBackgroundColor = new Dictionary<LogType, Color>();

		private void Awake() {
			_logTextColor.Add(LogType.Error, _logTextErrorColor);
			_logTextColor.Add(LogType.Assert, _logTextAssertColor);
			_logTextColor.Add(LogType.Warning, _logTextWarningColor);
			_logTextColor.Add(LogType.Log, _logTextLogColor);
			_logTextColor.Add(LogType.Exception, _logTextExceptionColor);

			_logBackgroundColor.Add(LogType.Error, _logBackgroundErrorColor);
			_logBackgroundColor.Add(LogType.Assert, _logBackgroundAssertColor);
			_logBackgroundColor.Add(LogType.Warning, _logBackgroundWarningColor);
			_logBackgroundColor.Add(LogType.Log, _logBackgroundLogColor);
			_logBackgroundColor.Add(LogType.Exception, _logBackgroundExceptionColor);
		}
#endregion Private
	}
}