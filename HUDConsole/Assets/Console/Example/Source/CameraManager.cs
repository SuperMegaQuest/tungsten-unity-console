using UnityEngine;
using HUDConsole;

public class CameraManager : MonoBehaviour {
	private Camera _camera;

	private void Awake() {
		_camera = GetComponent<Camera>();

		Console.AddCommand("CameraManager.SetFOV", CameraManagerSetFOV, "Set the camera's FOV.");
	}

	private void CameraManagerSetFOV(string[] args) {
		_camera.fieldOfView = int.Parse(args[0]);
	}
}