using UnityEngine;

namespace HUDConsole {
public class ObeliskResize : MonoBehaviour {
	
	public void BeginResizeDrag() {
		_resizePosBegin = _container.localPosition;
		_resizeMousePosBegin = Input.mousePosition;
		_resizeSizeBegin = _container.sizeDelta;
	}

	public void ResizeDrag() {
		// Stop resizing when mouse is off screen.
		if (Input.mousePosition.x >= Screen.width
		|| Input.mousePosition.x <= 0f
		|| Input.mousePosition.y >= Screen.height
		|| Input.mousePosition.y <= 0f) {
			return;
		}

		// Calculate new size.
		var xSize = _resizeSizeBegin.x + ((Input.mousePosition.x - _resizeMousePosBegin.x) * 0.75f);
		var ySize = _resizeSizeBegin.y - ((Input.mousePosition.y - _resizeMousePosBegin.y) * 0.75f);
		xSize = Mathf.Clamp(xSize, _resizeMin.x, Screen.width);
		ySize = Mathf.Clamp(ySize, _resizeMin.y, Screen.height);
		_container.sizeDelta = new Vector2(xSize, ySize);

		// Offset position to maintain relative position.
		// This is needed because size is applied in both directions.
		var newPos = new Vector2();
		var difference = _container.sizeDelta - _resizeSizeBegin;
		newPos.x = _resizePosBegin.x + (difference.x * 0.5f);
		newPos.y = _resizePosBegin.y - (difference.y * 0.5f);
		_container.localPosition = newPos;
	}

	[SerializeField] private RectTransform _container;

	private Vector3 _resizePosBegin = Vector3.zero;
	private Vector3 _resizeMousePosBegin = Vector3.zero;
	private Vector2 _resizeSizeBegin = Vector3.zero;

	[Tooltip("Minimum size the console can be set to.")]
	[SerializeField] private Vector2 _resizeMin = new Vector2(256f, 128f);
	
}
}