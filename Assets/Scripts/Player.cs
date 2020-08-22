using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour, ITakeDamage
{
	private bool _isFacingRight;
	private CharacterController2D _controller;
	private float _normalizedHorizontalSpeed;
	 
	public float MaxSpeed = 8f;
	public float SpeedAccelerateionOnGround = 10f;
	public float SpeedAccelerateionInAir = 5f;
	public int MaxHealth = 100;
	public GameObject OuchEffect;
	public Projectile Projectile;
	public float FireRate;
	public Transform ProjectileFireLocation;
	public AudioClip PlayerHitSound;
	public AudioClip PlayerShootSound;
	public AudioClip PlayerHealthSound;
	public Animator Animator;

	public int Health { get; private set; }
	public bool IsDead {get; private set;} 

	private float _canFireIn;

	public void Awake ()
	{
		Health = MaxHealth;
		_controller = GetComponent<CharacterController2D> ();
		//Debug.Log (_controller);
		if(_controller == null) Debug.Log("can not get CharacterController2D");
		_isFacingRight = transform.localScale.x > 0;
	}
	public void Start()
	{

	}

	public void Update()
	{
		_canFireIn -= Time.deltaTime;

		if (!IsDead)
			HandleInput ();

		var movementFactor = _controller.State.IsGrounded ? SpeedAccelerateionOnGround : SpeedAccelerateionInAir;
		//movementFactor = 10f;
		//_normalizedHorizontalSpeed = 0f;
		// _controller.SetHorizontalForce (8);
		float spd = _normalizedHorizontalSpeed * MaxSpeed;
		//float td1 = Time.deltaTime;
		float td2 = Time.deltaTime * movementFactor;
		//float xlerp = Mathf.Lerp(_controller.Velocity.x,spd,td2);
		//string XX = string.Format (
		//	"(Lerp: a:{0} b:{1} td1:{2} td2:{3} lerp:{4} mvf:{5})",
		//	_controller.Velocity.x, 
		//	spd, 
		//	td1, 
		//	td2, 
		//	xlerp,
		//	movementFactor);
		//Debug.Log(XX);

		if (IsDead)
			_controller.SetHorizontalForce (0);
		else
			_controller.SetHorizontalForce(Mathf.Lerp(_controller.Velocity.x,spd,td2));
		
		Animator.SetBool ("IsGrounded", _controller.State.IsGrounded);

		float xx = Mathf.Abs (_controller.Velocity.x) / MaxSpeed;
		Debug.Log(string.Format ("Speed {0}", xx));
		Animator.SetFloat ("Speed", xx);
	}

	public void FinishLevel()
	{
		enabled = false;
		_controller.enabled = false;
		// collider2D.enabled = false;
	}

	public void Kill()
	{
		_controller.HandleCollisions = false;
		this.gameObject.GetComponent<Collider2D>().enabled = false;
		IsDead = true;
		Health = 0;
		Animator.SetBool ("IsDead", IsDead);

		// Debug.Log("dead");
		_controller.SetForce (new Vector2 (0, 20));

	}

	public void RespawnAt (Transform spawnPoint)
	{
		if (!_isFacingRight)
			Flip ();
		IsDead = false;
		Animator.SetBool ("IsDead", IsDead);
		this.gameObject.GetComponent<Collider2D>().enabled = true;
		_controller.HandleCollisions = true;
		Health = MaxHealth;

		transform.position = spawnPoint.position;

	}

	public void TakeDamage(int damage, GameObject instigator)
	{
		AudioSource.PlayClipAtPoint (PlayerHitSound, transform.position);
		FloatingText.Show (string.Format ("-{0}", damage), 
			"PlayerDamageText",
			new FromWorldPointTextPositioner (Camera.main, transform.position, 5f, 60f));
		
		Instantiate (OuchEffect, transform.position, transform.rotation);
		Health -= damage;

		if (Health <= 0)
			LevelManager.Instance.KillPlayer ();
	}

	public void GiveHealth(int health, GameObject instigator)
	{
		AudioSource.PlayClipAtPoint (PlayerHealthSound, transform.position);
		FloatingText.Show (string.Format ("+{0}", health), 
			"PlayerGotHealthText",
			new FromWorldPointTextPositioner (Camera.main, transform.position, 5f, 60f));
		Health = Mathf.Min (Health += health, MaxHealth);
	}
				
	private void HandleInput()
	{
		if(Input.GetKey(KeyCode.D))
		{
			_normalizedHorizontalSpeed = 1;
			//Debug.Log("D " + _normalizedHorizontalSpeed);
			if(!_isFacingRight)
				Flip();
		}
		else if(Input.GetKey(KeyCode.A))
		{
			_normalizedHorizontalSpeed = -1;
			//Debug.Log("A " + _normalizedHorizontalSpeed);
			if(_isFacingRight)
				Flip();
		}
		else
		{
			_normalizedHorizontalSpeed = 0;
		}

		if(_controller.CanJump && Input.GetKeyDown(KeyCode.Space))
		{
			_controller.Jump();
		}

		if (Input.GetMouseButtonDown (0))
			FireProjectile ();
	}

	private void FireProjectile()
	{
		if (_canFireIn > 0)
			return;

		var direction = _isFacingRight ? Vector2.right : -Vector2.right;
		var projectile = (Projectile)Instantiate (Projectile, 
			                ProjectileFireLocation.position, ProjectileFireLocation.rotation);
		projectile.Initialize (gameObject, direction, _controller.Velocity);
		//projectile.transform.localScale = new Vector3 (_isFacingRight ? 1 : -1, 1, 1);
		_canFireIn = FireRate;		

		AudioSource.PlayClipAtPoint (PlayerShootSound, transform.position);
		Animator.SetTrigger ("Fire");
	}

	private void Flip()
	{
		transform.localScale = new Vector3 (-transform.localScale.x,transform.localScale.y,transform.localScale.z);
		_isFacingRight = transform.localScale.x > 0;
	}
}