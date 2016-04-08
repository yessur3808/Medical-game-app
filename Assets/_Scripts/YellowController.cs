using UnityEngine;
using System.Collections;

public class YellowController : MonoBehaviour {
	private bool checking = false;
	private Collider someThingIn=null;
	public int ScoreValue;
	private GameController gameController;

	public GameObject explosion;
	public bool areaPress = false;
	public bool areaPressStay = false;
	public bool areaPressEnd = false;
	public bool areaNormal = false;
	
	private ChangeScence change;
	
	public int Number;
	
	void Start()
	{
		Number = ChangeScence.SoundEffectNo;
		GameObject changeObject = GameObject.FindWithTag ("Manager");
		if (changeObject != null) {
			change = changeObject.GetComponents<ChangeScence>()[0];
		}
		if (change == null) {
			Debug.Log ("Cannot find the changeObject Script");
		}
		
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponents<GameController>()[0];
		}
		if (gameController == null) {
			Debug.Log ("Cannot find the GameController Script");
		}
	}
	void Update () {
		if ( (Input.GetKeyDown (KeyCode.Z) && checking == true && someThingIn != null) ||
		    areaPress && checking == true && someThingIn != null) {

			Jumping(someThingIn);
		}
	}
	
	void OnTriggerEnter(Collider other)
	{

		checking = true;
		someThingIn = other;
		
	}
	void OnTriggerExit(Collider other)
	{
		someThingIn = null;
		checking = false;
	}
	void Jumping(Collider other)
	{
		if (other.tag == "Boundary") {
			return;
		}
		else if (other.tag == "Ball") {
			Destroy(other.gameObject);
			gameController.AddScore(ScoreValue);
			Instantiate(explosion,transform.position,transform.rotation);
			change.AddSoundEffect(Number);
		}
		checking = false;
	}
}	
