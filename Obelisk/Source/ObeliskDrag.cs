using UnityEngine;

namespace HUDConsole {
	public class ObeliskDrag : MonoBehaviour {
#region Public
		public void BeginDrag() {
			_dragOffsetBegin = _container.position - Input.mousePosition;
		}

		public void Drag() {
			if (Input.mousePosition.x >= Screen.width
			|| Input.mousePosition.x <= 0f
			|| Input.mousePosition.y >= Screen.height
			|| Input.mousePosition.y <= 0f) {
				return;
			}

			var x = Input.mousePosition.x + _dragOffsetBegin.x;
			var y = Input.mousePosition.y + _dragOffsetBegin.y;
			_container.position = new Vector3(x, y, 0f);
		}
#endregion Public

#region Private
		[SerializeField] private Transform _container;
		private Vector3 _dragOffsetBegin = Vector3.zero;
#endregion Private
	}
}