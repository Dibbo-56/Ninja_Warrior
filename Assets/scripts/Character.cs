using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour {


	[SerializeField]
	protected Transform knifePos;

	[SerializeField]
	protected float movementSpeed;

	protected bool facingRight;

	[SerializeField]
	private GameObject KnifePrefabs;

	[SerializeField]
	protected int health;

	[SerializeField]
	private EdgeCollider2D swordCollider;

	[SerializeField]
	private List<string> damagesources;

	public abstract bool IsDead{ get;}

	public bool Attack { get ; set ;}

	public bool TakingDamage{ get; set;}

	public Animator MyAnimator{ get; private set;}

	public EdgeCollider2D SwordCollider
	{
		get{ 
			return swordCollider;
		}
	}

	public virtual void Start () {
		
		facingRight = true;
		MyAnimator = GetComponent<Animator> ();
	}
		

	void Update () {
		
	}

	public abstract IEnumerator TakeDamage ();

	public abstract void Death (); 

	public void ChangeDirection(){
		facingRight = !facingRight;
		transform.localScale = new Vector3 (transform.localScale.x * -1,1,1);
	}

	public virtual void ThrowKnife(int value){
		if(facingRight){
			GameObject tmp = (GameObject)Instantiate (KnifePrefabs,knifePos.position,Quaternion.Euler(new Vector3(0,0,-90)));
			tmp.GetComponent<Knife>().Initialize(Vector2.right);
		}
		else{
			GameObject tmp = (GameObject)Instantiate (KnifePrefabs,knifePos.position,Quaternion.Euler(new Vector3(0,0,90)));
			tmp.GetComponent<Knife>().Initialize(Vector2.left);
		}
	}

	public void MeleeAttack(){
		SwordCollider.enabled = true;
	}

	public virtual void OnTriggerEnter2D(Collider2D other){

		if(damagesources.Contains(other.tag)){
			StartCoroutine (TakeDamage());
		}
	
	}
}
