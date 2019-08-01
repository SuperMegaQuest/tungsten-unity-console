using System;
using UnityEngine;

namespace Gruel.Obelisk {
	public class ListenForDimensionsChange : MonoBehaviour {
		
#region Fields
		private Action _onDimensionsChanged;
#endregion Fields

#region Public Methods
		public void AddDimensionsChangedListener(Action callback) {
			_onDimensionsChanged += callback;
		}

		public void RemoveDimensionsChangedListener(Action callback) {
			_onDimensionsChanged -= callback;
		}
#endregion Public Methods

#region Private Methods
		private void OnRectTransformDimensionsChange() {
			if (_onDimensionsChanged != null) {
				_onDimensionsChanged();
			}
		}
#endregion Private Methods
		
	}
}