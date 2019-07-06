using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void DeadEventHandler();


public class player : Character {

	private static player instance; 

	public event DeadEventHandler Dead;


	public static player Instance
	{
		get{
			if(instance == null){
				instance = GameObject.FindObjectOfType<player> ();
			}
			return instance;
		}
	}

	[SerializeField]
	private Transform[] groundPoints;

	[SerializeField]
	private float groundRadius;

	[SerializeField]
	private LayerMask whatIsGround;

	[SerializeField]
	private bool airControl;

	[SerializeField]
	private float jumpForce;

	private bool immortal=false;

	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private float immortalTime;

	private float direction;

	private bool move;

	private float btnHorizontal;

	[SerializeField]
	public GameObject KnifePrefab; 

	public Rigidbody2D MyRigidbody{ get ; set ;}

	public bool Slide { get ; set ;}

	public bool Jump { get ; set ;}

	public bool OnGround { get ; set ;}

	private Vector2 startPos;


	public override bool IsDead
	{
		get{ 

			if(health <= 0){
				OnDead ();
			}

			return health <= 0;
		}
	}


	// Use this for initialization
	public override void Start () {


		base.Start ();

		startPos = transform.position;

		spriteRenderer = GetComponent<SpriteRenderer> ();

		MyRigidbody = GetComponent<Rigidbody2D> ();
	
	}

	void Update(){
		if(!TakingDamage && !IsDead){
			if(transform.position.y<= -14f){
				//MyRigidbody.velocity = Vector2.zero;
				//transform.position = startPos;
				Death();
			}

			HandleInput ();	
		}

	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if(!TakingDamage && !IsDead){
			float horizontal = Input.GetAxis ("Horizontal");

			OnGround = IsGrounded ();

			if (move) {
				this.btnHorizontal = Mathf.Lerp (btnHorizontal, direction, Time.deltaTime * 2);
				HandleMovement (btnHorizontal);
				Flip (direction);
			}
			else {
				
				HandleMovement (horizontal);

				Flip (horizontal);
			}
				

			HandleLayers ();	
		}
			
	}

	public void OnDead()
	{
		if(Dead != null){
			Dead ();
		}
	}


	private void HandleMovement(float horizontal)
	{
		if(MyRigidbody.velocity.y<0){
			MyAnimator.SetBool ("land",true);
		}

		if(!Attack && !Slide && (OnGround || airControl)){
			MyRigidbody.velocity = new Vector2 (horizontal * movementSpeed, MyRigidbody.velocity.y);
		}

		if(Jump && MyRigidbody.velocity.y==0){
			MyRigidbody.AddForce (new Vector2(0,jumpForce));
		}

		MyAnimator.SetFloat ("speed",Mathf.Abs(horizontal));
	}



	private void HandleInput() 
	{
		if(Input.GetKeyDown(KeyCode.Space)){
			MyAnimator.SetTrigger ("jump");
		}
		if(Input.GetKeyDown(KeyCode.LeftShift)){
			MyAnimator.SetTrigger ("attack");
		}
		//if(Input.GetKeyDown(KeyCode.LeftControl)){
		//	MyAnimator.SetTrigger ("slide");
		//}
		if(Input.GetKeyDown(KeyCode.V)){
			MyAnimator.SetTrigger ("throw");
			ThrowKnife (0);
		}
	}

	private void Flip(float horizontal)
	{
		if((horizontal>0 && !facingRight) ||(horizontal<0 && facingRight)){
			//facingRight = !facingRight;
			//transform.localScale = new Vector3 (transform.localScale.x * -1,1,1);
			ChangeDirection();
		}
	}



	private bool IsGrounded()
	{
		if(MyRigidbody.velocity.y<=0){
			foreach(Transform point in groundPoints){
				Collider2D[] colliders = Physics2D.OverlapCircleAll (point.position,groundRadius,whatIsGround);

				for(int i=0;i<colliders.Length;i++){
					if(colliders[i].gameObject != gameObject){
						
						return true;
					}
				}
			}
		}
		return false;
	}

	private void HandleLayers()
	{
		if (!OnGround) {
			MyAnimator.SetLayerWeight (1, 1);
		}
		else {
			MyAnimator.SetLayerWeight (1, 0);
		}
	}

	public override void ThrowKnife(int value)
	{
		if(!OnGround && value == 1 || OnGround && value == 0){
			if(facingRight){
				GameObject tmp = (GameObject)Instantiate (KnifePrefab, knifePos.position, Quaternion.Euler(new Vector3(0,0,-90)));
				tmp.GetComponent<Knife> ().Initialize (Vector2.right);
			}
			else{
				GameObject tmp = (GameObject)Instantiate (KnifePrefab, knifePos.position, Quaternion.Euler(new Vector3(0,0,90)));
				tmp.GetComponent<Knife> ().Initialize (Vector2.left);
			}		
		}


	}

	private IEnumerator IndicateImmortal(){
		while(immortal){
			spriteRenderer.enabled = false;
			yield return new WaitForSeconds (.1f);
			spriteRenderer.enabled = true;
			yield return new WaitForSeconds (.1f);
		}
	}

	public override IEnumerator TakeDamage (){

		if(!immortal){
			
			health -= 10;

			if (!IsDead) {
				MyAnimator.SetTrigger ("damage");
				immortal = true;
				StartCoroutine (IndicateImmortal());
				yield return new WaitForSeconds (immortalTime);

				immortal = false; 

			} else {
				MyAnimator.SetLayerWeight (1,0);
				MyAnimator.SetTrigger ("die");
			}	
		}
			
	}

	public override void Death()
	{
		MyRigidbody.velocity = Vector2.zero;
		MyAnimator.SetTrigger ("idle");
		health = 30;
		transform.position = startPos;
	}

	public void BtnJump(){
		MyAnimator.SetTrigger ("jump");
		Jump = true;
	}

	public void BtnThrow(){
		MyAnimator.SetTrigger ("throw");
		ThrowKnife (0);
	}

	public void BtnAttack(){
		MyAnimator.SetTrigger ("attack");
	}

	public void BtnMove(float direction){
		this.direction = direction;
		this.move = true;
	}

	public void BtnStopMove(){
		this.direction = 0;
		this.btnHorizontal = 0;
		this.move = false;
	}
}
