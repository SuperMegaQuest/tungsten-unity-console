using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HUDConsole {
	public class ObeliskFilterDropdown : MonoBehaviour {
#region Public
		public bool GetFilterSetting(LogType logType) {
			return _filterSettings[logType];
		}

		public void SubscribeToFilterChanges(Action callback) {
			FilterSettingsChanged += callback;
		}
#endregion Public

#region Private
		private GameObject _toggles;

		// Dropdown.
		private Button _dropdownButton;
		private Image _dropdownButtonImage;
		private Image _dropdownSymbolBackgroundImage;
		private Image _dropdownSymbolImage;
		private Image _dropdownArrowImage;

		// Error.
		private Toggle _errorToggle;
		private Image _errorToggleIconImage;
		private Image _errorToggleIconBackgroundImage;
		private Image _errorToggleButtonImage;

		// Assert.
		private Toggle _assertToggle;
		private Image _assertToggleIconImage;
		private Image _assertToggleIconBackgroundImage;
		private Image _assertToggleButtonImage;

		// Warning
		private Toggle _warningToggle;
		private Image _warningToggleIconImage;
		private Image _warningToggleIconBackgroundImage;
		private Image _warningToggleButtonImage;

		// Log
		private Toggle _logToggle;
		private Image _logToggleIconImage;
		private Image _logToggleIconIconBackgroundImage;
		private Image _logToggleButtonImage;

		// Exception.
		private Toggle _exceptionToggle;
		private Image _exceptionToggleIconImage;
		private Image _exceptionToggleIconBackgroundImage;
		private Image _exceptionToggleButtonImage;

		private bool _dropdownActive = false;

		private Dictionary<LogType, bool> _filterSettings = new Dictionary<LogType, bool> {
			{ LogType.Error, false },
			{ LogType.Assert, false },
			{ LogType.Warning, false },
			{ LogType.Log, false },
			{ LogType.Exception, false },
		};

		private Action FilterSettingsChanged;

		private void Awake() {
			GetComponents();
			ApplyColorSet();
		}

		private void GetComponents() {
			_toggles = transform.Find("Toggles").gameObject;

			// Dropdown.
			_dropdownButton = transform.Find("Button").GetComponent<Button>();
			_dropdownButtonImage = transform.Find("Button").GetComponent<Image>();
			_dropdownButton.onClick.AddListener(delegate { DropdownButtonHandler(_dropdownButton); });

			_dropdownSymbolBackgroundImage = transform.Find("Symbol").GetComponent<Image>();
			_dropdownSymbolImage = transform.Find("Symbol/Image").GetComponent<Image>();
			_dropdownArrowImage = transform.Find("Button/Image").GetComponent<Image>();

			// Error.
			_errorToggle = transform.Find("Toggles/Toggle_Error").GetComponent<Toggle>();
			_errorToggleButtonImage = transform.Find("Toggles/Toggle_Error").GetComponent<Image>();
			_errorToggle.onValueChanged.AddListener(delegate { ErrorToggleHandler(_errorToggle); });

			_errorToggleIconImage = transform.Find("Toggles/Toggle_Error/IconBackground/Image").GetComponent<Image>();
			_errorToggleIconBackgroundImage = transform.Find("Toggles/Toggle_Error/IconBackground").GetComponent<Image>();

			// Assert.
			_assertToggle = transform.Find("Toggles/Toggle_Assert").GetComponent<Toggle>();
			_assertToggleButtonImage = transform.Find("Toggles/Toggle_Assert").GetComponent<Image>();
			_assertToggle.onValueChanged.AddListener(delegate { AssertToggleHandler(_assertToggle); });

			_assertToggleIconImage = transform.Find("Toggles/Toggle_Assert/IconBackground/Image").GetComponent<Image>();
			_assertToggleIconBackgroundImage = transform.Find("Toggles/Toggle_Assert/IconBackground").GetComponent<Image>();

			// Warning.
			_warningToggle = transform.Find("Toggles/Toggle_Warning").GetComponent<Toggle>();
			_warningToggleButtonImage = transform.Find("Toggles/Toggle_Warning").GetComponent<Image>();
			_warningToggle.onValueChanged.AddListener(delegate { WarningToggleHandler(_warningToggle); });

			_warningToggleIconImage = transform.Find("Toggles/Toggle_Warning/IconBackground/Image").GetComponent<Image>();
			_warningToggleIconBackgroundImage = transform.Find("Toggles/Toggle_Warning/IconBackground").GetComponent<Image>();

			// Log.
			_logToggle = transform.Find("Toggles/Toggle_Log").GetComponent<Toggle>();
			_logToggleButtonImage = transform.Find("Toggles/Toggle_Log").GetComponent<Image>();
			_logToggle.onValueChanged.AddListener(delegate { LogToggleHandler(_logToggle); });

			_logToggleIconImage = transform.Find("Toggles/Toggle_Log/IconBackground/Image").GetComponent<Image>();
			_logToggleIconIconBackgroundImage = transform.Find("Toggles/Toggle_Log/IconBackground").GetComponent<Image>();

			// Exception.
			_exceptionToggle = transform.Find("Toggles/Toggle_Exception").GetComponent<Toggle>();
			_exceptionToggleButtonImage = transform.Find("Toggles/Toggle_Exception").GetComponent<Image>();
			_exceptionToggle.onValueChanged.AddListener(delegate { ExceptionToggleHandler(_exceptionToggle); });

			_exceptionToggleIconImage = transform.Find("Toggles/Toggle_Exception/IconBackground/Image").GetComponent<Image>();
			_exceptionToggleIconBackgroundImage = transform.Find("Toggles/Toggle_Exception/IconBackground").GetComponent<Image>();
		}

		private void ApplyColorSet() {
			_dropdownSymbolBackgroundImage.color = ObeliskConsole.ColorSet.IconBackgroundColor;
			_dropdownSymbolImage.color = ObeliskConsole.ColorSet.IconColor;
			_dropdownButtonImage.color = ObeliskConsole.ColorSet.ButtonColor;
			_dropdownArrowImage.color = ObeliskConsole.ColorSet.IconColor;

			_errorToggleIconBackgroundImage.color = ObeliskConsole.ColorSet.IconBackgroundColor;
			_assertToggleIconBackgroundImage.color = ObeliskConsole.ColorSet.IconBackgroundColor;
			_warningToggleIconBackgroundImage.color = ObeliskConsole.ColorSet.IconBackgroundColor;
			_logToggleIconIconBackgroundImage.color = ObeliskConsole.ColorSet.IconBackgroundColor;
			_exceptionToggleIconBackgroundImage.color = ObeliskConsole.ColorSet.IconBackgroundColor;

			_errorToggleButtonImage.color = ObeliskConsole.ColorSet.ButtonColor;
			_assertToggleButtonImage.color = ObeliskConsole.ColorSet.ButtonColor;
			_warningToggleButtonImage.color = ObeliskConsole.ColorSet.ButtonColor;
			_logToggleButtonImage.color = ObeliskConsole.ColorSet.ButtonColor;
			_exceptionToggleButtonImage.color = ObeliskConsole.ColorSet.ButtonColor;

			_errorToggleIconImage.color = ObeliskConsole.ColorSet.IconColor;
			_assertToggleIconImage.color = ObeliskConsole.ColorSet.IconColor;
			_warningToggleIconImage.color = ObeliskConsole.ColorSet.IconColor;
			_logToggleIconImage.color = ObeliskConsole.ColorSet.IconColor;
			_exceptionToggleIconImage.color = ObeliskConsole.ColorSet.IconColor;
		}

		private void DropdownButtonHandler(Button target) {
			ToggleState();
		}

		private void ToggleState() {
			if (_dropdownActive) {
				_dropdownActive = false;

				_toggles.SetActive(false);
			} else {
				_dropdownActive = true;

				_toggles.SetActive(true);
			}
		}

		private void ErrorToggleHandler(Toggle target) {
			_filterSettings[LogType.Error] = target.isOn;

			if (FilterSettingsChanged != null) {
				FilterSettingsChanged();
			}

			if (target.isOn) {
				_errorToggleIconImage.color = ObeliskConsole.ColorSet.IconDisabledColor;
			} else {
				_errorToggleIconImage.color = ObeliskConsole.ColorSet.IconColor;
			}
		}

		private void AssertToggleHandler(Toggle target) {
			_filterSettings[LogType.Assert] = target.isOn;

			if (FilterSettingsChanged != null) {
				FilterSettingsChanged();
			}

			if (target.isOn) {
				_assertToggleIconImage.color = ObeliskConsole.ColorSet.IconDisabledColor;
			} else {
				_assertToggleIconImage.color = ObeliskConsole.ColorSet.IconColor;
			}
		}

		private void WarningToggleHandler(Toggle target) {
			_filterSettings[LogType.Warning] = target.isOn;

			if (FilterSettingsChanged != null) {
				FilterSettingsChanged();
			}

			if (target.isOn) {
				_warningToggleIconImage.color = ObeliskConsole.ColorSet.IconDisabledColor;
			} else {
				_warningToggleIconImage.color = ObeliskConsole.ColorSet.IconColor;
			}
		}

		private void LogToggleHandler(Toggle target) {
			_filterSettings[LogType.Log] = target.isOn;

			if (FilterSettingsChanged != null) {
				FilterSettingsChanged();
			}

			if (target.isOn) {
				_logToggleIconImage.color = ObeliskConsole.ColorSet.IconDisabledColor;
			} else {
				_logToggleIconImage.color = ObeliskConsole.ColorSet.IconColor;
			}
		}

		private void ExceptionToggleHandler(Toggle target) {
			_filterSettings[LogType.Exception] = target.isOn;

			if (FilterSettingsChanged != null) {
				FilterSettingsChanged();
			}

			if (target.isOn) {
				_exceptionToggleIconImage.color = ObeliskConsole.ColorSet.IconDisabledColor;
			} else {
				_exceptionToggleIconImage.color = ObeliskConsole.ColorSet.IconColor;
			}
		}
#endregion Private
	}
}