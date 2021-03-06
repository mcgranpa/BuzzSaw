﻿using UnityEngine;
using System.Collections;

public class GiveDamageToPlayer : MonoBehaviour
{
	public int DamageToGive = 10;
	private Vector2
		_lastPositoin,
		_velocity;


	public void LateUpdate ()
	{
		_velocity = (_lastPositoin - (Vector2) transform.position) / Time.deltaTime;
		_lastPositoin = transform.position;
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		var player = other.GetComponent<Player> ();
		if (player == null)
			return;

		player.TakeDamage (DamageToGive, gameObject);
		var controller = player.GetComponent <CharacterController2D> ();
		var totalVelocity = controller.Velocity + _velocity;

		controller.SetForce (new Vector2 (
			-1 * Mathf.Sign (totalVelocity.x) * Mathf.Clamp (Mathf.Abs (totalVelocity.x) * 6, 10, 40),
			-1 * Mathf.Sign (totalVelocity.y) * Mathf.Clamp (Mathf.Abs (totalVelocity.y) * 6, 5, 30)));
	}
		
}

