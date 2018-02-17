using System;
using UnityEngine;

namespace HUDConsole {
	public class ListenForDimensionsChange : MonoBehaviour {
#region Public
		public void SubscribeToDimensionsChange(Action callback) {
			OnDimensionsChanged += callback;
		}
#endregion Public

#region Private
		private Action OnDimensionsChanged;

		private void OnRectTransformDimensionsChange() {
			if(OnDimensionsChanged != null) {
				OnDimensionsChanged();
			}
		}
#endregion Private
	}
}