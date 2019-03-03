using UnityEngine;
using System.Collections;

public class CollisionDetectors : MonoBehaviour {

    public bool thisIsLeft;
    Controller controllerScript;

    void Start()
    {
		GameObject game = transform.parent.transform.parent.gameObject;
		controllerScript = game.GetComponentInChildren<Controller> ();
		//controllerScript = GameObject.Find("Controller").GetComponent<Controller>();
    }

	public void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Ice")
		{
			controllerScript.FreezePivots(thisIsLeft, !thisIsLeft);
		}

		if (other.gameObject.tag == "Ice")
		{
			controllerScript.SlipPivots(thisIsLeft, !thisIsLeft);
		}
	}


	public void OnCollisionStay2D(Collision2D other)
    {
		if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Ice")
		{
			controllerScript.FreezePivots(thisIsLeft, !thisIsLeft);
		}

        if (other.gameObject.tag == "Ice")
        {
            controllerScript.SlipPivots(thisIsLeft, !thisIsLeft);
        }
    }
}
