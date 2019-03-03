using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour {

	AudioSource audioSource;
	public AudioClip[] footsteps;

	public Rigidbody2D bodyRB;
	public Rigidbody2D leftPivotRB;
	public Rigidbody2D rightPivotRB;
	public HingeJoint2D leftFoot;
	public HingeJoint2D rightFoot;

	public Transform leftForce;
	public Transform rightForce;

	public bool debugMode;
	public float WallDistance;
	public float fallingCriticalAngle;

	public float currentScore;
	public float bestScore;

	public Text currentScoreText;
	public Text bestScoreText;
	public GameObject bestLine;
	public Transform mainCamera;

	public bool gameLost;
	private bool resetting;


	private int footstepID;
	private int stepCount = 0;
	[HideInInspector]
	public float clickTime = 0;
	public bool isPressed = false;
	public float heightDifference;
	public float episodeStartTime;

	public AnimationCurve forceFadeIn;

	//cx constants
	private const float pivotDownBasicForce = 0.7f;
	private const float pivotDownForceStrength = 1300f;

	private const float bodyDownBasicForce = 1.2f;
	private const float bodyDownForceStrength = 2000f;

	private const float pivotUpForceStrength = 1700f;

	private const float bodyUpForceStrength = 2200f;

	//for agent reset
	private Vector3 bodyInitialPosition;
	private Quaternion bodyInitialRotation;

	private Vector3 leftLegInitialPosition;
	private Quaternion leftLegInitialRotation;

	private Vector3 leftFootInitialPosition;
	private Quaternion leftFootInitialRotation;

	private Vector3 rightLegInitialPosition;
	private Quaternion rightLegInitialRotation;

	private Vector3 rightFootInitialPosition;
	private Quaternion rightFootInitialRotation;

	private Vector3 leftPivotInitialPosition;
	private Vector3 rightPivotInitialPosition;

	private Vector3 cameraInitialPosition;

	private float startingPositionX;

	void Start () {
		audioSource = this.GetComponent<AudioSource> ();
		//PlayerPrefs.SetFloat ("bestScore", 0f);
		ResetForcePoints ();
		SetBestScoreCanvas ();

		gameLost = false;
		resetting = false;

		episodeStartTime = 0f;
		startingPositionX = bodyRB.transform.position.x;

	}

	private void SetBestScoreCanvas(){
		bestScore = PlayerPrefs.GetFloat ("bestScore");
		string bestScoreString = bestScore.ToString ("F2") + " m";

		bestScoreText.text = bestScoreString;
		bestLine.transform.position = new Vector2 (0, -10f);

		if (bestScore > 0f) {
			bestLine.transform.position = new Vector2 (0, bestScore);
			bestLine.transform.Find("BestScoreText").GetComponent<TextMesh>().text = bestScoreString;
		}
	}


	private void UpdateForce(Rigidbody2D firstPivot, Transform force, Rigidbody2D secondPivot, float coefficient){
		if(!firstPivot.isKinematic){
			force.position = new Vector2(startingPositionX + coefficient * WallDistance, (bodyRB.transform.position.y - secondPivot.transform.position.y)/3 + firstPivot.transform.position.y - coefficient * heightDifference);
		}
	}

	private void SetFootMotors(bool valueToSet){
		leftFoot.useMotor = valueToSet;
		rightFoot.useMotor = valueToSet;
	}

	private void UpdateCameraPosition() {
		mainCamera.position = Vector3.Lerp (mainCamera.position, new Vector3 (0, currentScore + 4f, -1f), Time.deltaTime/5f);
	}

	public void DownPressed(float ratio) {
		if (leftPivotRB.isKinematic && rightPivotRB.isKinematic && !isPressed) {

			float clickPositionRatio = ratio;
			Rigidbody2D pivotToForce;
			Rigidbody2D otherPivot;
			float forceDirection;

			if (clickPositionRatio >= 0.5f) {
				pivotToForce = leftPivotRB;
				otherPivot = rightPivotRB;
				forceDirection = -1f;
			} else {
				pivotToForce = rightPivotRB;
				otherPivot = leftPivotRB;
				forceDirection = 1f;
			}

			clickTime = Time.time;
			isPressed = true;
			ResetForcePoints ();
			pivotToForce.isKinematic = false;
			heightDifference = pivotToForce.transform.position.y - otherPivot.transform.position.y;

			pivotToForce.AddForce(new Vector2(forceDirection, pivotDownBasicForce - heightDifference / 4f) * pivotDownForceStrength);
			bodyRB.AddForce (new Vector2 (forceDirection * 0.4f, bodyDownBasicForce - heightDifference / 4f) * bodyDownForceStrength);

			stepCount++;
		}
	}


	private void CheckKeyDown() {
		if (Input.GetKeyDown (KeyCode.A)) { 
			DownPressed (0f);
		} else if (Input.GetKeyDown (KeyCode.D)) {
			DownPressed (1f);
		}
	}

	private void ForceToWallIfNeeded(Rigidbody2D pivot, Transform pivotForce, Rigidbody2D other, Transform otherForce, float direction) {
		if (!other.isKinematic) {
			return;
		}

		float forceCoefficient = getCurrentForce ();
		isPressed = false;	

		Vector3 pivotForceDirection = (pivotForce.position - other.transform.position).normalized;
		Vector3 forceToAdd = pivotForceDirection * pivotUpForceStrength * forceCoefficient;
		pivot.AddForce (forceToAdd);

		Vector3 bodyForceDirection = (pivotForce.position - bodyRB.transform.position + new Vector3 (0.1f * direction, 1f)).normalized;
		forceToAdd = bodyForceDirection * bodyUpForceStrength * forceCoefficient;
		bodyRB.AddForce (forceToAdd);

	}

	public void UpPressed() {
		if (isPressed) {
			ForceToWallIfNeeded (leftPivotRB, leftForce, rightPivotRB, rightForce, 1f);
			ForceToWallIfNeeded (rightPivotRB, rightForce, leftPivotRB, leftForce, -1f);
		}
	}

	private void CheckKeyUp() {
		if (Input.GetKeyDown(KeyCode.S)) {
			UpPressed ();
		}
	}

	private void UpdateForces() {
		if (isPressed){
			UpdateForce(leftPivotRB, leftForce, rightPivotRB, 1f);
			UpdateForce(rightPivotRB, rightForce, leftPivotRB, -1f);
		}
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.R)) {
			ResetGame ();
		}

		UpdateCameraPosition ();
		CheckKeyDown ();
		CheckKeyUp ();

		UpdateForces ();
		SetFootMotors(leftPivotRB.isKinematic && rightPivotRB.isKinematic);

		FixPhysicsErrors ();
		CheckForFall();

		if (gameLost && !resetting) {
			resetting = true;
			Invoke ("ResetGame", 2f);
		}

	}


	private void FreezeSinglePivot(bool mustFreeze, Rigidbody2D pivotToFreeze){
		if (mustFreeze) {
			if (!pivotToFreeze.isKinematic) {
				PlayFootstepSound ();
			}
			pivotToFreeze.isKinematic = true;
			pivotToFreeze.velocity = new Vector2 (0f, 0f);
			CalculateCurrentScore (pivotToFreeze.position.y);
		}
	}

	public void FreezePivots(bool leftFreeze, bool rightFreeze) {
		if (!isPressed) {
			FreezeSinglePivot (leftFreeze, leftPivotRB);
			FreezeSinglePivot (rightFreeze, rightPivotRB);
		}
	}

	private void SlipSinglePivot(bool mustSlip, Rigidbody2D pivot){
		if (mustSlip) {
			pivot.position = new Vector2(pivot.position.x, pivot.position.y - Time.deltaTime * 0.6f);
		}
	}

	public void SlipPivots(bool leftSlip, bool rightSlip)
	{
		SlipSinglePivot (leftPivotRB.isKinematic && leftSlip, leftPivotRB);
		SlipSinglePivot (rightPivotRB.isKinematic && rightSlip, rightPivotRB);
	}

	void ResetForcePoints(){
		leftForce.position = new Vector2(startingPositionX + WallDistance, leftPivotRB.position.y);
		rightForce.position = new Vector2(startingPositionX - WallDistance, rightPivotRB.position.y);
	}


	void checkPivotForFalling(Rigidbody2D pivot) {
		if (!pivot.isKinematic) {
			return;
		}

		float angle = Vector2.Angle (bodyRB.position - pivot.position, Vector2.down);

		if (angle < fallingCriticalAngle) {
			pivot.isKinematic = false;
			gameLost = true;
		}
	}

	void CheckForFall() {
		checkPivotForFalling (leftPivotRB);
		checkPivotForFalling (rightPivotRB);
	}


	void FixPhysicsErrors(){
		if(rightPivotRB.position.x < startingPositionX - WallDistance){
			rightPivotRB.position = new Vector2(startingPositionX - WallDistance, rightPivotRB.position.y);
		}

		if(leftPivotRB.position.x > startingPositionX + WallDistance){
			leftPivotRB.position = new Vector2(startingPositionX + WallDistance, leftPivotRB.position.y);
		}
	}

	void PlayFootstepSound() {
		if (gameLost) {
			return;
		}
		audioSource.clip = footsteps [Random.Range(0, footsteps.Length - 1)];
		audioSource.Play ();
	}


	public void ResetGame() {
		SceneManager.LoadScene (1);
	}

	void CalculateCurrentScore(float height){
		if(height > currentScore){
			currentScore = height;
			currentScoreText.text = currentScore.ToString("F2") + " m";
		}

		if(currentScore > bestScore){
			bestScore = currentScore;
			PlayerPrefs.SetFloat("bestScore", bestScore);
		}
	}


	public float getCurrentForce() {
		if (isPressed) {
			return forceFadeIn.Evaluate (Time.time - clickTime);
		} else {
			return 0f;
		}
	}

	public void touchDetected(Touch touch) {
		
		if (isPressed) {
			UpPressed ();
		} else {
			DownPressed (touch.position.x / Screen.width);
		}
	}
}
