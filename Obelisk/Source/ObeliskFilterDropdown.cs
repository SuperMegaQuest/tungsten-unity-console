using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HUDConsole {
public class ObeliskFilterDropdown : MonoBehaviour {
	
	public bool GetFilterSetting(LogType logType) {
		return _filterSettings[logType];
	}

	public void SubscribeToFilterChanges(Action callback) {
		FilterSettingsChanged += callback;
	}

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

	private bool _dropdownActive = false;

	private Dictionary<LogType, bool> _filterSettings = new Dictionary<LogType, bool> {
		{ LogType.Error, false },
		{ LogType.Assert, false },
		{ LogType.Warning, false },
		{ LogType.Log, false },
		{ LogType.Exception, false },
	};

	private Action FilterSettingsChanged;

#region Init
	private void Awake() {
		SetupComponents();
		ApplyColorSet();
	}

	private void SetupComponents() {
		// Dropdown.
		_dropdownButton.onClick.AddListener(delegate { DropdownButtonHandler(_dropdownButton); });

		// Error.
		_errorToggle.onValueChanged.AddListener(delegate { ErrorToggleHandler(_errorToggle); });

		// Assert.
		_assertToggle.onValueChanged.AddListener(delegate { AssertToggleHandler(_assertToggle); });

		// Warning.
		_warningToggle.onValueChanged.AddListener(delegate { WarningToggleHandler(_warningToggle); });

		// Log.
		_logToggle.onValueChanged.AddListener(delegate { LogToggleHandler(_logToggle); });

		// Exception.
		_exceptionToggle.onValueChanged.AddListener(delegate { ExceptionToggleHandler(_exceptionToggle); });
	}
#endregion Init

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

			_togglesContainer.SetActive(false);
		} else {
			_dropdownActive = true;

			_togglesContainer.SetActive(true);
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
	
}
}