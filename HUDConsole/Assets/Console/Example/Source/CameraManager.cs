using UnityEngine;
using HUDConsole;

public class CameraManager : MonoBehaviour {
	private Camera m_camera;

	private void Awake() {
		m_camera = GetComponent<Camera>();

		Console.AddCommand("CameraManager.SetFOV", CameraManagerSetFOV, "Set the camera's FOV.");
	}

	private void CameraManagerSetFOV(string[] args) {
		m_camera.fieldOfView = int.Parse(args[0]);
	}
}