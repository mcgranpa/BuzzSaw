using UnityEngine;

public class PathedProjectileSpawner : MonoBehaviour
{
	public Transform Destination;
	public PathedProjectile Projectile;

	public GameObject SpawnEffect;
	public float Speed;
	public float FireRate;
	public AudioClip SpawnedProjectileSound;
	public Animator Animator;


	private float _nextShotInSeconnds;

	public void Start ()
	{
		_nextShotInSeconnds = FireRate;
	}
	
	public void Update ()
	{
		if ((_nextShotInSeconnds -= Time.deltaTime) > 0)
			return;
	
		_nextShotInSeconnds = FireRate;
		var projectile = (PathedProjectile)Instantiate (Projectile, transform.position, transform.rotation);
		projectile.Initalize (Destination, Speed);

		if (SpawnEffect != null)
			Instantiate (SpawnEffect, transform.position, transform.rotation);
		
		if ( SpawnedProjectileSound != null)
			AudioSource.PlayClipAtPoint (SpawnedProjectileSound, transform.position);

		if (Animator != null)
			Animator.SetTrigger ("Fire"); 
	}

	public void OnDrawGizmos()
	{
		if (Destination == null)
			return;
		Gizmos.color = Color.red;
		Gizmos.DrawLine (transform.position, Destination.position);
	}

}

