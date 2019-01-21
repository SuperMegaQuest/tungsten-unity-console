using System;
using UnityEngine;

namespace HUDConsole {
public class ListenForDimensionsChange : MonoBehaviour {
	
	public void SubscribeToDimensionsChange(Action callback) {
		OnDimensionsChanged += callback;
	}

	private Action OnDimensionsChanged;

	private void OnRectTransformDimensionsChange() {
		if (OnDimensionsChanged != null) {
			OnDimensionsChanged();
		}
	}
	
}
}