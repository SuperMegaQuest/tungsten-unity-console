using System;

using UnityEngine;

namespace Monolith.UGUI {
public class ListenForDimensionsChange : MonoBehaviour {

#region Fields
    private Action _onDimensionsChanged;
#endregion Fields

#region Private Methods
    private void OnRectTransformDimensionsChange() {
        _onDimensionsChanged?.Invoke();
    }
#endregion Private Methods

#region Public Methods
    public void AddDimensionsChangedListener(Action callback) {
        _onDimensionsChanged += callback;
    }

    public void RemoveDimensionsChangedListener(Action callback) {
        _onDimensionsChanged -= callback;
    }
#endregion Public Methods

}
}