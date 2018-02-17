using UnityEngine;

namespace HUDConsole {
	public class ObeliskDrag : MonoBehaviour {
#region Public
		public void BeginDrag() {
			m_dragOffsetBegin = m_container.position - Input.mousePosition;
		}

		public void Drag() {
			if(Input.mousePosition.x >= Screen.width
			|| Input.mousePosition.x <= 0f
			|| Input.mousePosition.y >= Screen.height
			|| Input.mousePosition.y <= 0f) {
				return;
			}

			var x = Input.mousePosition.x + m_dragOffsetBegin.x;
			var y = Input.mousePosition.y + m_dragOffsetBegin.y;
			m_container.position = new Vector3(x, y, 0f);
		}
#endregion Public

#region Private
		private Transform m_container;

		private Vector3 m_dragOffsetBegin = Vector3.zero;

		private void Awake() {
			m_container = transform.Find("Container");
		}
#endregion Private
	}
}