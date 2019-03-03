using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDetector : MonoBehaviour {

	public Controller controller;

	public void Start() {
		controller = GetComponent<Controller> ();
	}

	// Update is called once per frame
	void Update () {

		foreach (Touch touch in Input.touches) {
			ManageTouch (touch);
		}
	}

	private void ManageTouch(Touch touch) {
		controller.touchDetected (touch);
	}
}
