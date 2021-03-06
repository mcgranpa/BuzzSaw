﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointStar : MonoBehaviour, IPlayerRespawnListener
{
	public GameObject Effect;
	public int PointsToAdd = 10;
	public AudioClip HitStarSound;
	public Animator Animator;
	public SpriteRenderer Renderer;

	private bool _isCollected = false;

	public void OnTriggerEnter2D(Collider2D other)
	{
		if (_isCollected)
			return;
				
		if (other.GetComponent<Player> () == null)
			return;

		_isCollected = true;
		Animator.SetTrigger ("Collect");

		if ( HitStarSound != null)
			AudioSource.PlayClipAtPoint (HitStarSound, transform.position);



		GameManager.Instance.AddPoints (PointsToAdd);
		Instantiate (Effect, transform.position, transform.rotation);

		FloatingText.Show (string.Format ("+{0}!", PointsToAdd), 
			"PointStarText",
			new FromWorldPointTextPositioner (Camera.main, transform.position, 10f, 25f));

		_isCollected = true;
		Animator.SetTrigger ("Collect");
		//gameObject.SetActive(false);
	}

	public void FinishAnimationEvent()
	{
		//gameObject.SetActive(false);
		Renderer.enabled = false;
		Animator.SetTrigger ("Reset");
	}

	public void OnPlayerRespawnInThisCheckpoint(Checkpoint checkpoint, Player player)
	{
		_isCollected = false;
		//gameObject.SetActive (true);
		Renderer.enabled = true;
	}

}
