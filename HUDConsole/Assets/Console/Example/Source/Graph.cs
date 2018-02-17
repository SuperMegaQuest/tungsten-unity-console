using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HUDConsole;

public class Graph : MonoBehaviour {
	private bool graphActive = false;
	private bool firstTime = true;

	public Transform pointPrefab;

	[Range(10, 100)]
	public int resolution = 50;

	private const float pi = Mathf.PI;

	public GraphFunctionName function;
	private static GraphFunction[] functions = {
		SineFunction,
		Sine2DFunction,
		MultiSineFunction,
		MultiSine2DFunction,
		Ripple,
		Cylinder,
		Sphere,
		Torus,
	};

	private Transform[] points;

	private void Awake() {
		AddConsoleCommands();
	}

	private void Update() {
		if(graphActive) {
			float t = Time.time;
			float step = 2.0f / resolution;

			GraphFunction f = functions[(int)function];

			for(int i = 0, z = 0; z < resolution; z++) {
				float v = (z + 0.5f) * step - 1.0f;

				for(int x = 0; x < resolution; x++, i++) {
					float u = (x + 0.5f) * step - 1.0f;
					points[i].localPosition = f(u, v, t);
				}
			}
		}
	}

	private void AddConsoleCommands() {
		Console.AddCommand("Graph.SetActive", SetActive, "Set the enabled state of the graph.");
		Console.AddCommand("Graph.SetFunction", SetFunction, "Set graph function to either Ripple, Cylinder, Sphere, or Torus");
	}

	private void SetActive(string[] args) {
		if(args[0].ToLowerInvariant() == "true") {
			graphActive = true;

			if(firstTime) {
				firstTime = false;

				float step = 2.0f / resolution;
				Vector3 scale = Vector3.one * step;
				Vector3 position = Vector3.zero;

				points = new Transform[resolution * resolution];

				for(int i = 0; i < points.Length; i++) {
					Transform point = Instantiate(pointPrefab);
					point.SetParent(transform, false);
					point.localScale = scale;
					points[i] = point;
				}
			}
		}
		else if(args[0].ToLowerInvariant() == "false") {
			graphActive = false;
		}
	}

	private void SetFunction(string[] args) {
		if(args[0] == "Ripple") {
			function = GraphFunctionName.Ripple;
			Console.Log("Switched to GraphFunctionName.Ripple");
		}
		else if(args[0] == "Cylinder") {
			function = GraphFunctionName.Cylinder;
			Console.Log("Switched to GraphFunctionName.Cylinder");
		}
		else if(args[0] == "Sphere") {
			function = GraphFunctionName.Sphere;
			Console.Log("Switched to GraphFunctionName.Sphere");
		}
		else if(args[0] == "Torus") {
			function = GraphFunctionName.Torus;
			Console.Log("Switched to GraphFunctionName.Torus");
		}
		else {
			Console.LogError("Graph Function " + args[0] + " does not exist!");
		}
	}

	static Vector3 SineFunction(float x, float z, float t) {
		Vector3 p;

		p.x = x;
		p.y = Mathf.Sin(pi * (x + t));
		p.z = z;

		return p;
	}

	static Vector3 Sine2DFunction(float x, float z, float t) {
		Vector3 p;

		p.x = x;

		p.y = Mathf.Sin(pi * (x + t));
		p.y += Mathf.Sin(pi * (z + t));
		p.y *= 0.5f;

		p.z = z;

		return p;
	}

	static Vector3 MultiSineFunction(float x, float z, float t) {
		Vector3 p;

		p.x = x;

		p.y = Mathf.Sin(pi * (x + t));
		p.y += Mathf.Sin(2.0f * pi * (x + 2.0f * t)) / 2.0f;
		p.y *= 2.0f / 3.0f;

		p.z = z;

		return p;
	}

	static Vector3 MultiSine2DFunction(float x, float z, float t) {
		Vector3 p;

		p.x = x;

		p.y = 4.0f * Mathf.Sin(pi * (x + z + t / 2.0f));
		p.y += Mathf.Sin(pi * (x + t));
		p.y += Mathf.Sin(2.0f * pi * (z + 2.0f * t)) * 0.5f;
		p.y *= 1.0f / 5.5f;

		p.z = z;

		return p;
	}

	static Vector3 Ripple(float x, float z, float t) {
		Vector3 p;

		float d = Mathf.Sqrt(x * x + z * z);

		p.x = x;
		p.y = Mathf.Sin(pi * (4.0f * d - t));
		p.y /= 1.0f + 10.0f * d;
		p.z = z;

		return p;
	}

	static Vector3 Cylinder(float u, float v, float t) {
		Vector3 p;

		float r = 0.8f + Mathf.Sin(pi * (6.0f * u + 2.0f * v + t)) * 0.2f;

		p.x = r * Mathf.Sin(pi * u);
		p.y = v;
		p.z = r * Mathf.Cos(pi * u);

		return p;
	}

	static Vector3 Sphere(float u, float v, float t) {
		Vector3 p;

		float r = 0.8f + Mathf.Sin(pi * (6f * u + t)) * 0.1f;
		r += Mathf.Sin(pi * (4.0f * v + t)) * 0.1f;

		float s = r * Mathf.Cos(pi * 0.5f * v);

		p.x = s * Mathf.Sin(pi * u);
		p.y = r * Mathf.Sin(pi * 0.5f * v);
		p.z = s * Mathf.Cos(pi * u);

		return p;
	}

	static Vector3 Torus(float u, float v, float t) {
		Vector3 p;

		float r1 = 0.65f + Mathf.Sin(pi * (6.0f * u + t)) * 0.1f;
		float r2 = 0.2f + Mathf.Sin(pi * (4.0f * v + t)) * 0.05f;
		float s = r2 * Mathf.Cos(pi * v) + r1;

		p.x = s * Mathf.Sin(pi * u);
		p.y = r2 * Mathf.Sin(pi * v);
		p.z = s * Mathf.Cos(pi * u);

		return p;
	}
}