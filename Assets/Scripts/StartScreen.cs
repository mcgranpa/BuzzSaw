using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
	public string FirstLevel;

	void Update ()
	{
		if (!Input.GetMouseButtonDown (0))
			return;
		GameManager.Instance.Reset ();
		SceneManager.LoadScene(FirstLevel);
	}
}

