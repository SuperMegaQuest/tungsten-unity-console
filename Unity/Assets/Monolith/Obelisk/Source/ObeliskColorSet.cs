using System.Collections.Generic;
using UnityEngine;

namespace Gruel.Obelisk {
	[CreateAssetMenu(menuName = "Gruel/Console/Obelisk ColorSet Asset", fileName = "ObeliskColorSet_")]
	public class ObeliskColorSet : ScriptableObject {

#region Properties
		// General.
		public Color IconBackgroundColor => _iconBackgroundColor;
		public Color IconColor => _iconColor;
		public Color IconDisabledColor => _iconDisabledColor;
		public Color ButtonColor => _buttonColor;
		public Color StackTraceTextColor => _stackTraceTextColor;

		// Main.
		public Color BackgroundColor => _backgroundColor;
		public Color OutlineColor => _outlineColor;

		// Titlebar.
		public Color TitlebarBackgroundColor => _titlebarBackgroundColor;
		public Color TitlebarTextColor => _titlebarTextColor;

		// Scrollbar.
		public Color ScrollbarBackgroundColor => _scrollbarBackgroundColor;
		public Color ScrollbarSliderColor => _scrollbarSliderColor;
		public Color ScrollbarSliderHighlightedColor => _scrollbarSliderHighlightedColor;
		public Color ScrollbarSliderPressedColor => _scrollbarSliderPressedColor;

		// Input.
		public Color InputContainerBackgroundColor => _inputContainerBackgroundColor;
		public Color InputTextColor => _inputTextColor;
#endregion Properties

#region Fields
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
		[SerializeField] private Color _logBackgroundErrorColor;

		[SerializeField] private Color _logBackgroundAssertColor;
		[SerializeField] private Color _logBackgroundWarningColor;
		[SerializeField] private Color _logBackgroundLogColor;
		[SerializeField] private Color _logBackgroundExceptionColor;

		private Dictionary<LogType, Color> _logTextColor = new Dictionary<LogType, Color>();
		private Dictionary<LogType, Color> _logBackgroundColor = new Dictionary<LogType, Color>();
#endregion Fields

#region Public Methods
		public Color GetLogTextColor(LogType logType) {
			return _logTextColor[logType];
		}

		public Color GetLogBackgroundColor(LogType logType) {
			return _logBackgroundColor[logType];
		}
#endregion Public Methods

#region Private Methods
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
#endregion Private Methods

	}
}