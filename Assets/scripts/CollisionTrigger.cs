using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTrigger : MonoBehaviour {

	//private BoxCollider2D playerCollider;

	[SerializeField]
	private BoxCollider2D platformCollider;
	[SerializeField]
	private BoxCollider2D platformTrigger;

	void Start () {
		//playerCollider = GameObject.Find ("player").GetComponent<BoxCollider2D>();
		Physics2D.IgnoreCollision (platformCollider,platformTrigger,true);
	}
	
	// Update is called once per frame
	void OnTriggerEnter2D (Collider2D other) {
		if(other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy"){
			Physics2D.IgnoreCollision (platformCollider,other,true);
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if(other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy"){
			Physics2D.IgnoreCollision (platformCollider,other,false);
		}
	}
}
