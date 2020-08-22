using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlatform : MonoBehaviour {

	public float JumpMagnitude = 20;
	public AudioClip JumpSound;

	public void ControllerEnter2D (CharacterController2D controller)
	{
		controller.SetVerticalForce (JumpMagnitude);
		if ( JumpSound != null)
			AudioSource.PlayClipAtPoint (JumpSound, transform.position);

	}
	public void ControllerExit2D (CharacterController2D controller)
	{
		//controller.SetVerticalForce(JumpMagnitude)

	}
	public void ControlleStay2D (CharacterController2D controller)
	{
		//controller.SetVerticalForce(JumpMagnitude)

	}
}
