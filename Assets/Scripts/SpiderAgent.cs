using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderAgent : Agent {

	private Controller controller;
	private Vector3 pastVelocity;

	void Start () {
		controller = gameObject.GetComponent<Controller> ();
	}

	private Vector2 GetLocalPosition (Vector2 position) {
		return controller.bodyRB.position - position;
	}

	public override List<float> CollectState ()
	{
		List<float> state = new List<float> ();

		//body
		state.Add (controller.bodyRB.transform.position.x);
		state.Add (controller.bodyRB.transform.rotation.z);

		state.Add (controller.bodyRB.velocity.x);
		state.Add (controller.bodyRB.velocity.y);

		state.Add ((controller.bodyRB.velocity.x - pastVelocity.x) / Time.fixedDeltaTime);
		state.Add ((controller.bodyRB.velocity.y - pastVelocity.y) / Time.fixedDeltaTime);
		pastVelocity = controller.bodyRB.velocity;

		//pivots
		state.Add (controller.leftPivotRB.position.x);
		state.Add (controller.leftPivotRB.position.y);

		state.Add (controller.leftPivotRB.velocity.x);
		state.Add (controller.leftPivotRB.velocity.y);

		state.Add (controller.rightPivotRB.position.x);
		state.Add (controller.rightPivotRB.position.y);

		state.Add (controller.rightPivotRB.velocity.x);
		state.Add (controller.rightPivotRB.velocity.y);

		//forces
		state.Add (controller.leftForce.transform.position.x);
		state.Add (controller.leftForce.transform.position.y);

		state.Add (controller.rightForce.transform.position.x);
		state.Add (controller.rightForce.transform.position.y);



		state.Add (controller.getCurrentForce());
		state.Add (controller.heightDifference);
		if (controller.isPressed) {
			state.Add (1f);
		} else {
			state.Add (0f);
		}

		Transform[] limbs = transform.parent.GetChild(2) .gameObject.GetComponentsInChildren<Transform> ();
		//Debug.Log (transform.parent.GetChild (2).name);

		foreach (Transform tr in limbs) {
			GameObject limb = tr.gameObject;
			if (limb.tag != "Limb") {
				continue;
			}
				
			Vector3 position = GetLocalPosition (limb.transform.position);

			state.Add (position.x);
			state.Add (position.y);

			state.Add (limb.transform.rotation.z);
			state.Add (limb.transform.rotation.w);

			Rigidbody2D rb = limb.gameObject.GetComponent<Rigidbody2D> ();
			state.Add (rb.velocity.x);
			state.Add (rb.velocity.y);

			state.Add (rb.angularVelocity);
		}

		if (controller.leftPivotRB.isKinematic) {
			state.Add (1f);
		} else {
			state.Add (0f);
		}

		if (controller.rightPivotRB.isKinematic) {
			state.Add (1f);
		} else {
			state.Add (0f);
		}

		return state;
	}

	//cx. will be deleted after
	//private int STEPCOUNT = 0;

	public override void AgentStep (float[] action)
	{
		
		//Debug.Log ("SETP CALLED: " + STEPCOUNT);
		//STEPCOUNT++;

		if (Time.time < controller.episodeStartTime + 0.777f) {
			reward = 0f;
			//Debug.Log ("STOPPED");
			return;
		}

		reward = -0.05f;

		if (action [0] == 0) {
			controller.DownPressed (0f);
		}

		if (action [0] == 1) {
			controller.DownPressed (1f);
		}

		if (action [0] == 2) {
			reward += controller.getCurrentForce ();
			controller.UpPressed ();
		}

		if (!done) {

			float vel = Mathf.Clamp (controller.bodyRB.velocity.y, -1.3f, 20f);

			reward += (0
				+ 1.0f * vel
			);
		}

		if (controller.bodyRB.transform.position.y < -4f) {
			reward = -10;
			done = true;
			return;
		}


		if (controller.gameLost) {
			reward = -10;
			done = true;
			return;
		}

		//Monitor.Log ("reward", reward, MonitorType.slider, controller.bodyRB.transform);

	}



	public override void AgentReset ()
	{
		controller.ResetGame ();
	}



}
