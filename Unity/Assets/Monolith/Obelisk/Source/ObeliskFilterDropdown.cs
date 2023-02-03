using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gruel.Obelisk {
	public class ObeliskFilterDropdown : MonoBehaviour {

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
		[Header("Dropdown")]
		[SerializeField] private GameObject _togglesContainer;
		[SerializeField] private Button _dropdownButton;
		[SerializeField] private Image _dropdownButtonImage;
		[SerializeField] private Image _dropdownSymbolBackgroundImage;
		[SerializeField] private Image _dropdownSymbolImage;
		[SerializeField] private Image _dropdownArrowImage;

		[Header("Error")]
		[SerializeField] private Toggle _errorToggle;
		[SerializeField] private Image _errorToggleButtonImage;
		[SerializeField] private Image _errorToggleIconImage;
		[SerializeField] private Image _errorToggleIconBackgroundImage;

		[Header("Assert")]
		[SerializeField] private Toggle _assertToggle;
		[SerializeField] private Image _assertToggleButtonImage;
		[SerializeField] private Image _assertToggleIconImage;
		[SerializeField] private Image _assertToggleIconBackgroundImage;

		[Header("Warning")]
		[SerializeField] private Toggle _warningToggle;
		[SerializeField] private Image _warningToggleButtonImage;
		[SerializeField] private Image _warningToggleIconImage;
		[SerializeField] private Image _warningToggleIconBackgroundImage;

		[Header("Log")]
		[SerializeField] private Toggle _logToggle;
		[SerializeField] private Image _logToggleButtonImage;
		[SerializeField] private Image _logToggleIconImage;
		[SerializeField] private Image _logToggleIconIconBackgroundImage;

		[Header("Exception")]
		[SerializeField] private Toggle _exceptionToggle;
		[SerializeField] private Image _exceptionToggleButtonImage;
		[SerializeField] private Image _exceptionToggleIconImage;
		[SerializeField] private Image _exceptionToggleIconBackgroundImage;

		private bool _dropdownActive;
		private ObeliskColorSet _colorSet;

		private Dictionary<LogType, bool> _filterSettings = new Dictionary<LogType, bool> {
			{ LogType.Error, false },
			{ LogType.Assert, false },
			{ LogType.Warning, false },
			{ LogType.Log, false },
			{ LogType.Exception, false },
		};

		private Action _filterSettingsChanged;
#endregion Fields

#region Public Methods
		public bool GetFilterSetting(LogType logType) {
			return _filterSettings[logType];
		}

		public void AddFilterChangedListener(Action callback) {
			_filterSettingsChanged += callback;
		}

		public void RemoveFilterChangedListener(Action callback) {
			_filterSettingsChanged -= callback;
		}
#endregion Public Methods

#region Private Methods
		private void Awake() {
			// Dropdown.
			_dropdownButton.onClick.AddListener(() => DropdownButtonHandler(_dropdownButton));

			// Error.
			_errorToggle.onValueChanged.AddListener(evt => ErrorToggleHandler(_errorToggle));

			// Assert.
			_assertToggle.onValueChanged.AddListener(evt => AssertToggleHandler(_assertToggle));

			// Warning.
			_warningToggle.onValueChanged.AddListener(evt => WarningToggleHandler(_warningToggle));

			// Log.
			_logToggle.onValueChanged.AddListener(evt => LogToggleHandler(_logToggle));

			// Exception.
			_exceptionToggle.onValueChanged.AddListener(evt => ExceptionToggleHandler(_exceptionToggle));
		}

		private void DropdownButtonHandler(Button target) {
			ToggleState();
		}

		private void ToggleState() {
			if (_dropdownActive) {
				_dropdownActive = false;

				_togglesContainer.SetActive(false);
			} else {
				_dropdownActive = true;

				_togglesContainer.SetActive(true);
			}
		}

		private void ErrorToggleHandler(Toggle target) {
			_filterSettings[LogType.Error] = target.isOn;
			_filterSettingsChanged?.Invoke();
			_errorToggleIconImage.color = target.isOn ? _colorSet.IconDisabledColor : _colorSet.IconColor;
		}

		private void AssertToggleHandler(Toggle target) {
			_filterSettings[LogType.Assert] = target.isOn;
			_filterSettingsChanged?.Invoke();
			_assertToggleIconImage.color = target.isOn ? _colorSet.IconDisabledColor : _colorSet.IconColor;
		}

		private void WarningToggleHandler(Toggle target) {
			_filterSettings[LogType.Warning] = target.isOn;
			_filterSettingsChanged?.Invoke();
			_warningToggleIconImage.color = target.isOn ? _colorSet.IconDisabledColor : _colorSet.IconColor;
		}

		private void LogToggleHandler(Toggle target) {
			_filterSettings[LogType.Log] = target.isOn;
			_filterSettingsChanged?.Invoke();
			_logToggleIconImage.color = target.isOn ? _colorSet.IconDisabledColor : _colorSet.IconColor;
		}

		private void ExceptionToggleHandler(Toggle target) {
			_filterSettings[LogType.Exception] = target.isOn;
			_filterSettingsChanged?.Invoke();
			_exceptionToggleIconImage.color = target.isOn ? _colorSet.IconDisabledColor : _colorSet.IconColor;
		}

		private void ColorSetChanged() {
			_dropdownSymbolBackgroundImage.color = _colorSet.IconBackgroundColor;
			_dropdownSymbolImage.color = _colorSet.IconColor;
			_dropdownButtonImage.color = _colorSet.ButtonColor;
			_dropdownArrowImage.color = _colorSet.IconColor;

			_errorToggleIconBackgroundImage.color = _colorSet.IconBackgroundColor;
			_assertToggleIconBackgroundImage.color = _colorSet.IconBackgroundColor;
			_warningToggleIconBackgroundImage.color = _colorSet.IconBackgroundColor;
			_logToggleIconIconBackgroundImage.color = _colorSet.IconBackgroundColor;
			_exceptionToggleIconBackgroundImage.color = _colorSet.IconBackgroundColor;

			_errorToggleButtonImage.color = _colorSet.ButtonColor;
			_assertToggleButtonImage.color = _colorSet.ButtonColor;
			_warningToggleButtonImage.color = _colorSet.ButtonColor;
			_logToggleButtonImage.color = _colorSet.ButtonColor;
			_exceptionToggleButtonImage.color = _colorSet.ButtonColor;

			_errorToggleIconImage.color = _colorSet.IconColor;
			_assertToggleIconImage.color = _colorSet.IconColor;
			_warningToggleIconImage.color = _colorSet.IconColor;
			_logToggleIconImage.color = _colorSet.IconColor;
			_exceptionToggleIconImage.color = _colorSet.IconColor;
		}
#endregion Private Methods

	}
}