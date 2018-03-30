using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HUDConsole {
	public class ObeliskFilterDropdown : MonoBehaviour {
#region Public
		public bool GetFilterSetting(LogType logType) {
			return m_filterSettings[logType];
		}

		public void SubscribeToFilterChanges(Action callback) {
			FilterSettingsChanged += callback;
		}
#endregion Public

#region Private
		private GameObject m_toggles;

		// Dropdown.
		private Button m_dropdownButton;
		private Image m_dropdownButtonImage;
		private Image m_dropdownSymbolBackgroundImage;
		private Image m_dropdownSymbolImage;
		private Image m_dropdownArrowImage;

		// Error.
		private Toggle m_errorToggle;
		private Image m_errorToggleIconImage;
		private Image m_errorToggleIconBackgroundImage;
		private Image m_errorToggleButtonImage;

		// Assert.
		private Toggle m_assertToggle;
		private Image m_assertToggleIconImage;
		private Image m_assertToggleIconBackgroundImage;
		private Image m_assertToggleButtonImage;

		// Warning
		private Toggle m_warningToggle;
		private Image m_warningToggleIconImage;
		private Image m_warningToggleIconBackgroundImage;
		private Image m_warningToggleButtonImage;

		// Log
		private Toggle m_logToggle;
		private Image m_logToggleIconImage;
		private Image m_logToggleIconIconBackgroundImage;
		private Image m_logToggleButtonImage;

		// Exception.
		private Toggle m_exceptionToggle;
		private Image m_exceptionToggleIconImage;
		private Image m_exceptionToggleIconBackgroundImage;
		private Image m_exceptionToggleButtonImage;

		private bool m_dropdownActive = false;

		private Dictionary<LogType, bool> m_filterSettings = new Dictionary<LogType, bool>() {
		{ LogType.Error, false },  
		{ LogType.Assert, false },  
		{ LogType.Warning, false },  
		{ LogType.Log, false },  
		{ LogType.Exception, false },  
		};

		private Action FilterSettingsChanged;

		private void Awake () {
			GetComponents();
			ApplyColorSet();
		}

		private void GetComponents() {
			m_toggles = transform.Find("Toggles").gameObject;

			// Dropdown.
			m_dropdownButton = transform.Find("Button").GetComponent<Button>();
			m_dropdownButtonImage = transform.Find("Button").GetComponent<Image>();
			m_dropdownButton.onClick.AddListener(delegate { DropdownButtonHandler(m_dropdownButton); });
			
			m_dropdownSymbolBackgroundImage = transform.Find("Symbol").GetComponent<Image>();
			m_dropdownSymbolImage = transform.Find("Symbol/Image").GetComponent<Image>();
			m_dropdownArrowImage = transform.Find("Button/Image").GetComponent<Image>();

			// Error.
			m_errorToggle = transform.Find("Toggles/Toggle_Error").GetComponent<Toggle>();
			m_errorToggleButtonImage = transform.Find("Toggles/Toggle_Error").GetComponent<Image>();
			m_errorToggle.onValueChanged.AddListener(delegate { ErrorToggleHandler(m_errorToggle); });

			m_errorToggleIconImage = transform.Find("Toggles/Toggle_Error/IconBackground/Image").GetComponent<Image>();
			m_errorToggleIconBackgroundImage = transform.Find("Toggles/Toggle_Error/IconBackground").GetComponent<Image>();

			// Assert.
			m_assertToggle = transform.Find("Toggles/Toggle_Assert").GetComponent<Toggle>();
			m_assertToggleButtonImage = transform.Find("Toggles/Toggle_Assert").GetComponent<Image>();
			m_assertToggle.onValueChanged.AddListener(delegate { AssertToggleHandler(m_assertToggle); });

			m_assertToggleIconImage = transform.Find("Toggles/Toggle_Assert/IconBackground/Image").GetComponent<Image>();
			m_assertToggleIconBackgroundImage = transform.Find("Toggles/Toggle_Assert/IconBackground").GetComponent<Image>();

			// Warning.
			m_warningToggle = transform.Find("Toggles/Toggle_Warning").GetComponent<Toggle>();
			m_warningToggleButtonImage = transform.Find("Toggles/Toggle_Warning").GetComponent<Image>();
			m_warningToggle.onValueChanged.AddListener(delegate { WarningToggleHandler(m_warningToggle); });

			m_warningToggleIconImage = transform.Find("Toggles/Toggle_Warning/IconBackground/Image").GetComponent<Image>();
			m_warningToggleIconBackgroundImage = transform.Find("Toggles/Toggle_Warning/IconBackground").GetComponent<Image>();
			
			// Log.
			m_logToggle = transform.Find("Toggles/Toggle_Log").GetComponent<Toggle>();
			m_logToggleButtonImage = transform.Find("Toggles/Toggle_Log").GetComponent<Image>();
			m_logToggle.onValueChanged.AddListener(delegate { LogToggleHandler(m_logToggle); });

			m_logToggleIconImage = transform.Find("Toggles/Toggle_Log/IconBackground/Image").GetComponent<Image>();
			m_logToggleIconIconBackgroundImage = transform.Find("Toggles/Toggle_Log/IconBackground").GetComponent<Image>();

			// Exception.
			m_exceptionToggle = transform.Find("Toggles/Toggle_Exception").GetComponent<Toggle>();
			m_exceptionToggleButtonImage = transform.Find("Toggles/Toggle_Exception").GetComponent<Image>();
			m_exceptionToggle.onValueChanged.AddListener(delegate { ExceptionToggleHandler(m_exceptionToggle); });

			m_exceptionToggleIconImage = transform.Find("Toggles/Toggle_Exception/IconBackground/Image").GetComponent<Image>();
			m_exceptionToggleIconBackgroundImage = transform.Find("Toggles/Toggle_Exception/IconBackground").GetComponent<Image>();
		}

		private void ApplyColorSet() {
			m_dropdownSymbolBackgroundImage.color = ObeliskConsole.ColorSet.iconBackgroundColor;
			m_dropdownSymbolImage.color = ObeliskConsole.ColorSet.iconColor;
			m_dropdownButtonImage.color = ObeliskConsole.ColorSet.buttonColor;
			m_dropdownArrowImage.color = ObeliskConsole.ColorSet.iconColor;

			m_errorToggleIconBackgroundImage.color = ObeliskConsole.ColorSet.iconBackgroundColor;
			m_assertToggleIconBackgroundImage.color = ObeliskConsole.ColorSet.iconBackgroundColor;
			m_warningToggleIconBackgroundImage.color = ObeliskConsole.ColorSet.iconBackgroundColor;
			m_logToggleIconIconBackgroundImage.color = ObeliskConsole.ColorSet.iconBackgroundColor;
			m_exceptionToggleIconBackgroundImage.color = ObeliskConsole.ColorSet.iconBackgroundColor;

			m_errorToggleButtonImage.color = ObeliskConsole.ColorSet.buttonColor;
			m_assertToggleButtonImage.color = ObeliskConsole.ColorSet.buttonColor;
			m_warningToggleButtonImage.color = ObeliskConsole.ColorSet.buttonColor;
			m_logToggleButtonImage.color = ObeliskConsole.ColorSet.buttonColor;
			m_exceptionToggleButtonImage.color = ObeliskConsole.ColorSet.buttonColor;

			m_errorToggleIconImage.color = ObeliskConsole.ColorSet.iconColor;
			m_assertToggleIconImage.color = ObeliskConsole.ColorSet.iconColor;
			m_warningToggleIconImage.color = ObeliskConsole.ColorSet.iconColor;
			m_logToggleIconImage.color = ObeliskConsole.ColorSet.iconColor;
			m_exceptionToggleIconImage.color = ObeliskConsole.ColorSet.iconColor;
		}

		private void DropdownButtonHandler(Button target) {
			ToggleState();
		}

		private void ToggleState() {
			if(m_dropdownActive) {
				m_dropdownActive = false;

				m_toggles.SetActive(false);
			}
			else {
				m_dropdownActive = true;

				m_toggles.SetActive(true);
			}
		}

		private void ErrorToggleHandler(Toggle target) {
			m_filterSettings[LogType.Error] = target.isOn;

			if(FilterSettingsChanged != null) {
				FilterSettingsChanged();
			}

			if(target.isOn) {
				m_errorToggleIconImage.color = ObeliskConsole.ColorSet.iconDisabledColor;
			}
			else {
				m_errorToggleIconImage.color = ObeliskConsole.ColorSet.iconColor;
			}
		}

		private void AssertToggleHandler(Toggle target) {
			m_filterSettings[LogType.Assert] = target.isOn;

			if(FilterSettingsChanged != null) {
				FilterSettingsChanged();
			}

			if(target.isOn) {
				m_assertToggleIconImage.color = ObeliskConsole.ColorSet.iconDisabledColor;
			}
			else {
				m_assertToggleIconImage.color = ObeliskConsole.ColorSet.iconColor;
			}
		}

		private void WarningToggleHandler(Toggle target) {
			m_filterSettings[LogType.Warning] = target.isOn;
			
			if(FilterSettingsChanged != null) {
				FilterSettingsChanged();
			}

			if(target.isOn) {
				m_warningToggleIconImage.color = ObeliskConsole.ColorSet.iconDisabledColor;
			}
			else {
				m_warningToggleIconImage.color = ObeliskConsole.ColorSet.iconColor;
			}
		}

		private void LogToggleHandler(Toggle target) {
			m_filterSettings[LogType.Log] = target.isOn;
			
			if(FilterSettingsChanged != null) {
				FilterSettingsChanged();
			}

			if(target.isOn) {
				m_logToggleIconImage.color = ObeliskConsole.ColorSet.iconDisabledColor;
			}
			else {
				m_logToggleIconImage.color = ObeliskConsole.ColorSet.iconColor;
			}
		}

		private void ExceptionToggleHandler(Toggle target) {
			m_filterSettings[LogType.Exception] = target.isOn;
			
			if(FilterSettingsChanged != null) {
				FilterSettingsChanged();
			}

			if(target.isOn) {
				m_exceptionToggleIconImage.color = ObeliskConsole.ColorSet.iconDisabledColor;
			}
			else {
				m_exceptionToggleIconImage.color = ObeliskConsole.ColorSet.iconColor;
			}
		}
#endregion Private
	}
}