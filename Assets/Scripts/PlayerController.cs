using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour {

	public float speed;
	private Rigidbody rb;
	public delegate void forwardsAction();
	public delegate void backwardsAction();
	public static event forwardsAction OnForwardsTrigger;
	public static event backwardsAction OnBackwardsTrigger;
	 

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector3 movement = new Vector3 (moveHorizontal, 0, moveVertical);

		rb.AddForce (movement * speed);
	}

	void OnTriggerEnter(Collider other) 
	{
		if (other.gameObject.CompareTag ("forwardsTrigger"))
		{
			if (OnForwardsTrigger != null) {
				OnForwardsTrigger ();
			}
		}
		else if (other.gameObject.CompareTag ("backwardsTrigger"))
		{
			if (OnBackwardsTrigger != null) {
				OnBackwardsTrigger ();
			}
		}
	}

}
