using System;
using UnityEngine;

namespace HUDConsole {
	public class ObeliskResize : MonoBehaviour {
#region Public
		public void BeginResizeDrag() {
			m_resizePosBegin = m_container.localPosition;
			m_resizeMousePosBegin = Input.mousePosition;
			m_resizeSizeBegin = m_container.sizeDelta;
		}

		public void ResizeDrag() {
			// Stop resizing when mouse is off screen.
			if(Input.mousePosition.x >= Screen.width
			|| Input.mousePosition.x <= 0f
			|| Input.mousePosition.y >= Screen.height
			|| Input.mousePosition.y <= 0f) {
				return;
			}

			// Calculate new size.
			var xSize = m_resizeSizeBegin.x + ((Input.mousePosition.x - m_resizeMousePosBegin.x) * 0.75f);
			var ySize = m_resizeSizeBegin.y - ((Input.mousePosition.y - m_resizeMousePosBegin.y) * 0.75f);
			xSize = Mathf.Clamp(xSize, m_resizeMin.x, Screen.width);
			ySize = Mathf.Clamp(ySize, m_resizeMin.y, Screen.height);
			m_container.sizeDelta = new Vector2(xSize, ySize);

			// Offset position to maintain relative position.
			// This is needed because size is applied in both directions.
			var newPos = new Vector2();
			var difference = m_container.sizeDelta - m_resizeSizeBegin;
			newPos.x = m_resizePosBegin.x + (difference.x * 0.5f);
			newPos.y = m_resizePosBegin.y - (difference.y * 0.5f);
			m_container.localPosition = newPos;
		}
#endregion Public

#region Private
		private RectTransform m_container;

		private Vector3 m_resizePosBegin = Vector3.zero;
		private Vector3 m_resizeMousePosBegin = Vector3.zero;
		private Vector2 m_resizeSizeBegin = Vector3.zero;

		[Tooltip("Minimum size the console can be set to.")]
		[SerializeField] private Vector2 m_resizeMin = new Vector2(256f, 128f);

		private void Awake() {
			m_container = transform.Find("Container").GetComponent<RectTransform>();
		}
#endregion Private
	}
}