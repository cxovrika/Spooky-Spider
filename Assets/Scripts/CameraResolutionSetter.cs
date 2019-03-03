using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolutionSetter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		float width = Screen.width;
		float height= Screen.height;

		if (16f * width > 9f * height) {
			width = height * 9f / 16f;
		} else {
			height = width * 16f / 9f;
		}

		Screen.SetResolution ((int)width, (int)height, false);
	}

}
