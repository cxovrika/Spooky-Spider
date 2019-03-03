using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour {


	public GameObject Wall;
	public GameObject IceWall;
	public GameObject Walls;

	private Controller controller;

	private float currentHeight = 61.3f;
	private float buildingDifference = 40f;
	private float leftWallX = -5.65f;
	private float rightWallX = 4.7f;


	// Use this for initialization
	void Start () {
		controller = transform.gameObject.GetComponent<Controller> ();
	}
	
	// Update is called once per frame
	void Update () {
		addWallsIfNeeded ();
		destroyNotNeededWalls ();
	}

	private void destroyNotNeededWalls() {
		GameObject[] walls = GameObject.FindGameObjectsWithTag ("Wall");
		GameObject[] ices = GameObject.FindGameObjectsWithTag ("Ice");

		for (int i=0; i<walls.Length; i++){
			GameObject wall = walls [i];

			if (wall.transform.position.y < controller.bodyRB.gameObject.transform.position.y - buildingDifference) {
				Destroy (wall);
			}
		}

		for (int i=0; i<ices.Length; i++){
			GameObject ice = ices [i];

			if (ice.transform.position.y < controller.bodyRB.gameObject.transform.position.y - buildingDifference) {
				Destroy (ice);
			}
		}
			
	}


	private void addWallsIfNeeded() {
		while (controller.bodyRB.gameObject.transform.position.y + buildingDifference > currentHeight) {
			addWalls ();
		}
	}

	private void addWalls() {
		currentHeight += 1f;
		float peak = 1000 + controller.currentScore;

		float dif = Random.Range (0f, peak);

		if (dif < 400f) {
			Instantiate (Wall, new Vector3 (leftWallX, currentHeight, 0f), Quaternion.Euler (0f, 0f, 90f), Walls.transform);
			Instantiate (Wall, new Vector3 (rightWallX, currentHeight, 0f), Quaternion.Euler (0f, 0f, 90f), Walls.transform);
		} else if (dif < 600f) {
			if (dif < 500f) {
				Instantiate (IceWall, new Vector3 (leftWallX, currentHeight, 0f), Quaternion.Euler (0f, 0f, 90f), Walls.transform);
				Instantiate (Wall, new Vector3 (rightWallX, currentHeight, 0f), Quaternion.Euler (0f, 0f, 90f), Walls.transform);
			} else {
				Instantiate (Wall, new Vector3 (leftWallX, currentHeight, 0f), Quaternion.Euler (0f, 0f, 90f), Walls.transform);
				Instantiate (IceWall, new Vector3 (rightWallX, currentHeight, 0f), Quaternion.Euler (0f, 0f, 90f), Walls.transform);
			}
		} else if (dif < 700f) {
			Instantiate (IceWall, new Vector3 (leftWallX, currentHeight, 0f), Quaternion.Euler (0f, 0f, 90f), Walls.transform);
			Instantiate (IceWall, new Vector3 (rightWallX, currentHeight, 0f), Quaternion.Euler (0f, 0f, 90f), Walls.transform);
		} else if (dif < 900f) {
			if (dif < 850f) {
				Instantiate (Wall, new Vector3 (leftWallX, currentHeight, 0f), Quaternion.Euler (0f, 0f, 90f), Walls.transform);
			} else {
				Instantiate (Wall, new Vector3 (rightWallX, currentHeight, 0f), Quaternion.Euler (0f, 0f, 90f), Walls.transform);
			}
		} else	{
			if (dif < 900f + (peak - 900f) / 2) {
				Instantiate (IceWall, new Vector3 (leftWallX, currentHeight, 0f), Quaternion.Euler (0f, 0f, 90f), Walls.transform);
			} else {
				Instantiate (IceWall, new Vector3 (rightWallX, currentHeight, 0f), Quaternion.Euler (0f, 0f, 90f), Walls.transform);
			}
		}

	}
}
