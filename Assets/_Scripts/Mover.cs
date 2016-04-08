using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {

	private float speed = 0.1f; 

	void FixedUpdate(){

		transform.Translate (0, -1 * speed, 0);
	}
}
